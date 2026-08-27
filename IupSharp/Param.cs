using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace IupSharp
{
    /// <summary>
    /// A single field in a GetParams dialog. Create one with the static factory
    /// methods on <see cref="Param"/>, pass them to
    /// <see cref="Iup.GetParams(string, Param[])"/>, and read the result from the
    /// Value property afterwards.
    /// </summary>
    /// <remarks>
    /// IUP's own API is a printf-style format string plus a matching list of pointers,
    /// with no checking that the two agree - a mismatch corrupts memory rather than
    /// raising an error. This class builds both sides together so they cannot
    /// disagree, and owns the unmanaged storage for the duration of the call.
    /// </remarks>
    public abstract class Param
    {
        /// <summary>The label shown to the left of the field.</summary>
        public string Title { get; }

        /// <summary>The IUP format fragment for this parameter, without the newline.</summary>
        internal abstract string Format { get; }

        /// <summary>
        /// True if IUP counts this as a parameter that consumes a pointer. Separators
        /// and button-title parameters do not.
        /// </summary>
        internal virtual bool HasValue => true;

        /// <summary>The unmanaged storage handed to IUP, valid only during the call.</summary>
        internal IntPtr Buffer { get; private set; }

        protected Param(string title)
        {
            Title = title ?? "";
        }

        /// <summary>Allocates and initialises the unmanaged storage.</summary>
        internal virtual void Allocate() { }

        /// <summary>Reads the value back out of the unmanaged storage.</summary>
        internal virtual void ReadBack() { }

        /// <summary>Releases the unmanaged storage.</summary>
        internal virtual void Free()
        {
            if (Buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(Buffer);
                Buffer = IntPtr.Zero;
            }
        }

        protected void AllocBytes(int size)
        {
            Buffer = Marshal.AllocHGlobal(size);

            // Zero it: IUP reads the initial value from here, and a string buffer
            // must be null terminated.
            for (int i = 0; i < size; i++)
                Marshal.WriteByte(Buffer, i, 0);
        }

        /// <summary>
        /// Escapes a title for the format string. A literal '%' must be doubled, and a
        /// '\n' would terminate the parameter early.
        /// </summary>
        protected static string EscapeTitle(string title) =>
            (title ?? "").Replace("%", "%%").Replace("\n", " ");

        // ------------------------------------------------------------------
        // Factories
        // ------------------------------------------------------------------

        /// <summary>A True/False toggle. Maps to IUP's 'b' type.</summary>
        public static BoolParam Bool(string title, bool value = false,
                                     string falseText = null, string trueText = null) =>
            new BoolParam(title, value, falseText, trueText);

        /// <summary>An integer field with a spin. Maps to IUP's 'i' type.</summary>
        public static IntParam Int(string title, int value = 0,
                                   int? min = null, int? max = null, int? step = null) =>
            new IntParam(title, value, min, max, step);

        /// <summary>
        /// A real number field. Maps to IUP's 'R' type, which uses double in C. When
        /// both min and max are given a slider is added too.
        /// </summary>
        public static DoubleParam Real(string title, double value = 0.0,
                                       double? min = null, double? max = null, double? step = null) =>
            new DoubleParam(title, value, min, max, step);

        /// <summary>
        /// An angle in degrees, shown with a dial. Maps to IUP's 'A' type, which uses
        /// double in C.
        /// </summary>
        public static DoubleParam Angle(string title, double value = 0.0) =>
            new DoubleParam(title, value, null, null, null, angle: true);

        /// <summary>A single line text field. Maps to IUP's 's' type.</summary>
        public static StringParam String(string title, string value = "",
                                         int maxLength = 512, string mask = null) =>
            new StringParam(title, value, maxLength, 's', mask);

        /// <summary>A multiline text field. Maps to IUP's 'm' type.</summary>
        public static StringParam MultilineString(string title, string value = "",
                                                  int maxLength = 10240) =>
            new StringParam(title, value, maxLength, 'm', null);

        /// <summary>
        /// A text field with a browse button opening a file dialog. Maps to IUP's 'f'
        /// type.
        /// </summary>
        public static StringParam File(string title, string value = "",
                                       int maxLength = 4096, string filter = null,
                                       bool save = false, bool directory = false)
        {
            // The extra data is dialogtype|filter|directory|nochangedir|nooverwriteprompt
            string type = directory ? "DIR" : (save ? "SAVE" : "OPEN");
            string extra = $"{type}|{filter ?? ""}|||";
            return new StringParam(title, value, maxLength, 'f', null, extra);
        }

        /// <summary>
        /// A text field with a button opening a colour dialog. Maps to IUP's 'c' type.
        /// The value is an "R G B" or "R G B A" string.
        /// </summary>
        public static ColorParam Color(string title, Color value = default) =>
            new ColorParam(title, value);

        /// <summary>
        /// A text field with a button opening a font dialog. Maps to IUP's 'n' type.
        /// </summary>
        public static StringParam Font(string title, string value = "", int maxLength = 512) =>
            new StringParam(title, value, maxLength, 'n', null);

        /// <summary>
        /// A date field using a date picker. Maps to IUP's 'd' type. The value is a
        /// "yyyy/mm/dd" string.
        /// </summary>
        public static DateParam Date(string title, DateTime? value = null) =>
            new DateParam(title, value);

        /// <summary>
        /// A dropdown list. The value is the zero based index of the selected item.
        /// Maps to IUP's 'l' type.
        /// </summary>
        public static ListParam List(string title, int selected, params string[] items) =>
            new ListParam(title, selected, false, items);

        /// <summary>
        /// A group of radio toggles. The value is the zero based index of the selected
        /// item. Maps to IUP's 'o' type.
        /// </summary>
        public static ListParam Options(string title, int selected, params string[] items) =>
            new ListParam(title, selected, true, items);

        /// <summary>
        /// A horizontal line. Maps to IUP's 't' type, and consumes no value.
        /// </summary>
        public static SeparatorParam Separator(string title = "") =>
            new SeparatorParam(title);

        /// <summary>
        /// Overrides the default button titles, and optionally adds a third button.
        /// Maps to IUP's 'u' type, and consumes no value. Pass null for a button to
        /// keep its default name.
        /// </summary>
        public static ButtonsParam Buttons(string ok = null, string cancel = null, string help = null) =>
            new ButtonsParam(ok, cancel, help);
    }


    /// <summary>A boolean parameter, shown as a toggle.</summary>
    public sealed class BoolParam : Param
    {
        private readonly string _falseText;
        private readonly string _trueText;

        /// <summary>The value, updated when the dialog is confirmed.</summary>
        public bool Value { get; set; }

        internal BoolParam(string title, bool value, string falseText, string trueText)
            : base(title)
        {
            Value = value;
            _falseText = falseText;
            _trueText = trueText;
        }

        internal override string Format
        {
            get
            {
                string extra = (_falseText != null && _trueText != null)
                    ? $"[{_falseText},{_trueText}]"
                    : "";
                return $"{EscapeTitle(Title)}%b{extra}";
            }
        }

        internal override void Allocate()
        {
            AllocBytes(sizeof(int));
            Marshal.WriteInt32(Buffer, Value ? 1 : 0);
        }

        internal override void ReadBack() => Value = Marshal.ReadInt32(Buffer) != 0;
    }


    /// <summary>An integer parameter.</summary>
    public sealed class IntParam : Param
    {
        private readonly int? _min, _max, _step;

        /// <summary>The value, updated when the dialog is confirmed.</summary>
        public int Value { get; set; }

        internal IntParam(string title, int value, int? min, int? max, int? step)
            : base(title)
        {
            Value = value;
            _min = min;
            _max = max;
            _step = step;
        }

        internal override string Format
        {
            get
            {
                string extra = "";
                if (_min.HasValue)
                {
                    var sb = new StringBuilder("[");
                    sb.Append(_min.Value.ToString(CultureInfo.InvariantCulture));
                    if (_max.HasValue)
                    {
                        sb.Append(',').Append(_max.Value.ToString(CultureInfo.InvariantCulture));
                        if (_step.HasValue)
                            sb.Append(',').Append(_step.Value.ToString(CultureInfo.InvariantCulture));
                    }
                    sb.Append(']');
                    extra = sb.ToString();
                }

                return $"{EscapeTitle(Title)}%i{extra}";
            }
        }

        internal override void Allocate()
        {
            AllocBytes(sizeof(int));
            Marshal.WriteInt32(Buffer, Value);
        }

        internal override void ReadBack() => Value = Marshal.ReadInt32(Buffer);
    }


    /// <summary>A real number parameter, optionally shown as an angle.</summary>
    public sealed class DoubleParam : Param
    {
        private readonly double? _min, _max, _step;
        private readonly bool _angle;

        /// <summary>The value, updated when the dialog is confirmed.</summary>
        public double Value { get; set; }

        internal DoubleParam(string title, double value,
                             double? min, double? max, double? step, bool angle = false)
            : base(title)
        {
            Value = value;
            _min = min;
            _max = max;
            _step = step;
            _angle = angle;
        }

        internal override string Format
        {
            get
            {
                string extra = "";
                if (_min.HasValue)
                {
                    var sb = new StringBuilder("[");
                    sb.Append(_min.Value.ToString("R", CultureInfo.InvariantCulture));
                    if (_max.HasValue)
                    {
                        sb.Append(',').Append(_max.Value.ToString("R", CultureInfo.InvariantCulture));
                        if (_step.HasValue)
                            sb.Append(',').Append(_step.Value.ToString("R", CultureInfo.InvariantCulture));
                    }
                    sb.Append(']');
                    extra = sb.ToString();
                }

                // 'A' and 'R' are the double variants of 'a' and 'r'.
                return $"{EscapeTitle(Title)}%{(_angle ? 'A' : 'R')}{extra}";
            }
        }

        internal override void Allocate()
        {
            AllocBytes(sizeof(double));
            Marshal.Copy(new[] { Value }, 0, Buffer, 1);
        }

        internal override void ReadBack()
        {
            double[] tmp = new double[1];
            Marshal.Copy(Buffer, tmp, 0, 1);
            Value = tmp[0];
        }
    }


    /// <summary>A string parameter, in one of several presentations.</summary>
    public class StringParam : Param
    {
        private readonly int _maxLength;
        private readonly char _type;
        private readonly string _mask;
        private readonly string _extra;

        /// <summary>The value, updated when the dialog is confirmed.</summary>
        public string Value { get; set; }

        internal StringParam(string title, string value, int maxLength,
                             char type, string mask, string extra = null)
            : base(title)
        {
            if (maxLength < 1)
                throw new ArgumentOutOfRangeException(nameof(maxLength));

            Value = value ?? "";
            _maxLength = maxLength;
            _type = type;
            _mask = mask;
            _extra = extra;
        }

        internal override string Format
        {
            get
            {
                // A mask uses no brackets, to leave [ and ] available inside it.
                string extra = _extra != null ? $"[{_extra}]" : (_mask ?? "");
                return $"{EscapeTitle(Title)}%{_type}{extra}";
            }
        }

        internal override void Allocate()
        {
            // IUP writes the edited text straight into this buffer, so it must be
            // large enough for whatever the user types. MAXSTR caps it.
            byte[] utf8 = Encoding.UTF8.GetBytes(Value);
            int size = Math.Max(_maxLength, utf8.Length + 1) + 1;

            AllocBytes(size);
            Marshal.Copy(utf8, 0, Buffer, Math.Min(utf8.Length, size - 1));
        }

        internal override void ReadBack() => Value = Marshal.PtrToStringUTF8(Buffer) ?? "";
    }


    /// <summary>A colour parameter, shown with a colour picker button.</summary>
    public sealed class ColorParam : Param
    {
        private const int BufferSize = 64;

        /// <summary>The value, updated when the dialog is confirmed.</summary>
        public Color Value { get; set; }

        internal ColorParam(string title, Color value) : base(title)
        {
            Value = value;
        }

        internal override string Format => $"{EscapeTitle(Title)}%c";

        internal override void Allocate()
        {
            AllocBytes(BufferSize);

            string text = Value.IsEmpty ? "0 0 0" : Utils.FormatColor(Value);
            byte[] utf8 = Encoding.UTF8.GetBytes(text);
            Marshal.Copy(utf8, 0, Buffer, Math.Min(utf8.Length, BufferSize - 1));
        }

        internal override void ReadBack()
        {
            string text = Marshal.PtrToStringUTF8(Buffer);
            Value = Utils.ParseColor(text);
        }
    }


    /// <summary>A date parameter, shown with a date picker.</summary>
    public sealed class DateParam : Param
    {
        private const int BufferSize = 64;

        /// <summary>The value, updated when the dialog is confirmed.</summary>
        public DateTime? Value { get; set; }

        internal DateParam(string title, DateTime? value) : base(title)
        {
            Value = value;
        }

        internal override string Format => $"{EscapeTitle(Title)}%d";

        internal override void Allocate()
        {
            AllocBytes(BufferSize);

            string text = (Value ?? DateTime.Today).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            byte[] utf8 = Encoding.UTF8.GetBytes(text);
            Marshal.Copy(utf8, 0, Buffer, Math.Min(utf8.Length, BufferSize - 1));
        }

        internal override void ReadBack()
        {
            string text = Marshal.PtrToStringUTF8(Buffer);

            if (DateTime.TryParseExact(text, new[] { "yyyy/M/d", "yyyy/MM/dd" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                Value = d;
            else
                Value = null;
        }
    }


    /// <summary>A list parameter, as a dropdown or a group of radio toggles.</summary>
    public sealed class ListParam : Param
    {
        private readonly string[] _items;
        private readonly bool _radio;

        /// <summary>
        /// The zero based index of the selected item, updated when the dialog is
        /// confirmed.
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>The selected item's text, or null if the index is out of range.</summary>
        public string SelectedItem =>
            SelectedIndex >= 0 && SelectedIndex < _items.Length ? _items[SelectedIndex] : null;

        internal ListParam(string title, int selected, bool radio, string[] items)
            : base(title)
        {
            if (items == null || items.Length == 0)
                throw new ArgumentException("A list parameter needs at least one item.", nameof(items));

            foreach (string item in items)
            {
                if (item != null && item.Contains('|'))
                    throw new ArgumentException(
                        "List items cannot contain '|', which separates them in IUP's format string.",
                        nameof(items));
            }

            _items = items;
            _radio = radio;
            SelectedIndex = selected;
        }

        internal override string Format =>
            $"{EscapeTitle(Title)}%{(_radio ? 'o' : 'l')}|{string.Join("|", _items)}|";

        internal override void Allocate()
        {
            AllocBytes(sizeof(int));
            Marshal.WriteInt32(Buffer, SelectedIndex);
        }

        internal override void ReadBack() => SelectedIndex = Marshal.ReadInt32(Buffer);
    }


    /// <summary>A horizontal separator. Consumes no value.</summary>
    public sealed class SeparatorParam : Param
    {
        internal SeparatorParam(string title) : base(title)
        {
        }

        internal override string Format => $"{EscapeTitle(Title)}%t";

        internal override bool HasValue => false;
    }


    /// <summary>
    /// Overrides the dialog's button titles. Consumes no value.
    /// </summary>
    public sealed class ButtonsParam : Param
    {
        private readonly string _ok, _cancel, _help;

        internal ButtonsParam(string ok, string cancel, string help) : base("")
        {
            _ok = ok;
            _cancel = cancel;
            _help = help;
        }

        internal override string Format => $"%u[{_ok ?? ""},{_cancel ?? ""},{_help ?? ""}]";

        internal override bool HasValue => false;
    }


    /// <summary>
    /// Which button or situation triggered a GetParams callback.
    /// </summary>
    public enum ParamAction
    {
        /// <summary>A parameter's value was changed. Index holds which one.</summary>
        ValueChanged = 0,
        /// <summary>The OK button was pressed.</summary>
        Ok = IupNative.IUP_GETPARAM_BUTTON1,
        /// <summary>The dialog was mapped and is about to be shown.</summary>
        Init = IupNative.IUP_GETPARAM_INIT,
        /// <summary>The Cancel button was pressed.</summary>
        Cancel = IupNative.IUP_GETPARAM_BUTTON2,
        /// <summary>The third button, normally Help, was pressed.</summary>
        Help = IupNative.IUP_GETPARAM_BUTTON3,
        /// <summary>The dialog's close button was clicked.</summary>
        Close = IupNative.IUP_GETPARAM_CLOSE,
        /// <summary>The dialog is about to be mapped.</summary>
        Map = IupNative.IUP_GETPARAM_MAP
    }


    /// <summary>Data passed to a GetParams callback.</summary>
    public class ParamCallbackData
    {
        /// <summary>What triggered the callback.</summary>
        public readonly ParamAction Action;

        /// <summary>
        /// The zero based index of the parameter being changed, when Action is
        /// ValueChanged. Otherwise -1. Note this counts only parameters that carry a
        /// value, so separators and button titles are skipped, matching IUP.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// The parameter being changed, when Action is ValueChanged. Otherwise null.
        /// Its Value has NOT been updated yet - IUP is asking whether to accept the
        /// change.
        /// </summary>
        public readonly Param Parameter;

        /// <summary>
        /// Set to false to reject the change or the button action. Default: true.
        /// </summary>
        public bool Accept = true;

        public ParamCallbackData(ParamAction action, int index, Param parameter)
        {
            this.Action = action;
            this.Index = index;
            this.Parameter = parameter;
        }
    }

    public delegate void ParamCallback(ParamCallbackData d);
}