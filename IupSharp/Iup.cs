using System;

namespace IupSharp
{
    public static class Iup
    {
        

        public static void Message(string title, string message) => IupNative.Message(title, message);
        public static void Message(string message) => Message("", message);
        public static void MessageError(Control parent, string message) => IupNative.MessageError(parent==null ? IntPtr.Zero:parent.Handle, message);
        public static void MessageError(string message) => IupNative.MessageError(IntPtr.Zero, message);
        
        public static OpenResult Open()
        {
            int res=IupNative.Open();
            OpenResult rr;
            switch(res)
            {
                case IupNative.IUP_ERROR:
                    rr=OpenResult.Error;
                    break;
                case IupNative.IUP_OPENED:
                    rr=OpenResult.AlreadyOpen;
                    break;
                default:
                    rr=OpenResult.NoError;
                    break;
            }

            if (rr != OpenResult.Error)
            {
                IupNative.SetStrGlobal("UTF8MODE", "YES");
                IupNative.SetStrGlobal("UTF8MODE_FILE", "YES");
            }

            return rr;
        }

        public static void Close()
        {
            IupNative.Close();
            IupObject.InvalidateAll();
        }

        public static void MainLoop() => IupNative.IupMainLoop();
        public static void ExitLoop() => IupNative.IupExitLoop();
        public static void Flush() => IupNative.IupFlush();

    }
}
