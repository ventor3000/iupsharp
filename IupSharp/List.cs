using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates a list of items. It can be a plain list, a dropdown (combo box), a
    /// list with an edit box, or a dropdown with an edit box - four controls in one,
    /// chosen at construction.
    /// </summary>
    /// <remarks>
    /// <para>DropDown, EditBox, Multiple, Sort, ShowImage and ShowDragDrop are all
    /// creation-only, so they are constructor arguments rather than settable
    /// properties.</para>
    ///
    /// <para>Item positions start at 1 throughout, matching IUP. Use the Items
    /// collection rather than the raw numbered attributes.</para>
    ///
    /// <para>The edit box members - Caret, Selection, Mask, ReadOnly and so on - only
    /// have an effect when EditBox is true. They are exposed unconditionally because
    /// IUP does the same, but they silently do nothing otherwise.</para>
    /// </remarks>
    public class List : Control
    {
        /// <summary>
        /// Creates a new list with the given style and items.
        /// </summary>
        /// <param name="style">
        /// The creation-only options. These cannot be changed afterwards, which is why
        /// they are given here rather than as properties.
        /// </param>
        /// <param name="items">Initial items, in order.</param>
        /// <exception cref="ArgumentException">The style combination is not valid.</exception>
        public List(ListStyle style, params string[] items) : base(IupNative.List(null))
        {
            Validate(style);

            // All creation-only: they must be set before the element is mapped.
            if (style.HasFlag(ListStyle.DropDown)) SetAttribute("DROPDOWN", "YES");
            if (style.HasFlag(ListStyle.EditBox)) SetAttribute("EDITBOX", "YES");
            if (style.HasFlag(ListStyle.Multiple)) SetAttribute("MULTIPLE", "YES");
            if (style.HasFlag(ListStyle.Sort)) SetAttribute("SORT", "YES");
            if (style.HasFlag(ListStyle.ShowImage)) SetAttribute("SHOWIMAGE", "YES");
            if (style.HasFlag(ListStyle.ShowDragDrop)) SetAttribute("SHOWDRAGDROP", "YES");
            if (style.HasFlag(ListStyle.NoScrollBar)) SetAttribute("SCROLLBAR", "NO");
            if (style.HasFlag(ListStyle.NoFocus)) SetAttribute("CANFOCUS", "NO");

            _items = new ItemCollection(this);

            if (items == null)
                return;

            foreach (string item in items)
                Items.Add(item);
        }

        /// <summary>
        /// Creates a new plain list containing the given items.
        /// </summary>
        public List(params string[] items) : this(ListStyle.None, items)
        {
        }

        /// <summary>
        /// Rejects style combinations that IUP silently ignores, so they fail at
        /// construction rather than producing a control that quietly behaves
        /// differently from what was asked for.
        /// </summary>
        private static void Validate(ListStyle style)
        {
            if (style.HasFlag(ListStyle.Multiple) &&
                (style.HasFlag(ListStyle.DropDown) || style.HasFlag(ListStyle.EditBox)))
                throw new ArgumentException(
                    "ListStyle.Multiple is only valid without DropDown and EditBox.", nameof(style));

            if (style.HasFlag(ListStyle.ShowDragDrop) &&
                (style.HasFlag(ListStyle.DropDown) || style.HasFlag(ListStyle.Multiple)))
                throw new ArgumentException(
                    "ListStyle.ShowDragDrop is only valid without DropDown and Multiple.", nameof(style));
        }

        #region ITEMS

        private readonly ItemCollection _items;

        /// <summary>
        /// Gets the list's items. Positions start at 1, matching IUP.
        /// </summary>
        public ItemCollection Items => _items;

        /// <summary>
        /// The items of a List, as a one-based collection over IUP's numbered
        /// attributes.
        /// </summary>
        public sealed class ItemCollection : IEnumerable<string>
        {
            private readonly List _owner;

            internal ItemCollection(List owner)
            {
                _owner = owner;
            }

            /// <summary>Gets the number of items.</summary>
            public int Count
            {
                get
                {
                    string v = _owner.GetAttribute("COUNT");
                    return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int c)
                        ? c
                        : 0;
                }
            }

            /// <summary>
            /// Gets or sets the item at the given one-based position. Setting replaces
            /// the item; the position must already exist.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException">The position is below 1.</exception>
            public string this[int position]
            {
                get
                {
                    CheckPosition(position);
                    return _owner.GetAttributeId("", position);
                }
                set
                {
                    CheckPosition(position);
                    _owner.SetAttributeId("", position, value);
                }
            }

            /// <summary>
            /// Appends an item at the end.
            /// </summary>
            /// <remarks>
            /// Before the element is mapped this writes the numbered attribute
            /// directly; afterwards it uses APPENDITEM, which IUP ignores before map.
            /// </remarks>
            public void Add(string text)
            {
                if (_owner.IsMapped)
                    _owner.SetAttribute("APPENDITEM", text);
                else
                    _owner.SetAttributeId("", Count + 1, text);
            }

            /// <summary>Appends several items at the end.</summary>
            public void AddRange(IEnumerable<string> items)
            {
                if (items == null)
                    return;

                foreach (string item in items)
                    Add(item);
            }

            /// <summary>
            /// Inserts an item before the given one-based position. A position equal to
            /// Count + 1 appends. Ignored by IUP before the element is mapped and when
            /// out of bounds.
            /// </summary>
            public void Insert(int position, string text)
            {
                CheckPosition(position);

                if (_owner.IsMapped)
                    _owner.SetAttributeId("INSERTITEM", position, text);
                else
                    _owner.SetAttributeId("", position, text);
            }

            /// <summary>
            /// Removes the item at the given one-based position. Ignored by IUP before
            /// the element is mapped.
            /// </summary>
            public void RemoveAt(int position)
            {
                CheckPosition(position);
                _owner.SetAttribute("REMOVEITEM", position.ToString(CultureInfo.InvariantCulture));
            }

            /// <summary>Removes every item.</summary>
            public void Clear()
            {
                if (_owner.IsMapped)
                    _owner.SetAttribute("REMOVEITEM", "ALL");
                else
                    _owner.SetAttributeId("", 1, null);   // NULL at 1 clears the list
            }

            /// <summary>
            /// Finds the one-based position of the first item with the given text, or
            /// -1 if there is none. The comparison is ordinal.
            /// </summary>
            public int IndexOf(string text)
            {
                int count = Count;
                for (int i = 1; i <= count; i++)
                {
                    if (string.Equals(this[i], text, StringComparison.Ordinal))
                        return i;
                }

                return -1;
            }

            /// <summary>True if an item with the given text exists.</summary>
            public bool Contains(string text) => IndexOf(text) >= 0;

            /// <summary>
            /// Sets the image shown beside the item at the given one-based position.
            /// Requires ShowImage to have been set at construction, and the item must
            /// already exist. Not shown in the edit box.
            /// [Windows and GTK only] (write only) (since 3.6)
            /// </summary>
            public void SetImage(int position, Image image)
            {
                CheckPosition(position);
                _owner.CheckAlive();
                IupNative.SetAttributeHandleId(_owner.Handle, "IMAGE", position,
                    image == null ? IntPtr.Zero : image.Handle);
            }

            /// <summary>
            /// Sets the image shown beside the item at the given one-based position, by
            /// name rather than by object. Accepts a stock image name (after
            /// IupImageLib.Open), a name registered with IupSetHandle, a system
            /// resource name, or a path to an image file.
            /// [Windows and GTK only] (write only) (since 3.6)
            /// </summary>
            /// <remarks>
            /// This and <see cref="SetImage"/> set the same IUP attribute for a given
            /// position, so whichever is called last wins. Unlike the image properties
            /// on other controls there is no cached managed reference here, so an image
            /// object passed to SetImage must be kept alive by the caller.
            /// </remarks>
            public void SetImageName(int position, string imageName)
            {
                CheckPosition(position);
                _owner.CheckAlive();
                _owner.SetAttributeId("IMAGE", position, imageName);
            }

            public IEnumerator<string> GetEnumerator()
            {
                int count = Count;
                for (int i = 1; i <= count; i++)
                    yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private static void CheckPosition(int position)
            {
                if (position < 1)
                    throw new ArgumentOutOfRangeException(nameof(position),
                        "List positions start at 1.");
            }
        }

        #endregion

        #region SELECTION

        /// <summary>
        /// Gets or sets the selected item's one-based position, or 0 when nothing is
        /// selected. Not meaningful when EditBox or Multiple is true - use Text or
        /// SelectedPositions instead.
        /// (non inheritable)
        /// </summary>
        public int SelectedPosition
        {
            get
            {
                string v = GetAttribute("VALUE");
                return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)
                    ? i
                    : 0;
            }
            set => SetAttribute("VALUE", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the selected item's text. Works only when EditBox is false and
        /// DropDown is true, or when both DropDown and Multiple are false. Setting it
        /// selects the first item with the same text.
        /// (non inheritable) (since 3.12)
        /// </summary>
        public string SelectedText
        {
            get => GetAttribute("VALUESTRING");
            set => SetAttribute("VALUESTRING", value);
        }

        /// <summary>
        /// Gets or sets the text in the edit box. Works only when EditBox is true.
        /// (non inheritable)
        /// </summary>
        public string Text
        {
            get => GetAttribute("VALUE") ?? "";
            set => SetAttribute("VALUE", value);
        }

        /// <summary>
        /// Sets the edit box text, but only if it passes the Mask. Works only when
        /// EditBox is true.
        /// (write only) (since 3.13)
        /// </summary>
        public string TextMasked
        {
            set => SetAttribute("VALUEMASKED", value);
        }

        /// <summary>
        /// Gets or sets the selected positions when Multiple is true. Positions are
        /// one-based. Setting replaces the whole selection.
        /// </summary>
        /// <remarks>
        /// IUP represents this as a string of '+' and '-', one per item. This property
        /// converts in both directions, and pads or truncates to the current item count
        /// when setting.
        /// </remarks>
        public int[] SelectedPositions
        {
            get
            {
                string v = GetAttribute("VALUE");
                if (string.IsNullOrEmpty(v))
                    return Array.Empty<int>();

                var result = new System.Collections.Generic.List<int>();
                for (int i = 0; i < v.Length; i++)
                {
                    if (v[i] == '+')
                        result.Add(i + 1);
                }

                return result.ToArray();
            }
            set
            {
                int count = Items.Count;
                char[] flags = new char[count];

                for (int i = 0; i < count; i++)
                    flags[i] = '-';

                if (value != null)
                {
                    foreach (int pos in value)
                    {
                        if (pos >= 1 && pos <= count)
                            flags[pos - 1] = '+';
                    }
                }

                SetAttribute("VALUE", new string(flags));
            }
        }

        /// <summary>
        /// Scrolls the given one-based item to the top of the list, or as near as
        /// possible. Valid only when DropDown is false.
        /// (write only) (since 3.0)
        /// </summary>
        public void ScrollToItem(int position) =>
            SetAttribute("TOPITEM", position.ToString(CultureInfo.InvariantCulture));

        #endregion

        #region CONFIGURATION

        /// <summary>Gets whether this list is a dropdown. (creation only)</summary>
        public bool DropDown => GetAttribute("DROPDOWN") == "YES";

        /// <summary>Gets whether this list has an edit box. (creation only)</summary>
        public bool EditBox => GetAttribute("EDITBOX") == "YES";

        /// <summary>Gets whether several items can be selected at once. (creation only)</summary>
        public bool Multiple => GetAttribute("MULTIPLE") == "YES";

        /// <summary>Gets whether the list is kept alphabetically sorted. (creation only)</summary>
        public bool Sort => GetAttribute("SORT") == "YES";

        /// <summary>
        /// Opens or closes the dropdown list. Valid only when DropDown is true, and
        /// ignored before the element is mapped.
        /// (write only)
        /// </summary>
        public bool ShowDropDown
        {
            set => SetAttribute("SHOWDROPDOWN", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the list redraws itself as items change. Set it to
        /// false while adding many items, then back to true. Default: true.
        /// [Windows only] (non inheritable) (since 3.3)
        /// </summary>
        public virtual bool AutoRedraw
        {
            get => GetAttribute("AUTOREDRAW") != "NO";
            set => SetAttribute("AUTOREDRAW", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets whether internal drag and drop of items within this list is enabled,
        /// which also enables DragDropCB. Set with ListStyle.ShowDragDrop at
        /// construction.
        /// (non inheritable) (since 3.7)
        /// </summary>
        public bool ShowDragDrop => GetAttribute("SHOWDRAGDROP") == "YES";

        /// <summary>
        /// Gets whether each item can show an image. Set with ListStyle.ShowImage at
        /// construction.
        /// [Windows and GTK only] (since 3.6)
        /// </summary>
        public bool ShowImage => GetAttribute("SHOWIMAGE") == "YES";

        /// <summary>
        /// Gets or sets whether dragging items between lists is prepared. The drag and
        /// drop attributes must still be set to activate it. Default: false.
        /// (non inheritable) (since 3.10)
        /// </summary>
        public virtual bool DragDropList
        {
            get => GetAttribute("DRAGDROPLIST") == "YES";
            set => SetAttribute("DRAGDROPLIST", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether dropping files onto the control is enabled. Default:
        /// false, but defining DropFilesCB before the element is mapped enables it.
        /// [Windows and GTK only] (non inheritable) (since 3.0)
        /// </summary>
        public virtual bool DropFilesTarget
        {
            get => GetAttribute("DROPFILESTARGET") == "YES";
            set => SetAttribute("DROPFILESTARGET", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets whether focus traversal is enabled. Clear it with ListStyle.NoFocus at
        /// construction. In Windows the control still gets the focus when clicked.
        /// (non inheritable) (since 3.0)
        /// </summary>
        public bool CanFocus => GetAttribute("CANFOCUS") != "NO";

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
        /// Gets or sets the background colour of the text. Default: the global
        /// attribute TxtBgColor. Does nothing in GTK when DropDown is true.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; }

        /// <summary>
        /// Gets or sets the text colour. Default: the global attribute TxtFgColor.
        /// </summary>
        public override Color FgColor { get => base.FgColor; set => base.FgColor = value; }

        /// <summary>
        /// Gets whether scrollbars are attached when DropDown is false. Clear it with
        /// ListStyle.NoScrollBar at construction. When DropDown is true the scrollbars
        /// are system dependent and this is ignored.
        /// </summary>
        public bool ScrollBar => GetAttribute("SCROLLBAR") != "NO";

        /// <summary>
        /// Gets or sets whether scrollbars are shown only when needed. Default: true.
        /// Not supported in Motif.
        /// </summary>
        public virtual bool AutoHide
        {
            get => GetAttribute("AUTOHIDE") != "NO";
            set => SetAttribute("AUTOHIDE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the dropped list expands to fit the widest item.
        /// Default: true.
        /// [Windows only]
        /// </summary>
        public virtual bool DropExpand
        {
            get => GetAttribute("DROPEXPAND") != "NO";
            set => SetAttribute("DROPEXPAND", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the internal padding for each item. Vertically the gap between
        /// items ends up twice this value. Valid only when DropDown is false.
        /// (since 3.0)
        /// </summary>
        public int Spacing
        {
            get => GetInt("SPACING", 0);
            set => SetAttribute("SPACING", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the internal margin of the edit box. Works only when EditBox is
        /// true. Default: (0,0).
        /// </summary>
        public (int, int) Padding
        {
            get { CheckAlive(); IupNative.GetIntInt(Handle, "PADDING", out int x, out int y); return (x, y); }
            set => SetAttribute("PADDING", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets how many items are visible when the dropdown is open.
        /// Default: 5.
        /// [Windows and Motif only]
        /// </summary>
        public int VisibleItems
        {
            get => GetInt("VISIBLEITEMS", 5);
            set => SetAttribute("VISIBLEITEMS", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the number of visible columns used for the natural size, which
        /// also acts as a minimum. Setting this speeds up natural size computation for
        /// very large lists.
        /// (since 3.0)
        /// </summary>
        public int VisibleColumns
        {
            get => GetInt("VISIBLECOLUMNS", 0);
            set => SetAttribute("VISIBLECOLUMNS", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the number of visible lines used for the natural size when
        /// DropDown is false, which also acts as a minimum.
        /// (since 3.0)
        /// </summary>
        public int VisibleLines
        {
            get => GetInt("VISIBLELINES", 0);
            set => SetAttribute("VISIBLELINES", value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region EDIT BOX

        /// <summary>
        /// Gets or sets the caret position in the edit box as a zero based character
        /// index. Works only when EditBox is true.
        /// (non inheritable)
        /// </summary>
        public int CaretPos
        {
            get => GetInt("CARETPOS", 0);
            set => SetAttribute("CARETPOS", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the selection in the edit box as a zero based character range,
        /// where End is the position after the last selected character. Returns null
        /// when there is no selection. Works only when EditBox is true.
        /// (non inheritable)
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

                int.TryParse(p[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int a);
                int.TryParse(p[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int b);
                return (a, b);
            }
            set
            {
                if (value == null)
                    return;

                SetAttribute("SELECTIONPOS", $"{value.Value.Start}:{value.Value.End}");
            }
        }

        /// <summary>
        /// Gets or sets the selected text in the edit box. Returns null when there is
        /// no selection; assigning replaces the selection and does nothing when there
        /// is none. Works only when EditBox is true.
        /// (non inheritable)
        /// </summary>
        public string SelectedEditText
        {
            get => GetAttribute("SELECTEDTEXT");
            set => SetAttribute("SELECTEDTEXT", value);
        }

        /// <summary>Selects all the edit box text. Works only when EditBox is true.</summary>
        public void SelectAll() => SetAttribute("SELECTION", "ALL");

        /// <summary>Clears the edit box selection. Works only when EditBox is true.</summary>
        public void SelectNone() => SetAttribute("SELECTION", "NONE");

        /// <summary>
        /// Appends text at the end of the edit box. Ignored before the element is
        /// mapped. Works only when EditBox is true.
        /// </summary>
        public void AppendText(string text) => SetAttribute("APPEND", text);

        /// <summary>
        /// Inserts text at the caret in the edit box, replacing the selection if there
        /// is one. Ignored before the element is mapped. Works only when EditBox is
        /// true.
        /// </summary>
        public void InsertText(string text) => SetAttribute("INSERT", text);

        /// <summary>Copies the edit box selection to the clipboard.</summary>
        public void Copy() => SetAttribute("CLIPBOARD", "COPY");

        /// <summary>Cuts the edit box selection to the clipboard.</summary>
        public void Cut() => SetAttribute("CLIPBOARD", "CUT");

        /// <summary>Pastes the clipboard into the edit box at the caret.</summary>
        public void Paste() => SetAttribute("CLIPBOARD", "PASTE");

        /// <summary>Clears the edit box selection.</summary>
        public void ClearSelection() => SetAttribute("CLIPBOARD", "CLEAR");

        /// <summary>
        /// Gets or sets whether the edit box is read only. This restricts keyboard
        /// input only; the text can still be changed through Text. Default: false.
        /// </summary>
        public virtual bool ReadOnly
        {
            get => GetAttribute("READONLY") == "YES";
            set => SetAttribute("READONLY", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the maximum number of characters accepted from the keyboard in
        /// the edit box. Zero means no limit.
        /// </summary>
        public int MaxLength
        {
            get => GetInt("NC", 0);
            set => SetAttribute("NC", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets a mask that filters interactive input into the edit box.
        /// (non inheritable)
        /// </summary>
        public virtual string Mask
        {
            get => GetAttribute("MASK");
            set => SetAttribute("MASK", value);
        }

        /// <summary>
        /// Gets or sets a prompt shown while the edit box is empty. Requires Visual
        /// Styles on Windows.
        /// [Windows and GTK only] (non inheritable)
        /// </summary>
        public virtual string CueBanner
        {
            get => GetAttribute("CUEBANNER");
            set => SetAttribute("CUEBANNER", value);
        }

        static readonly (string, TextFilter)[] _filters = new[]
        {
            ("", TextFilter.None),
            ("LOWERCASE", TextFilter.Lowercase),
            ("UPPERCASE", TextFilter.Uppercase),
            ("NUMBER", TextFilter.Number)
        };

        /// <summary>
        /// Gets or sets a filter applied to characters typed into the edit box.
        /// [Windows only] (non inheritable)
        /// </summary>
        public virtual TextFilter Filter
        {
            get => Utils.MapAttrib(GetAttribute("FILTER"), _filters);
            set => SetAttribute("FILTER", value == TextFilter.None
                ? null
                : Utils.MapEnum(value, _filters));
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

        #endregion

        #region CALLBACKS

        private ListActionCallback _action;
        private IFnsii _actionInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when an item's selection state changes.
        /// Not called when MultiSelectCB is defined and Multiple is true.
        /// </summary>
        /// <remarks>
        /// The deselection call is simulated by IUP. If you add or remove items and
        /// rely on it, set the selection afterwards so IUP's idea of the previous state
        /// stays correct.
        /// </remarks>
        public ListActionCallback Action
        {
            get => _action;
            set
            {
                _action = value;
                _actionInternal = ActionInternal;
                SetCallback("ACTION", Utils.CastCallback<Icallback>(_actionInternal));
            }
        }
        private int ActionInternal(nint ih, string text, int item, int state)
        {
            try
            {
                var cb = new ListActionData(this, text, item, state);
                _action?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in List Action callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _valueChangedCB;
        private IFn _valueChangedCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called after the value was interactively changed,
        /// whether by changing the selection or editing the text.
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

        private MultiSelectCallback _multiSelectCB;
        private IFns _multiSelectCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the selection changes in a multiple
        /// selection list, called once the interaction is over. Only called when
        /// Multiple is true, and when it is defined the Action callback is not called.
        /// </summary>
        public MultiSelectCallback MultiSelectCB
        {
            get => _multiSelectCB;
            set
            {
                _multiSelectCB = value;
                _multiSelectCBInternal = MultiSelectCBInternal;
                SetCallback("MULTISELECT_CB", Utils.CastCallback<Icallback>(_multiSelectCBInternal));
            }
        }
        private int MultiSelectCBInternal(nint ih, string value)
        {
            try
            {
                var cb = new MultiSelectData(this, value);
                _multiSelectCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in MultiSelectCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private DblClickCallback _dblClickCB;
        private IFnis _dblClickCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the user double clicks an item.
        /// Called only when DropDown is false.
        /// (since 3.0)
        /// </summary>
        public DblClickCallback DblClickCB
        {
            get => _dblClickCB;
            set
            {
                _dblClickCB = value;
                _dblClickCBInternal = DblClickCBInternal;
                SetCallback("DBLCLICK_CB", Utils.CastCallback<Icallback>(_dblClickCBInternal));
            }
        }
        private int DblClickCBInternal(nint ih, int item, string text)
        {
            try
            {
                var cb = new DblClickData(this, item, text);
                _dblClickCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in DblClickCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private DropStateCallback _dropDownCB;
        private IFni _dropDownCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the dropdown list is shown or hidden.
        /// Called only when DropDown is true.
        /// (since 3.0)
        /// </summary>
        public DropStateCallback DropDownCB
        {
            get => _dropDownCB;
            set
            {
                _dropDownCB = value;
                _dropDownCBInternal = DropDownCBInternal;
                SetCallback("DROPDOWN_CB", Utils.CastCallback<Icallback>(_dropDownCBInternal));
            }
        }
        private int DropDownCBInternal(nint ih, int state)
        {
            try
            {
                var cb = new DropStateData(this, state);
                _dropDownCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in List DropDownCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private TextActionCallback _editCB;
        private IFnis _editCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the edit box text is changed by the
        /// user, before the value is actually updated. Works only when EditBox is true.
        /// This is the same shape as the Text control's Action callback.
        /// </summary>
        public TextActionCallback EditCB
        {
            get => _editCB;
            set
            {
                _editCB = value;
                _editCBInternal = EditCBInternal;
                SetCallback("EDIT_CB", Utils.CastCallback<Icallback>(_editCBInternal));
            }
        }
        private int EditCBInternal(nint ih, int c, string newValue)
        {
            try
            {
                var cb = new TextActionData(this, c, newValue);
                _editCB?.Invoke(cb);

                if (cb.Replacement != '\0')
                    return cb.Replacement;

                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in EditCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private CaretCBCallback _caretCB;
        private IFniii _caretCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the caret position changes in the
        /// edit box. Works only when EditBox is true. The line is always 1 and the
        /// position is always column - 1.
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
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in List CaretCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private DragDropCallback _dragDropCB;
        private IFniiii _dragDropCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when an internal drag and drop completes.
        /// Active only when ShowDragDrop is true.
        /// </summary>
        /// <remarks>
        /// Setting the callback data's Result to Continue, or leaving the callback
        /// undefined, moves the item to the new position. Holding Ctrl copies instead
        /// of moving.
        /// </remarks>
        public DragDropCallback DragDropCB
        {
            get => _dragDropCB;
            set
            {
                _dragDropCB = value;
                _dragDropCBInternal = DragDropCBInternal;
                SetCallback("DRAGDROP_CB", Utils.CastCallback<Icallback>(_dragDropCBInternal));
            }
        }
        private int DragDropCBInternal(nint ih, int dragId, int dropId, int isShift, int isControl)
        {
            try
            {
                var cb = new DragDropData(this, dragId, dropId, isShift, isControl);
                _dragDropCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in DragDropCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private ButtonCBCallback _buttonCB;
        private IFniiiis _buttonCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when a mouse button is pressed or released
        /// inside the list. Called only when DropDown is false, and when there is an
        /// edit box only for clicks on the list part.
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
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in List ButtonCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private MotionCBCallback _motionCB;
        private IFniis _motionCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse moves over the list. Called
        /// only when DropDown is false, and when there is an edit box only over the
        /// list part.
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
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in List MotionCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private DropFilesCBCallback _dropFilesCB;
        private IFnsiii _dropFilesCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when one or more files are dropped on the
        /// control.
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
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in List DropFilesCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}