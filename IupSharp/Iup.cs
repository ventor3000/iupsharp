using System;

namespace IupSharp
{
    public static class Iup
    {
        

        public static void Message(string title, string message) => NativeIup.Message(title, message);
        public static void Message(string message) => Message("", message);
        public static void MessageError(Control parent, string message) => NativeIup.MessageError(parent==null ? IntPtr.Zero:parent.Handle, message);
        public static void MessageError(string message) => NativeIup.MessageError(IntPtr.Zero, message);
        
        public static OpenResult Open()
        {
            int res=NativeIup.Open();
            OpenResult rr;
            switch(res)
            {
                case NativeIup.IUP_ERROR:
                    rr=OpenResult.Error;
                    break;
                case NativeIup.IUP_OPENED:
                    rr=OpenResult.AlreadyOpen;
                    break;
                default:
                    rr=OpenResult.NoError;
                    break;
            }

            if (rr != OpenResult.Error)
            {
                NativeIup.SetStrGlobal("UTF8MODE", "YES");
                NativeIup.SetStrGlobal("UTF8MODE_FILE", "YES");
            }

            return rr;
        }

        public static void Close()
        {
            NativeIup.Close();
            IupObject.InvalidateAll();
        }

        public static void MainLoop() => NativeIup.IupMainLoop();
        public static void ExitLoop() => NativeIup.IupExitLoop();
        public static void Flush() => NativeIup.IupFlush();

        /// <summary>
        /// Shows a modal dialog built from the given parameters, and returns true if
        /// the user pressed OK. Each parameter's Value is updated in place, so read
        /// the values from the Param objects after the call.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="parameters">The fields, in the order they should appear.</param>
        /// <returns>True if OK was pressed; false if the user cancelled.</returns>
        /// <example>
        /// <code>
        /// var name = Param.String("Name", "Robert");
        /// var age  = Param.Int("Age", 40, min: 0, max: 150);
        /// var ok   = Param.Bool("Subscribe", true);
        ///
        /// if (Iup.GetParams("Details", name, age, ok))
        ///     Console.WriteLine($"{name.Value}, {age.Value}, {ok.Value}");
        /// </code>
        /// </example>
        public static bool GetParams(string title, params Param[] parameters) =>
            GetParams(title, null, parameters);

        /// <summary>
        /// Shows a modal dialog built from the given parameters, with a callback
        /// invoked as values change and as buttons are pressed.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="callback">
        /// Called when a value changes and when a button is pressed. Set the callback
        /// data's Accept to false to reject the change or the button action. It can be
        /// null.
        /// </param>
        /// <param name="parameters">The fields, in the order they should appear.</param>
        /// <returns>True if OK was pressed; false if the user cancelled.</returns>
        /// <exception cref="ArgumentException">No parameters were given.</exception>
        public static bool GetParams(string title, ParamCallback callback, params Param[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                throw new ArgumentException("At least one parameter is required.", nameof(parameters));

            // IUP counts only the parameters that consume a pointer. Separators and
            // button titles appear in the format string but not in the data array.
            var valued = new System.Collections.Generic.List<Param>();
            foreach (Param p in parameters)
            {
                if (p == null)
                    throw new ArgumentException("A parameter is null.", nameof(parameters));

                if (p.HasValue)
                    valued.Add(p);
            }

            // Each fragment is terminated by a newline.
            var format = new System.Text.StringBuilder();
            foreach (Param p in parameters)
                format.Append(p.Format).Append('\n');

            IntPtr[] data = new IntPtr[valued.Count];

            // Keep the delegate alive for the duration of the call; IUP holds a raw
            // function pointer to it.
            Iparamcb native = null;

            if (callback != null)
            {
                native = (dlg, index, userData) =>
                {
                    try
                    {
                        ParamAction action = index >= 0
                            ? ParamAction.ValueChanged
                            : (ParamAction)index;

                        Param changed = (index >= 0 && index < valued.Count) ? valued[index] : null;

                        var cb = new ParamCallbackData(action, index >= 0 ? index : -1, changed);
                        callback(cb);

                        // IUP wants 1 to accept and 0 to reject.
                        return cb.Accept ? 1 : 0;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[IupSharp] unhandled exception in GetParams callback: {ex}");
                        return 1;
                    }
                };
            }

            try
            {
                for (int i = 0; i < valued.Count; i++)
                {
                    valued[i].Allocate();
                    data[i] = valued[i].Buffer;
                }

                int result = NativeIup.GetParamv(
                    title, native, IntPtr.Zero, format.ToString(),
                    valued.Count, 0, data);

                if (result == 1)
                {
                    foreach (Param p in valued)
                        p.ReadBack();
                }

                return result == 1;
            }
            finally
            {
                foreach (Param p in valued)
                    p.Free();

                // Make the lifetime requirement explicit rather than relying on the
                // JIT keeping the local alive across the P/Invoke.
                GC.KeepAlive(native);
            }
        }
    

}
}
