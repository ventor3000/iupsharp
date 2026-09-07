using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>Where the tab buttons sit relative to the tab contents.</summary>
    public enum TabType
    {
        /// <summary>Along the top edge. This is the default.</summary>
        Top,
        /// <summary>Along the bottom edge.</summary>
        Bottom,
        /// <summary>Down the left side.</summary>
        Left,
        /// <summary>Down the right side.</summary>
        Right
    }

    /// <summary>
    /// Creates a native container holding several children in hidden layers, with one
    /// visible at a time and a row of tab buttons for choosing between them. Native
    /// systems call it a notebook. (since 3.0)
    /// </summary>
    /// <remarks>
    /// <para><b>A tab's title and image are attributes of the child, not of the
    /// container</b> - IUP stores them as TABTITLEn and TABIMAGEn indexed by position.
    /// The <see cref="TabItems"/> collection handles that indexing, so use
    /// <c>tabs.TabItems[0].Title</c> rather than reaching for the raw attributes.</para>
    ///
    /// <para>Setting a title or image before the child is appended also works, through
    /// the child's own TABTITLE and TABIMAGE attributes - but only before it is added,
    /// which is why this wrapper does not offer that route.</para>
    ///
    /// <para>Unlike Zbox, Tabs does <b>not</b> use the VISIBLE attribute to decide
    /// what is shown. Set <c>TabItems[n].Visible</c> to hide a tab; the indices do not
    /// change when one is hidden.</para>
    ///
    /// <para>There is deliberately no way to disable a single tab. That is a design
    /// decision shared by all the native toolkits, not an IUP limitation.</para>
    ///
    /// <example>
    /// <code>
    /// var tabs = new Tabs(generalPage, advancedPage);
    /// tabs.TabItems[0].Title = "&amp;General";
    /// tabs.TabItems[1].Title = "&amp;Advanced";
    /// tabs.SelectedIndex = 0;
    /// </code>
    /// </example>
    /// </remarks>
    public class Tabs : ContainerControl
    {
        private readonly System.Collections.Generic.List<Control> _children = new();
        private readonly TabCollection _tabs;

        /// <summary>
        /// Creates a new tab container holding the given children, one per tab. It can
        /// also be created empty and filled later with Append.
        /// </summary>
        public Tabs(params Control[] children)
            : base(NativeIup.IupTabsv(new nint[] { IntPtr.Zero }))
        {
            _tabs = new TabCollection(this);

            if (children == null)
                return;

            foreach (Control child in children)
                Append(child);
        }

        /// <summary>
        /// Creates a new tab container from a list of titled tabs, so the whole
        /// notebook can be written as one expression.
        /// </summary>
        /// <example>
        /// <code>
        /// var tabs = new Tabs(
        ///     new Tab("&amp;General",  new VBox(nameField, emailField)),
        ///     new Tab("&amp;Advanced", new VBox(logLevel, cachePath)));
        /// </code>
        /// </example>
        public Tabs(params Tab[] tabs)
            : base(NativeIup.IupTabsv(new nint[] { IntPtr.Zero }))
        {
            _tabs = new TabCollection(this);

            if (tabs == null)
                return;

            foreach (Tab tab in tabs)
                Append(tab);
        }

        #region CHILDREN

        /// <summary>
        /// Adds a child as a new tab at the end, with no title. Set one afterwards
        /// through TabItems, or use the Tab overload to give it one up front.
        /// </summary>
        public override void Append(Control child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            base.Append(child);
            _children.Add(child);
        }

        /// <summary>
        /// Adds a titled tab at the end.
        /// </summary>
        /// <remarks>
        /// The title and image are written to the child before it is appended, which
        /// is what IUP requires - its child-side TABTITLE and TABIMAGE attributes are
        /// read when the child joins the container and ignored afterwards. To change
        /// them later, use <c>TabItems[n]</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The tab, or its child, is null.</exception>
        public virtual void Append(Tab tab)
        {
            if (tab == null)
                throw new ArgumentNullException(nameof(tab));
            if (tab.Child == null)
                throw new ArgumentNullException(nameof(tab), "The tab has no child.");

            // Must be set on the child BEFORE it is appended.
            if (tab.Title != null)
                tab.Child.SetAttribute("TABTITLE", tab.Title);

            if (tab.ImageName != null)
                tab.Child.SetAttribute("TABIMAGE", tab.ImageName);

            Append(tab.Child);

            // An Image object cannot go through the child-side attribute, since that
            // one is a name. Apply it by position once the child is in place.
            if (tab.Image != null)
                TabItems[_children.Count - 1].SetImage(tab.Image);
        }

        /// <summary>
        /// Gets the number of tabs. Reads IUP's COUNT attribute, which is the same
        /// value IupGetChildCount returns.
        /// (since 3.3)
        /// </summary>
        public int Count => GetInt("COUNT", _children.Count);

        /// <summary>
        /// Gets the tabs, indexed from 0. Each entry carries the title, image and
        /// visibility of one tab, which IUP stores against this container rather than
        /// against the child.
        /// </summary>
        public TabCollection TabItems => _tabs;

        #endregion

        #region CURRENT TAB

        /// <summary>
        /// Gets or sets the current tab by its zero based position. The first child
        /// added is current when the container is created.
        /// (non inheritable) (since 3.0)
        /// </summary>
        /// <remarks>
        /// In GTK, reading this inside TabChangeCB still returns the previous value.
        /// </remarks>
        public int SelectedIndex
        {
            get => GetInt("VALUEPOS", 0);
            set => SetAttribute("VALUEPOS", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the current tab by its child element.
        /// (non inheritable) (since 3.0)
        /// </summary>
        /// <remarks>
        /// Returns null if the current child was not created through IupSharp. Setting
        /// it requires the element to be one of this container's children.
        /// </remarks>
        public virtual Control SelectedChild
        {
            get
            {
                CheckAlive();
                return Find<Control>(NativeIup.GetAttributePtr(Handle, "VALUE_HANDLE"));
            }
            set
            {
                CheckAlive();
                NativeIup.SetAttributeHandle(Handle, "VALUE_HANDLE",
                    value == null ? IntPtr.Zero : value.Handle);
            }
        }

        #endregion

        #region APPEARANCE

        static readonly (string, TabType)[] _tabTypes = new[]
        {
            ("TOP", TabType.Top),
            ("BOTTOM", TabType.Bottom),
            ("LEFT", TabType.Left),
            ("RIGHT", TabType.Right)
        };

        /// <summary>
        /// Gets or sets where the tab buttons sit. Default: Top.
        /// (non inheritable) (creation only in Windows)
        /// </summary>
        /// <remarks>
        /// On Windows this cannot be changed after the element is mapped, and choosing
        /// Left or Right also forces Multiline on and TabOrientation to Vertical.
        /// Anything other than Top removes the visual style from the tabs there.
        /// </remarks>
        public virtual TabType TabType
        {
            get => Utils.MapAttrib(GetAttribute("TABTYPE"), _tabTypes);
            set => SetAttribute("TABTYPE", Utils.MapEnum(value, _tabTypes));
        }

        /// <summary>
        /// Gets or sets the direction of the tab title text. Default: Horizontal.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// Vertical is supported only on GTK and Windows. On Windows it cannot be set
        /// directly - it follows TabType, becoming Vertical for Left or Right and
        /// Horizontal for Top or Bottom. GTK is the only system that supports vertical
        /// text with TabType.Top.
        /// </remarks>
        public virtual Orientation TabOrientation
        {
            get => GetAttribute("TABORIENTATION") == "VERTICAL"
                ? Orientation.Vertical
                : Orientation.Horizontal;
            set => SetAttribute("TABORIENTATION",
                value == Orientation.Vertical ? "VERTICAL" : "HORIZONTAL");
        }

        /// <summary>
        /// Gets or sets the internal margin of the tab titles, in pixels. Works like
        /// the Margin of a VBox or HBox, but named differently to avoid inheritance
        /// problems. Default: (0,0).
        /// (non inheritable) (since 3.0, renamed in 3.19)
        /// </summary>
        public (int, int) TabPadding
        {
            get { CheckAlive(); NativeIup.GetIntInt(Handle, "TABPADDING", out int x, out int y); return (x, y); }
            set => SetAttribute("TABPADDING", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets whether several rows of tab buttons are allowed, which hides
        /// the tab scroller and fits every button on screen. Default: false. Always on
        /// when TabType is Left or Right.
        /// [Windows only] (non inheritable) (since 3.0)
        /// </summary>
        public virtual bool Multiline
        {
            get => GetAttribute("MULTILINE") == "YES";
            set => SetAttribute("MULTILINE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether each tab shows a close button. Default: false.
        /// [Windows and GTK only] (non inheritable) (since 3.10)
        /// </summary>
        /// <remarks>
        /// A closed tab is hidden rather than removed unless TabCloseCB says
        /// otherwise. On Windows enabling this forces the classic visual style.
        /// </remarks>
        public virtual bool ShowClose
        {
            get => GetAttribute("SHOWCLOSE") == "YES";
            set => SetAttribute("SHOWCLOSE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the natural size is computed from every child rather
        /// than only the current one. Default: true.
        /// (non inheritable) (since 3.27)
        /// </summary>
        public virtual bool ChildSizeAll
        {
            get => GetAttribute("CHILDSIZEALL") != "NO";
            set => SetAttribute("CHILDSIZEALL", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets a position offset for the children, in pixels. It does not
        /// affect the natural size, and allows positioning controls outside the client
        /// area. Default: (0,0).
        /// (since 3.14)
        /// </summary>
        public (int, int) ChildOffset
        {
            get { CheckAlive(); NativeIup.GetIntInt(Handle, "CHILDOFFSET", out int x, out int y); return (x, y); }
            set => SetAttribute("CHILDOFFSET", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets the background colour. Default: the global attribute
        /// DlgBgColor. On Windows, and on GTK under Windows, the tab buttons'
        /// background is always chosen by the system and this affects only the client
        /// area.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; }

        /// <summary>
        /// Gets or sets the tab title colour. Default: the global attribute
        /// DlgFgColor.
        /// </summary>
        public override Color FgColor { get => base.FgColor; set => base.FgColor = value; }

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

        protected override void OnDestroying()
        {
            _children.Clear();
            base.OnDestroying();
        }

        // ==================================================================
        // Tab collection
        // ==================================================================

        /// <summary>
        /// The tabs of a <see cref="Tabs"/> container, indexed from 0.
        /// </summary>
        /// <remarks>
        /// IUP keeps a tab's title, image and visibility on the container as
        /// TABTITLEn, TABIMAGEn and TABVISIBLEn rather than on the child, so this
        /// collection exists to hide that indexing.
        /// </remarks>
        public sealed class TabCollection : IEnumerable<TabItem>
        {
            private readonly Tabs _owner;

            internal TabCollection(Tabs owner)
            {
                _owner = owner;
            }

            /// <summary>Gets the number of tabs.</summary>
            public int Count => _owner.Count;

            /// <summary>
            /// Gets the tab at the given zero based position. A fresh accessor is
            /// returned each time; it reads and writes the container's indexed
            /// attributes rather than holding any state, so it is meaningful only
            /// while that position exists.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException">The index is negative.</exception>
            public TabItem this[int index]
            {
                get
                {
                    if (index < 0)
                        throw new ArgumentOutOfRangeException(nameof(index),
                            "Tab indices start at 0.");

                    return new TabItem(_owner, index);
                }
            }

            public IEnumerator<TabItem> GetEnumerator()
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                    yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// One tab of a <see cref="Tabs"/> container: its title, image and
        /// visibility.
        /// </summary>
        /// <remarks>
        /// <para>A lightweight accessor rather than an element of its own. It holds
        /// the container and a position, so it stops being meaningful if tabs are
        /// added or removed before that position.</para>
        ///
        /// <para>Deliberately a class rather than a struct: the indexer returns a new
        /// instance each time, and assigning to a property of a returned struct would
        /// be a compile error even though the write goes straight to the container's
        /// attributes.</para>
        /// </remarks>
        public sealed class TabItem
        {
            private readonly Tabs _owner;

            /// <summary>The zero based position of this tab.</summary>
            public int Index { get; }

            internal TabItem(Tabs owner, int index)
            {
                _owner = owner;
                Index = index;
            }

            /// <summary>
            /// Gets or sets the text shown in the tab button. The "&amp;" character
            /// defines a mnemonic, activated with Alt from anywhere in the dialog; use
            /// "&amp;&amp;" for a literal "&amp;".
            /// (non inheritable) (mnemonics since 3.3)
            /// </summary>
            public string Title
            {
                get => _owner.GetAttributeId("TABTITLE", Index);
                set => _owner.SetAttributeId("TABTITLE", Index, value);
            }

            /// <summary>
            /// Sets the tab's image from an Image object.
            /// (non inheritable) (since 3.0)
            /// </summary>
            /// <remarks>
            /// <para>On Motif the image appears only when Title is null. On Windows and
            /// Motif, set the container's BgColor before setting an image.</para>
            ///
            /// <para>Unlike the image properties on other controls, no managed
            /// reference is kept here, so the caller must keep the Image alive for as
            /// long as the tab uses it.</para>
            /// </remarks>
            public void SetImage(Image image)
            {
                _owner.CheckAlive();
                NativeIup.SetAttributeHandleId(_owner.Handle, "TABIMAGE", Index,
                    image == null ? IntPtr.Zero : image.Handle);
            }

            /// <summary>
            /// Gets or sets the tab's image by name: a stock image name (after
            /// ImageLib.Open), a name registered with IupSetHandle, a system resource
            /// name, or a path to an image file.
            /// (non inheritable) (since 3.0)
            /// </summary>
            public string ImageName
            {
                get => _owner.GetAttributeId("TABIMAGE", Index);
                set => _owner.SetAttributeId("TABIMAGE", Index, value);
            }

            /// <summary>
            /// Gets or sets whether the tab is shown. Default: true. Hiding a tab does
            /// not change any indices.
            /// (non inheritable) (since 3.8)
            /// </summary>
            public bool Visible
            {
                get => _owner.GetAttributeId("TABVISIBLE", Index) != "NO";
                set => _owner.SetAttributeId("TABVISIBLE", Index, value ? "YES" : "NO");
            }
        }

        // ==================================================================
        // Callbacks
        // ==================================================================

        #region CALLBACKS

        private TabChangeCallback _tabChangeCB;
        private IFnnn _tabChangeCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when the user changes the current tab. It is
        /// not called when the tab is changed or removed programmatically.
        /// </summary>
        /// <remarks>
        /// <b>Setting this suppresses <see cref="TabChangePosCB"/></b>, which IUP calls
        /// only when this one is undefined.
        /// </remarks>
        public TabChangeCallback TabChangeCB
        {
            get => _tabChangeCB;
            set
            {
                _tabChangeCB = value;
                _tabChangeCBInternal = TabChangeCBInternal;
                SetCallback("TABCHANGE_CB", Utils.CastCallback<Icallback>(_tabChangeCBInternal));
            }
        }
        private int TabChangeCBInternal(nint ih, nint newTab, nint oldTab)
        {
            try
            {
                var cb = new TabChangeData(this, Find<Control>(newTab), Find<Control>(oldTab));
                _tabChangeCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Tabs TabChangeCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private TabChangePosCallback _tabChangePosCB;
        private IFnii _tabChangePosCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when the user changes the current tab,
        /// reporting positions rather than elements. Called only when
        /// <see cref="TabChangeCB"/> is not defined.
        /// (since 3.3)
        /// </summary>
        public TabChangePosCallback TabChangePosCB
        {
            get => _tabChangePosCB;
            set
            {
                _tabChangePosCB = value;
                _tabChangePosCBInternal = TabChangePosCBInternal;
                SetCallback("TABCHANGEPOS_CB", Utils.CastCallback<Icallback>(_tabChangePosCBInternal));
            }
        }
        private int TabChangePosCBInternal(nint ih, int newPos, int oldPos)
        {
            try
            {
                var cb = new TabChangePosData(this, newPos, oldPos);
                _tabChangePosCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Tabs TabChangePosCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private TabPositionCallback _tabCloseCB;
        private IFni _tabCloseCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when the user clicks a tab's close button.
        /// Only called when ShowClose is true.
        /// [Windows and GTK only] (since 3.10)
        /// </summary>
        /// <remarks>
        /// The result decides what happens: Default hides the tab, which is also what
        /// happens with no callback at all; Continue removes the tab and destroys its
        /// children; Ignore does nothing.
        /// </remarks>
        public TabPositionCallback TabCloseCB
        {
            get => _tabCloseCB;
            set
            {
                _tabCloseCB = value;
                _tabCloseCBInternal = TabCloseCBInternal;
                SetCallback("TABCLOSE_CB", Utils.CastCallback<Icallback>(_tabCloseCBInternal));
            }
        }
        private int TabCloseCBInternal(nint ih, int pos)
        {
            try
            {
                var cb = new TabPositionData(this, pos);
                _tabCloseCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Tabs TabCloseCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private TabPositionCallback _rightClickCB;
        private IFni _rightClickCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when the user right clicks a tab button,
        /// typically to show a context menu.
        /// (since 3.10)
        /// </summary>
        public TabPositionCallback RightClickCB
        {
            get => _rightClickCB;
            set
            {
                _rightClickCB = value;
                _rightClickCBInternal = RightClickCBInternal;
                SetCallback("RIGHTCLICK_CB", Utils.CastCallback<Icallback>(_rightClickCBInternal));
            }
        }
        private int RightClickCBInternal(nint ih, int pos)
        {
            try
            {
                var cb = new TabPositionData(this, pos);
                _rightClickCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Tabs RightClickCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private FocusCBCallback _focusCB;
        private IFni _focusCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when a child of the container gains or loses
        /// the focus. Only called for children that have PropagateFocus set.
        /// (since 3.23)
        /// </summary>
        public FocusCBCallback FocusCB
        {
            get => _focusCB;
            set
            {
                _focusCB = value;
                _focusCBInternal = FocusCBInternal;
                SetCallback("FOCUS_CB", Utils.CastCallback<Icallback>(_focusCBInternal));
            }
        }
        private int FocusCBInternal(nint ih, int focus)
        {
            try
            {
                var cb = new FocusCBData(this, focus);
                _focusCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Tabs FocusCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}