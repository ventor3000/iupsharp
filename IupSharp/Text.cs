using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates an editable text field, either single line or multiple lines.
    /// </summary>
    /// <remarks>
    /// <para>Multiline is a creation-only setting, so use the constructor overload
    /// rather than trying to change it later.</para>
    ///
    /// <para>All character positions and counts are in CHARACTERS, not bytes. With
    /// UTF-8 this matters: a character may occupy several bytes, so indices from this
    /// control do not line up with byte offsets. They do line up with C# string
    /// indices for text in the Basic Multilingual Plane, but not for characters that
    /// need a surrogate pair.</para>
    ///
    /// <para>When Multiline is true the Enter key inserts a new line and Tab inserts a
    /// tab, so a dialog's DefaultEnter button will not fire while this control has the
    /// focus, and Ctrl+Tab is needed to move focus onward.</para>
    /// </remarks>
    public class Text : Control
    {
        /// <summary>
        /// Creates a new single line text field.
        /// </summary>
        public Text() : base(NativeIup.Text(null))
        {
        }

        /// <summary>
        /// Creates a new text field.
        /// </summary>
        /// <param name="multiline">
        /// True for a multiple line field. This cannot be changed afterwards. Setting
        /// it also turns the scrollbars on.
        /// </param>
        public Text(bool multiline) : this()
        {
            if (multiline)
                SetAttribute("MULTILINE", "YES");
        }

        /// <summary>
        /// Creates a new text field with an initial value.
        /// </summary>
        /// <param name="value">The initial text.</param>
        /// <param name="multiline">True for a multiple line field.</param>
        public Text(string value, bool multiline = false) : this(multiline)
        {
            Value = value;
        }

        #region CONTENT

        /// <summary>
        /// Gets or sets the text. The '\n' character starts a new line, which is only
        /// valid when Multiline is true. Once the element is mapped this returns the
        /// empty string rather than null when there is no text.
        /// (non inheritable)
        /// </summary>
        public string Value
        {
            get => GetAttribute("VALUE") ?? "";
            set => SetAttribute("VALUE", value);
        }

        /// <summary>
        /// Sets the text, but only if it passes the Mask. Does nothing if it fails.
        /// (write only) (since 3.4)
        /// </summary>
        public string ValueMasked
        {
            set => SetAttribute("VALUEMASKED", value);
        }

        /// <summary>
        /// Gets the number of characters in the text, including line breaks.
        /// (read only) (since 3.5)
        /// </summary>
        public int Count => GetInt("COUNT", 0);

        /// <summary>
        /// Gets the number of lines in the text. Always 1 when Multiline is false.
        /// (read only) (since 3.5)
        /// </summary>
        public int LineCount => GetInt("LINECOUNT", 1);

        /// <summary>
        /// Gets the text of the line the caret is on, without the trailing '\n'.
        /// Returns the same as Value when Multiline is false.
        /// (read only) (since 3.5)
        /// </summary>
        public string LineValue => GetAttribute("LINEVALUE") ?? "";

        /// <summary>
        /// Appends text at the end. In a multiline field a '\n' is inserted first when
        /// AppendNewline is true and the field is not empty. Ignored before the element
        /// is mapped.
        /// </summary>
        public void Append(string text) => SetAttribute("APPEND", text);

        /// <summary>
        /// Inserts text at the caret, replacing the selection if there is one. Ignored
        /// before the element is mapped.
        /// </summary>
        public void Insert(string text) => SetAttribute("INSERT", text);

        /// <summary>
        /// Gets or sets whether Append inserts a '\n' before the appended text in a
        /// multiline field that is not empty. Default: true.
        /// </summary>
        public virtual bool AppendNewline
        {
            get => GetAttribute("APPENDNEWLINE") != "NO";
            set => SetAttribute("APPENDNEWLINE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Changes the case of the whole text. Supports Latin-1 encoding only, even
        /// when using UTF-8, and does not depend on the current locale.
        /// (write only) (since 3.28)
        /// </summary>
        public void ChangeCase(TextCase value) =>
            SetAttribute("CHANGECASE", Utils.MapEnum(value, _textCases));

        static readonly (string, TextCase)[] _textCases = new[]
        {
            ("UPPER", TextCase.Upper),
            ("LOWER", TextCase.Lower),
            ("TOGGLE", TextCase.Toggle),
            ("TITLE", TextCase.Title)
        };

        #endregion

        #region CARET AND SELECTION

        /// <summary>
        /// Gets or sets the caret position as a zero based character index into Value.
        /// This is usually easier to work with than Caret.
        /// (non inheritable) (since 3.0)
        /// </summary>
        public int CaretPos
        {
            get => GetInt("CARETPOS", 0);
            set => SetAttribute("CARETPOS", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the caret position as a one based line and column. For a single
        /// line field the line is always 1.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// In Windows, when the element does not have the focus the value returned is
        /// the position of the first character of the current selection.
        /// </remarks>
        public (int Line, int Column) Caret
        {
            get
            {
                string v = GetAttribute("CARET");
                if (string.IsNullOrWhiteSpace(v))
                    return (1, 1);

                string[] p = v.Split(',');
                if (p.Length == 1)
                    return (1, ParseInt(p[0], 1));

                return (ParseInt(p[0], 1), ParseInt(p[1], 1));
            }
            set => SetAttribute("CARET", Multiline
                ? $"{value.Line},{value.Column}"
                : value.Column.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the selection as a zero based character range, where End is the
        /// position after the last selected character. Returns null when there is no
        /// selection; assigning null does nothing - use SelectNone to clear it.
        /// (non inheritable) (since 3.0)
        /// </summary>
        public (int Start, int End)? SelectionPos
        {
            get
            {
                string v = GetAttribute("SELECTIONPOS");
                if (string.IsNullOrWhiteSpace(v))
                    return null;

                string[] p = v.Split(':');
                if (p.Length != 2)
                    return null;

                return (ParseInt(p[0], 0), ParseInt(p[1], 0));
            }
            set
            {
                if (value == null)
                    return;

                SetAttribute("SELECTIONPOS", $"{value.Value.Start}:{value.Value.End}");
            }
        }

        /// <summary>
        /// Gets or sets the selected text. Returns null when there is no selection.
        /// Assigning replaces the current selection, and does nothing when there is no
        /// selection - use Insert to add text at the caret instead.
        /// (non inheritable)
        /// </summary>
        public string SelectedText
        {
            get => GetAttribute("SELECTEDTEXT");
            set => SetAttribute("SELECTEDTEXT", value);
        }

        /// <summary>Selects the whole text.</summary>
        public void SelectAll() => SetAttribute("SELECTION", "ALL");

        /// <summary>Clears the selection.</summary>
        public void SelectNone() => SetAttribute("SELECTION", "NONE");

        /// <summary>
        /// Scrolls the text to make the given zero based character position visible.
        /// (since 3.0)
        /// </summary>
        public void ScrollTo(int pos) =>
            SetAttribute("SCROLLTOPOS", pos.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Converts a one based line and column into a zero based character position.
        /// For a single line field the result is always column - 1.
        /// (since 3.0)
        /// </summary>
        public int LineColToPos(int line, int col)
        {
            CheckAlive();
            NativeIup.IupTextConvertLinColToPos(Handle, line, col, out int pos);
            return pos;
        }

        /// <summary>
        /// Converts a zero based character position into a one based line and column.
        /// For a single line field the line is always 1 and the column is pos + 1.
        /// (since 3.0)
        /// </summary>
        public (int Line, int Column) PosToLineCol(int pos)
        {
            CheckAlive();
            NativeIup.IupTextConvertPosToLinCol(Handle, pos, out int lin, out int col);
            return (lin, col);
        }

        #endregion

        #region CLIPBOARD

        /// <summary>Copies the selection to the clipboard. (since 3.0)</summary>
        public void Copy() => SetAttribute("CLIPBOARD", "COPY");

        /// <summary>Cuts the selection to the clipboard. (since 3.0)</summary>
        public void Cut() => SetAttribute("CLIPBOARD", "CUT");

        /// <summary>Pastes the clipboard at the caret. (since 3.0)</summary>
        public void Paste() => SetAttribute("CLIPBOARD", "PASTE");

        /// <summary>Clears the selection. (since 3.0)</summary>
        public void Clear() => SetAttribute("CLIPBOARD", "CLEAR");

        /// <summary>
        /// Undoes the last edit.
        /// [Windows only]
        /// </summary>
        public void Undo() => SetAttribute("CLIPBOARD", "UNDO");

        /// <summary>
        /// Redoes the last undone edit. Requires Formatting to be true.
        /// [Windows only]
        /// </summary>
        public void Redo() => SetAttribute("CLIPBOARD", "REDO");

        #endregion

        #region BEHAVIOUR

        /// <summary>
        /// Gets whether this is a multiple line field. Creation only - use the
        /// constructor overload to set it.
        /// (non inheritable)
        /// </summary>
        public bool Multiline => GetAttribute("MULTILINE") == "YES";

        /// <summary>
        /// Gets or sets whether the user can only read the contents. This restricts
        /// keyboard input only: the text can still be changed through Value, and the
        /// navigation keys still work. Default: false.
        /// </summary>
        public virtual bool ReadOnly
        {
            get => GetAttribute("READONLY") == "YES";
            set => SetAttribute("READONLY", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the maximum number of characters accepted from the keyboard.
        /// Larger text can still be set through Value. Zero means no limit, which is
        /// the default.
        /// </summary>
        public int MaxLength
        {
            get => GetInt("NC", 0);
            set => SetAttribute("NC", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets a mask that filters interactive text input. See the IUP mask
        /// documentation for the pattern syntax.
        /// (non inheritable)
        /// </summary>
        public virtual string Mask
        {
            get => GetAttribute("MASK");
            set => SetAttribute("MASK", value);
        }

        /// <summary>
        /// Gets or sets whether typed characters are hidden behind asterisks. Creation
        /// only. Default: false.
        /// [Windows and GTK only] (non inheritable)
        /// </summary>
        public virtual bool Password
        {
            get => GetAttribute("PASSWORD") == "YES";
            set => SetAttribute("PASSWORD", value ? "YES" : "NO");
        }

        static readonly (string, TextFilter)[] _filters = new[]
        {
            ("", TextFilter.None),
            ("LOWERCASE", TextFilter.Lowercase),
            ("UPPERCASE", TextFilter.Uppercase),
            ("NUMBER", TextFilter.Number)
        };

        /// <summary>
        /// Gets or sets a filter applied to typed characters.
        /// [Windows only] (non inheritable) (since 3.0)
        /// </summary>
        public virtual TextFilter Filter
        {
            get => Utils.MapAttrib(GetAttribute("FILTER"), _filters);
            set => SetAttribute("FILTER", value == TextFilter.None
                ? null
                : Utils.MapEnum(value, _filters));
        }

        /// <summary>
        /// Gets or sets whether text formatting attributes can be used. Always enabled
        /// in GTK when Multiline is true. Default: false.
        /// [Windows and GTK only] (non inheritable) (since 3.0)
        /// </summary>
        public virtual bool Formatting
        {
            get => GetAttribute("FORMATTING") == "YES";
            set => SetAttribute("FORMATTING", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets overwrite mode. Only works when Formatting is true.
        /// [Windows and GTK only] (non inheritable) (since 3.0)
        /// </summary>
        public virtual bool Overwrite
        {
            get => GetAttribute("OVERWRITE") == "YES";
            set => SetAttribute("OVERWRITE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether dropping files onto the control is enabled. Default:
        /// false, but defining DropFilesCB before the element is mapped enables it
        /// automatically.
        /// [Windows and GTK only] (non inheritable) (since 3.0)
        /// </summary>
        public virtual bool DropFilesTarget
        {
            get => GetAttribute("DROPFILESTARGET") == "YES";
            set => SetAttribute("DROPFILESTARGET", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether focus traversal is enabled. Creation only. In Windows
        /// the control still gets the focus when clicked. Default: true.
        /// (non inheritable) (since 3.0)
        /// </summary>
        public virtual bool CanFocus
        {
            get => GetAttribute("CANFOCUS") != "NO";
            set => SetAttribute("CANFOCUS", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the focus callback is forwarded to the next native
        /// parent with FocusCB defined. Default: false.
        /// (non inheritable) (since 3.23)
        /// </summary>
        public virtual bool PropagateFocus
        {
            get => GetAttribute("PROPAGATEFOCUS") == "YES";
            set => SetAttribute("PROPAGATEFOCUS", value ? "YES" : "NO");
        }

        #endregion

        #region APPEARANCE

        /// <summary>
        /// Gets or sets the background color of the text. Default: the global attribute
        /// TxtBgColor. Ignored in GTK when Multiline is false.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; }

        /// <summary>
        /// Gets or sets the text color. Default: the global attribute TxtFgColor.
        /// </summary>
        public override Color FgColor { get => base.FgColor; set => base.FgColor = value; }

        static readonly (string, TextAlignment)[] _alignments = new[]
        {
            ("ALEFT", TextAlignment.Left),
            ("ACENTER", TextAlignment.Center),
            ("ARIGHT", TextAlignment.Right)
        };

        /// <summary>
        /// Gets or sets the horizontal text alignment. Default: Left. In Motif text is
        /// always left aligned.
        /// [Windows and GTK only] (non inheritable)
        /// </summary>
        public virtual TextAlignment Alignment
        {
            get => Utils.MapAttrib(GetAttribute("ALIGNMENT"), _alignments);
            set => SetAttribute("ALIGNMENT", Utils.MapEnum(value, _alignments));
        }

        /// <summary>
        /// Gets or sets whether a border is shown. Creation only. Default: true.
        /// </summary>
        public virtual bool Border
        {
            get => GetAttribute("BORDER") != "NO";
            set => SetAttribute("BORDER", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets a prompt shown while the control is empty, as a hint to the
        /// user. Valid only when Multiline is false, and requires Visual Styles on
        /// Windows.
        /// [Windows and GTK only] (non inheritable) (since 3.0)
        /// </summary>
        public virtual string CueBanner
        {
            get => GetAttribute("CUEBANNER");
            set => SetAttribute("CUEBANNER", value);
        }

        /// <summary>
        /// Gets or sets the internal margin in pixels. Works like the Margin attribute
        /// of HBox and VBox but is named differently to avoid inheritance problems.
        /// Default: (0,0). In Windows only the horizontal value is used.
        /// (since 3.0)
        /// </summary>
        public (int, int) Padding
        {
            get { CheckAlive(); NativeIup.GetIntInt(Handle, "PADDING", out int x, out int y); return (x, y); }
            set => SetAttribute("PADDING", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets the number of visible columns used for the natural size, which
        /// also acts as a minimum. Uses a wider character than Size does, so strings fit
        /// better. Default: 5.
        /// (since 3.0)
        /// </summary>
        public int VisibleColumns
        {
            get => GetInt("VISIBLECOLUMNS", 5);
            set => SetAttribute("VISIBLECOLUMNS", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the number of visible lines used for the natural size when
        /// Multiline is true, which also acts as a minimum. Default: 1.
        /// (since 3.0)
        /// </summary>
        public int VisibleLines
        {
            get => GetInt("VISIBLELINES", 1);
            set => SetAttribute("VISIBLELINES", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the number of characters in a tab stop. Valid only when
        /// Multiline is true. Default: 8.
        /// [Windows and GTK only]
        /// </summary>
        public int TabSize
        {
            get => GetInt("TABSIZE", 8);
            set => SetAttribute("TABSIZE", value.ToString(CultureInfo.InvariantCulture));
        }

        static readonly (string, ScrollBars)[] _scrollBars = new[]
        {
            ("YES", ScrollBars.Both),
            ("NO", ScrollBars.No),
            ("HORIZONTAL", ScrollBars.Horizontal),
            ("VERTICAL", ScrollBars.Vertical)
        };

        /// <summary>
        /// Gets or sets which scrollbars are attached. Creation only, and valid only
        /// when Multiline is true. Default: Both.
        /// </summary>
        public virtual ScrollBars ScrollBar
        {
            get => Utils.MapAttrib(GetAttribute("SCROLLBAR"), _scrollBars);
            set => SetAttribute("SCROLLBAR", Utils.MapEnum(value, _scrollBars));
        }

        /// <summary>
        /// Gets or sets whether scrollbars are shown only when needed. Default: false.
        /// Not supported in Motif, nor in Windows when Formatting is false.
        /// </summary>
        public virtual bool AutoHide
        {
            get => GetAttribute("AUTOHIDE") == "YES";
            set => SetAttribute("AUTOHIDE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether long lines wrap instead of scrolling horizontally.
        /// Creation only, and valid only when Multiline is true. Default: false.
        /// </summary>
        public virtual bool WordWrap
        {
            get => GetAttribute("WORDWRAP") == "YES";
            set => SetAttribute("WORDWRAP", value ? "YES" : "NO");
        }

        #endregion

        #region SPIN

        /// <summary>
        /// Gets or sets whether a spin control is attached. Creation only.
        /// Default: false. The spin increments an integer; editing remains available.
        /// (non inheritable) (since 3.0)
        /// </summary>
        public virtual bool Spin
        {
            get => GetAttribute("SPIN") == "YES";
            set => SetAttribute("SPIN", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the current spin value, limited to SpinMin and SpinMax.
        /// (non inheritable)
        /// </summary>
        public int SpinValue
        {
            get => GetInt("SPINVALUE", 0);
            set => SetAttribute("SPINVALUE", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>Gets or sets the spin minimum. Default: 0. (non inheritable)</summary>
        public int SpinMin
        {
            get => GetInt("SPINMIN", 0);
            set => SetAttribute("SPINMIN", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>Gets or sets the spin maximum. Default: 100. (non inheritable)</summary>
        public int SpinMax
        {
            get => GetInt("SPINMAX", 100);
            set => SetAttribute("SPINMAX", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>Gets or sets the spin increment. Default: 1. (non inheritable)</summary>
        public int SpinInc
        {
            get => GetInt("SPININC", 1);
            set => SetAttribute("SPININC", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets whether the spin sits on the left or the right. Creation only.
        /// Default: Right, and always Right in GTK.
        /// </summary>
        public virtual bool SpinOnLeft
        {
            get => GetAttribute("SPINALIGN") == "LEFT";
            set => SetAttribute("SPINALIGN", value ? "LEFT" : "RIGHT");
        }

        /// <summary>
        /// Gets or sets whether the spin wraps around at its limits. Creation only.
        /// Default: false.
        /// </summary>
        public virtual bool SpinWrap
        {
            get => GetAttribute("SPINWRAP") == "YES";
            set => SetAttribute("SPINWRAP", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the text is updated automatically by the spin.
        /// Creation only. Default: true. Set it to false and update Value from SpinCB
        /// to control the text yourself.
        /// </summary>
        public virtual bool SpinAuto
        {
            get => GetAttribute("SPINAUTO") != "NO";
            set => SetAttribute("SPINAUTO", value ? "YES" : "NO");
        }

        #endregion

        #region HELPERS

        private int GetInt(string name, int fallback)
        {
            string v = GetAttribute(name);
            return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)
                ? i
                : fallback;
        }

        private static int ParseInt(string s, int fallback) =>
            int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i) ? i : fallback;

        #endregion

        #region CALLBACKS

        private TextActionCallback _action;
        private IFnis _actionInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the text is edited, before the value
        /// is actually changed. Fires for keyboard input, undo, and the clipboard.
        /// </summary>
        /// <remarks>
        /// Set the callback data's Result to Ignore to reject the change, or assign
        /// Replacement to substitute a different character. Value can only be changed
        /// from this callback when the result is Ignore.
        /// </remarks>
        public TextActionCallback Action
        {
            get => _action;
            set
            {
                _action = value;
                _actionInternal = ActionInternal;
                SetCallback("ACTION", Utils.CastCallback<Icallback>(_actionInternal));
            }
        }
        private int ActionInternal(nint ih, int c, string newValue)
        {
            try
            {
                var cb = new TextActionData(this, c, newValue);
                _action?.Invoke(cb);

                // A valid replacement character overrides the normal result.
                if (cb.Replacement != '\0')
                    return cb.Replacement;

                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in Text Action callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _valueChangedCB;
        private IFn _valueChangedCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called after the value was interactively changed by
        /// the user. Unlike Action this fires after the change, so Value is current.
        /// (since 3.0)
        /// </summary>
        public Callback ValueChangedCB
        {
            get => _valueChangedCB;
            set
            {
                _valueChangedCB = value;
                _valueChangedCBInternal = ValueChangedCBInternal;
                SetCallback("VALUECHANGED_CB", Utils.CastCallback<Icallback>(_valueChangedCBInternal));
            }
        }
        private int ValueChangedCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _valueChangedCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ValueChangedCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private CaretCBCallback _caretCB;
        private IFniii _caretCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the caret position changes.
        /// </summary>
        public CaretCBCallback CaretCB
        {
            get => _caretCB;
            set
            {
                _caretCB = value;
                _caretCBInternal = CaretCBInternal;
                SetCallback("CARET_CB", Utils.CastCallback<Icallback>(_caretCBInternal));
            }
        }
        private int CaretCBInternal(nint ih, int lin, int col, int pos)
        {
            try
            {
                var cb = new CaretCBData(this, lin, col, pos);
                _caretCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in CaretCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private SpinCBCallback _spinCB;
        private IFni _spinCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when a spin button is pressed. Valid only
        /// when Spin is true. The Action callback is not called while this one is.
        /// Value can only be changed from here when SpinAuto is false.
        /// (since 3.0)
        /// </summary>
        public SpinCBCallback SpinCB
        {
            get => _spinCB;
            set
            {
                _spinCB = value;
                _spinCBInternal = SpinCBInternal;
                SetCallback("SPIN_CB", Utils.CastCallback<Icallback>(_spinCBInternal));
            }
        }
        private int SpinCBInternal(nint ih, int pos)
        {
            try
            {
                var cb = new SpinCBData(this, pos);
                _spinCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in SpinCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private ButtonCBCallback _buttonCB;
        private IFniiiis _buttonCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when any mouse button is pressed or
        /// released. Use ConvertXYToPos to turn the coordinates into a character
        /// position.
        /// (since 3.0)
        /// </summary>
        public ButtonCBCallback ButtonCB
        {
            get => _buttonCB;
            set
            {
                _buttonCB = value;
                _buttonCBInternal = ButtonCBInternal;
                SetCallback("BUTTON_CB", Utils.CastCallback<Icallback>(_buttonCBInternal));
            }
        }
        private int ButtonCBInternal(nint ih, int but, int pressed, int x, int y, string status)
        {
            try
            {
                var cb = new ButtonCBData(this, but, pressed, x, y, status);
                _buttonCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ButtonCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private MotionCBCallback _motionCB;
        private IFniis _motionCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse is moved. Use
        /// ConvertXYToPos to turn the coordinates into a character position.
        /// (since 3.0)
        /// </summary>
        public MotionCBCallback MotionCB
        {
            get => _motionCB;
            set
            {
                _motionCB = value;
                _motionCBInternal = MotionCBInternal;
                SetCallback("MOTION_CB", Utils.CastCallback<Icallback>(_motionCBInternal));
            }
        }
        private int MotionCBInternal(nint ih, int x, int y, string status)
        {
            try
            {
                var cb = new MotionCBData(this, x, y, status);
                _motionCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in MotionCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private DropFilesCBCallback _dropFilesCB;
        private IFnsiii _dropFilesCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when one or more files are dropped on the
        /// control. Defining this before the element is mapped enables DropFilesTarget
        /// automatically.
        /// [Windows and GTK only] (since 3.0)
        /// </summary>
        public DropFilesCBCallback DropFilesCB
        {
            get => _dropFilesCB;
            set
            {
                _dropFilesCB = value;
                _dropFilesCBInternal = DropFilesCBInternal;
                SetCallback("DROPFILES_CB", Utils.CastCallback<Icallback>(_dropFilesCBInternal));
            }
        }
        private int DropFilesCBInternal(nint ih, string filename, int num, int x, int y)
        {
            try
            {
                var cb = new DropFilesCBData(this, filename, num, x, y);
                _dropFilesCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in DropFilesCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}