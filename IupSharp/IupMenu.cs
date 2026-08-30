using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace IupSharp
{
    // =====================================================================
    // MenuElement - shared base
    // =====================================================================

    /// <summary>
    /// Base class for the three things a Menu can contain: Item, Submenu and
    /// MenuSeparator. Nothing else may be placed in a menu.
    /// </summary>
    /// <remarks>
    /// Menu elements are not Controls. They have no size, position, font, colours or
    /// focus, so the Control surface would be meaningless on them. They are mapped
    /// along with the menu, which is why they derive from MappableObject.
    /// </remarks>
    public abstract class MenuElement : MappableObject
    {
        protected MenuElement(nint handle) : base(handle)
        {
        }

        /// <summary>
        /// Gets or sets whether the element is enabled. Default: true.
        /// </summary>
        public virtual bool Active
        {
            get => GetAttribute("ACTIVE") != "NO";
            set => SetAttribute("ACTIVE", value ? "YES" : "NO");
        }
    }


    // =====================================================================
    // MenuSeparator
    // =====================================================================

    /// <summary>
    /// Creates a horizontal line between two menu items, used to group related
    /// items.
    /// </summary>
    /// <remarks>
    /// A separator is ignored when it appears directly in a menu bar; it is only
    /// meaningful inside a dropdown or popup menu.
    /// </remarks>
    public class Separator : MenuElement
    {
        /// <summary>Creates a new menu separator.</summary>
        public Separator() : base(IupNative.IupSeparator())
        {
        }
    }


    // =====================================================================
    // Item
    // =====================================================================

    /// <summary>
    /// Creates a menu item. When selected it generates an action.
    /// </summary>
    public class MenuItem : MenuElement
    {
        /// <summary>
        /// Creates a new menu item.
        /// </summary>
        /// <param name="title">
        /// The item text. Use "&amp;" before a character to define a mnemonic, and
        /// "&amp;&amp;" to show a literal "&amp;". A '\t' right-aligns everything after
        /// it, which is the convention for showing a shortcut, as in "Save\tCtrl+S".
        /// </param>
        public MenuItem(string title = null) : base(IupNative.Item(title, null))
        {
        }

        /// <summary>
        /// Creates a new menu item with an action.
        /// </summary>
        /// <param name="title">The item text.</param>
        /// <param name="action">The action invoked when the item is selected.</param>
        public MenuItem(string title, Callback action) : this(title)
        {
            Action = action;
        }

        #region ATTRIBUTES

        /// <summary>
        /// Gets or sets the item text. The "&amp;" character defines a mnemonic; use
        /// "&amp;&amp;" for a literal "&amp;". A '\t' right-aligns the remaining text,
        /// which is how shortcut hints are shown, as in "Save\tCtrl+S".
        /// </summary>
        /// <remarks>
        /// The shortcut itself is NOT implemented by writing it in the title. Handle it
        /// with the dialog's KAny callback.
        /// </remarks>
        public string Title
        {
            get => GetAttribute("TITLE") ?? "";
            set => SetAttribute("TITLE", value);
        }

        /// <summary>
        /// Gets or sets whether the item shows a check mark. Default: false. An item
        /// directly in a menu bar cannot be checked, and the check mark is not shown
        /// when Image is set.
        /// (non inheritable)
        /// </summary>
        public virtual bool Checked
        {
            get => GetAttribute("VALUE") == "ON";
            set => SetAttribute("VALUE", value ? "ON" : "OFF");
        }

        /// <summary>
        /// Gets or sets whether Checked toggles automatically when the item is
        /// activated. Default: false.
        /// (non inheritable) (since 3.0)
        /// </summary>
        public virtual bool AutoToggle
        {
            get => GetAttribute("AUTOTOGGLE") == "YES";
            set => SetAttribute("AUTOTOGGLE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the check box is hidden, which also makes the item
        /// impossible to check. When every item in a menu sets this, no empty space is
        /// left in front of the items.
        /// [Motif and GTK only]
        /// </summary>
        /// <remarks>
        /// Since GTK 2.14 the unmarked check box is always shown, so an item that will
        /// never be checked should set this to true. Under that GTK the default is
        /// true, unless Checked has been set, in which case it is false.
        /// </remarks>
        public virtual bool HideMark
        {
            get => GetAttribute("HIDEMARK") == "YES";
            set => SetAttribute("HIDEMARK", value ? "YES" : "NO");
        }

        private Image _image;
        private string _imageName;
        /// <summary>
        /// Gets or sets the check mark image used when Checked is false. About 16x16
        /// fits best; a larger image is cropped in Windows. Ignored for an item in a
        /// menu bar.
        /// [Windows and GTK only] (non inheritable)
        /// </summary>
        public virtual Image Image
        {
            get => _image;
            set => SetImageHandle("IMAGE", value, ref _image, ref _imageName);
        }

        /// <summary>
        /// Gets or sets the check mark image for the unchecked state by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="Image"/> set the same IUP attribute, so assigning
        /// either clears the other. Reading this returns null when the image was set
        /// as an object.
        /// </remarks>
        public virtual string ImageName
        {
            get => _imageName;
            set => SetImageName("IMAGE", value, ref _image, ref _imageName);
        }


        private Image _impress;
        private string _impressName;
        /// <summary>
        /// Gets or sets the check mark image used when Checked is true.
        /// [Windows and GTK only] (non inheritable)
        /// </summary>
        public virtual Image ImPress
        {
            get => _impress;
            set => SetImageHandle("IMPRESS", value, ref _impress, ref _impressName);
        }

        /// <summary>
        /// Gets or sets the check mark image for the checked state by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="ImPress"/> set the same IUP attribute, so assigning
        /// either clears the other. Reading this returns null when the image was set
        /// as an object.
        /// </remarks>
        public virtual string ImPressName
        {
            get => _impressName;
            set => SetImageName("IMPRESS", value, ref _impress, ref _impressName);
        }


        private Image _titleImage;
        private string _titleImageName;
        /// <summary>
        /// Gets or sets an image shown alongside the title. In Windows it appears
        /// before the text and after the check mark area, so both can be visible. In
        /// GTK it appears in the check mark area. In Motif it replaces the text and
        /// must be set before the element is mapped.
        /// (non inheritable) (since 3.0)
        /// </summary>
        public virtual Image TitleImage
        {
            get => _titleImage;
            set => SetImageHandle("TITLEIMAGE", value, ref _titleImage, ref _titleImageName);
        }

        /// <summary>
        /// Gets or sets the title image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="TitleImage"/> set the same IUP attribute, so assigning
        /// either clears the other. Reading this returns null when the image was set
        /// as an object.
        /// </remarks>
        public virtual string TitleImageName
        {
            get => _titleImageName;
            set => SetImageName("TITLEIMAGE", value, ref _titleImage, ref _titleImageName);
        }


        #endregion

        protected override void OnDestroying()
        {
            _image = null;
            _impress = null;
            _titleImage = null;
            _imageName = null;
            _impressName = null;
            _titleImageName = null;
            base.OnDestroying();
        }

        #region CALLBACKS

        private Callback _action;
        private IFn _actionInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the item is selected. Setting the
        /// callback data's Result to Close ends the main loop, or closes the current
        /// popup dialog when the item is in a popup menu.
        /// </summary>
        public Callback Action
        {
            get => _action;
            set
            {
                _action = value;
                _actionInternal = ActionInternal;
                SetCallback("ACTION", Utils.CastCallback<Icallback>(_actionInternal));
            }
        }
        private int ActionInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _action?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in Item Action callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _highlightCB;
        private IFn _highlightCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the item is highlighted, which
        /// happens as the user moves through the menu. Useful for showing help text in
        /// a status bar.
        /// </summary>
        public Callback HighlightCB
        {
            get => _highlightCB;
            set
            {
                _highlightCB = value;
                _highlightCBInternal = HighlightCBInternal;
                SetCallback("HIGHLIGHT_CB", Utils.CastCallback<Icallback>(_highlightCBInternal));
            }
        }
        private int HighlightCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _highlightCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in HighlightCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _helpCB;
        private IFn _helpCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the user presses F1 while the item is
        /// highlighted. Setting the callback data's Result to Close is processed.
        /// </summary>
        public Callback HelpCB
        {
            get => _helpCB;
            set
            {
                _helpCB = value;
                _helpCBInternal = HelpCBInternal;
                SetCallback("HELP_CB", Utils.CastCallback<Icallback>(_helpCBInternal));
            }
        }
        private int HelpCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _helpCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in Item HelpCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }


    // =====================================================================
    // Submenu
    // =====================================================================

    /// <summary>
    /// Creates a menu item that opens another menu when selected.
    /// </summary>
    public class Submenu : MenuElement
    {
        private Menu _menu;

        /// <summary>
        /// Creates a new submenu.
        /// </summary>
        /// <param name="title">The text shown on the submenu item.</param>
        /// <param name="menu">The menu opened when the item is selected. It can be null.</param>
        public Submenu(string title, Menu menu = null)
    : base(IupNative.Submenu(title, IntPtr.Zero))
        {
            Title = title;
            Menu = menu;
        }

        /// <summary>
        /// Gets or sets the menu opened when this submenu is selected. It becomes a
        /// child of the submenu, so it is destroyed along with it.
        ///
        /// <para><b>A submenu with no menu is not displayed at all.</b> IUP inserts the
        /// submenu into its parent when the child menu is mapped, so a submenu without
        /// one never appears, rather than appearing and doing nothing.</para>
        ///
        /// <para>Assigning over an existing menu detaches the old one, which then
        /// becomes the caller's responsibility to destroy. Assigning null detaches
        /// without replacing.</para>
        /// </summary>
        /// <exception cref="ArgumentException">The menu is already attached elsewhere.</exception>
        public virtual Menu Menu
        {
            get => _menu;
            set
            {
                CheckAlive();

                if (ReferenceEquals(_menu, value))
                    return;

                if (value != null && IupNative.IupGetParent(value.Handle) != IntPtr.Zero)
                    throw new ArgumentException(
                        "That menu already belongs to another element. Detach it first.", nameof(value));

                // A submenu accepts only one child, so the old menu must go first.
                if (_menu != null && _menu.Handle != IntPtr.Zero)
                    IupNative.IupDetach(_menu.Handle);

                _menu = value;

                if (value == null)
                    return;

                if (IupNative.IupAppend(Handle, value.Handle) == IntPtr.Zero)
                {
                    _menu = null;
                    throw new IupException("Failed to attach the menu to the submenu.");
                }

                // The parent menu may already be mapped, in which case the new child
                // needs mapping for the submenu to appear.
                if (IsMapped)
                    value.Map();
            }
        }

        /// <summary>
        /// Gets or sets the submenu text. The "&amp;" character defines a mnemonic; use
        /// "&amp;&amp;" for a literal "&amp;".
        /// </summary>
        public string Title
        {
            get => GetAttribute("TITLE") ?? "";
            set => SetAttribute("TITLE", value);
        }

        private Image _image;
        private string _imageName;
        /// <summary>
        /// Gets or sets the submenu image. About 16x16 fits best. Ignored for a submenu
        /// in a menu bar, and in Windows a menu bar item cannot have a check mark.
        /// (non inheritable) (since 3.0)
        /// </summary>
        public virtual Image Image
        {
            get => _image;
            set => SetImageHandle("IMAGE", value, ref _image, ref _imageName);
        }

        /// <summary>
        /// Gets or sets the submenu image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="Image"/> set the same IUP attribute, so assigning
        /// either clears the other. Reading this returns null when the image was set
        /// as an object.
        /// </remarks>
        public virtual string ImageName
        {
            get => _imageName;
            set => SetImageName("IMAGE", value, ref _image, ref _imageName);
        }


        protected override void OnDestroying()
        {
            _menu = null;
            _image = null;
            _imageName = null;
            base.OnDestroying();
        }


    }


    // =====================================================================
    // Menu
    // =====================================================================

    /// <summary>
    /// Groups menu elements. A menu can be a dialog's menu bar, a dropdown opened by
    /// a Submenu, or a popup menu shown with Popup.
    /// </summary>
    /// <remarks>
    /// <para>Only Item, Submenu and Separator may be added. Anything else is an error
    /// in IUP, so this wrapper enforces it through the MenuElement type.</para>
    ///
    /// <para><b>Ownership.</b> Destroy a menu only when it is a popup menu. A menu bar
    /// attached to a dialog is destroyed with the dialog, and a menu inside a Submenu
    /// is destroyed with the submenu. But if you replace a dialog's menu with another
    /// one, the previous menu must be destroyed explicitly - IUP only unmaps it.</para>
    ///
    /// <example>
    /// <code>
    /// var fileMenu = new Menu(
    ///     new Item("&amp;New\tCtrl+N", d =&gt; NewFile()),
    ///     new Item("&amp;Open\tCtrl+O", d =&gt; OpenFile()),
    ///     new MenuSeparator(),
    ///     new Item("E&amp;xit", d =&gt; d.Result = CallbackResult.Close));
    ///
    /// dialog.Menu = new Menu(new Submenu("&amp;File", fileMenu));
    /// </code>
    /// </example>
    /// </remarks>
    public class Menu : MappableObject, IEnumerable<MenuElement>
    {
        private readonly List<MenuElement> _children = new();

        /// <summary>
        /// Creates a new menu containing the given elements. It can also be created
        /// empty and filled later with Append or Insert.
        /// </summary>
        public Menu(params MenuElement[] children) : base(CreateEmpty())
        {
            if (children == null)
                return;

            foreach (MenuElement child in children)
                Append(child);
        }

        private static nint CreateEmpty()
        {
            // IupMenuv takes a NULL-terminated array; a single NULL creates an
            // empty menu, which is then filled with IupAppend.
            return IupNative.IupMenuv(new nint[] { IntPtr.Zero });
        }

        #region CHILDREN

        /// <summary>Gets the number of elements in the menu.</summary>
        public int Count => _children.Count;

        /// <summary>Gets the element at the given position.</summary>
        public MenuElement this[int index] => _children[index];

        /// <summary>
        /// Adds an element to the end of the menu.
        /// </summary>
        /// <exception cref="ArgumentNullException">The child is null.</exception>
        /// <exception cref="IupException">IUP rejected the element.</exception>
        public virtual void Append(MenuElement child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            CheckAlive();

            if (IupNative.IupAppend(Handle, child.Handle) == IntPtr.Zero)
                throw new IupException($"Failed to append {child.GetType().Name} to the menu.");

            _children.Add(child);
        }

        /// <summary>
        /// Inserts an element before the given reference element. Pass null as the
        /// reference to append at the end.
        /// </summary>
        /// <exception cref="ArgumentNullException">The child is null.</exception>
        /// <exception cref="ArgumentException">The reference element is not in this menu.</exception>
        public virtual void Insert(MenuElement refChild, MenuElement child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (refChild == null)
            {
                Append(child);
                return;
            }

            int index = _children.IndexOf(refChild);
            if (index < 0)
                throw new ArgumentException("The reference element is not in this menu.", nameof(refChild));

            CheckAlive();

            if (IupNative.IupInsert(Handle, refChild.Handle, child.Handle) == IntPtr.Zero)
                throw new IupException($"Failed to insert {child.GetType().Name} into the menu.");

            _children.Insert(index, child);
        }

        /// <summary>
        /// Detaches an element from the menu without destroying it, so it can be
        /// appended elsewhere. The caller becomes responsible for destroying it.
        /// </summary>
        public virtual void Remove(MenuElement child)
        {
            if (child == null || !_children.Remove(child))
                return;

            if (Handle != IntPtr.Zero && child.Handle != IntPtr.Zero)
                IupNative.IupDetach(child.Handle);
        }

        /// <summary>
        /// Removes an element from the menu and destroys it.
        /// </summary>
        public virtual void RemoveAndDestroy(MenuElement child)
        {
            if (child == null || !_children.Remove(child))
                return;

            child.Destroy();
        }

        /// <summary>
        /// Removes and destroys every element in the menu, leaving it empty.
        /// </summary>
        public virtual void Clear()
        {
            // Copy first: Destroy fires LDESTROY_CB, which may reach back into here.
            var copy = _children.ToArray();
            _children.Clear();

            foreach (MenuElement child in copy)
                child.Destroy();
        }

        public IEnumerator<MenuElement> GetEnumerator() => _children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();

        #endregion

        #region ATTRIBUTES

        /// <summary>
        /// Gets or sets the menu background colour, which affects every item in the
        /// menu.
        /// (since 3.0)
        /// </summary>
        public Color BgColor
        {
            get => Utils.ParseColor(GetAttribute("BGCOLOR"));
            set => SetAttribute("BGCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets whether the menu behaves like a radio group: selecting one
        /// child item automatically deselects the others. Submenus and their children
        /// are not affected.
        /// (non inheritable)
        /// </summary>
        public virtual bool Radio
        {
            get => GetAttribute("RADIO") == "YES";
            set => SetAttribute("RADIO", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets how a popup menu is aligned relative to the point given to
        /// Popup. Default: Left combined with Top.
        /// (non inheritable) (since 3.28)
        /// </summary>
        public virtual Alignment PopupAlign
        {
            get => Utils.ParseAlignment(GetAttribute("POPUPALIGN"));
            set => SetAttribute("POPUPALIGN", Utils.FormatAlignment(value));
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Shows the menu as a popup at the given screen position, and returns once the
        /// user has dismissed it or chosen an item. Item actions are invoked before
        /// this returns.
        /// </summary>
        /// <param name="x">Horizontal position, or one of the IupNative position constants.</param>
        /// <param name="y">Vertical position, or one of the IupNative position constants.</param>
        /// <exception cref="IupException">The menu could not be shown.</exception>
        public void Popup(int x = IupNative.IUP_MOUSEPOS, int y = IupNative.IUP_MOUSEPOS)
        {
            CheckAlive();

            if (IupNative.IupPopup(Handle, x, y) != IupNative.IUP_NOERROR)
                throw new IupException("Failed to show the popup menu.");
        }

        #endregion

        protected override void OnDestroying()
        {
            // IUP destroys the children with the menu, so drop the references but do
            // not destroy them again.
            _children.Clear();
            base.OnDestroying();
        }

        #region CALLBACKS

        private Callback _openCB;
        private IFn _openCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called just before the menu is opened. Useful for
        /// enabling or checking items to match the current state.
        /// </summary>
        public Callback OpenCB
        {
            get => _openCB;
            set
            {
                _openCB = value;
                _openCBInternal = OpenCBInternal;
                SetCallback("OPEN_CB", Utils.CastCallback<Icallback>(_openCBInternal));
            }
        }
        private int OpenCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _openCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in Menu OpenCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _menuCloseCB;
        private IFn _menuCloseCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called just after the menu is closed.
        /// </summary>
        public Callback MenuCloseCB
        {
            get => _menuCloseCB;
            set
            {
                _menuCloseCB = value;
                _menuCloseCBInternal = MenuCloseCBInternal;
                SetCallback("MENUCLOSE_CB", Utils.CastCallback<Icallback>(_menuCloseCBInternal));
            }
        }
        private int MenuCloseCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _menuCloseCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in MenuCloseCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}