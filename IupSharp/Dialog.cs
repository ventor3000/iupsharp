using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Changes how the dialog will be shown. After Show or Popup the attribute is
    /// set back to Normal. Full is similar to FullScreen but only the dialog client
    /// area covers the screen area; menu and decorations will be there but out of the
    /// screen. In UNIX there is a chance that the placement won't work correctly,
    /// that depends on the Window Manager.
    /// </summary>
    public enum Placement
    {
        Normal,
        Maximized,
        Minimized,
        Full
    }

    /// <summary>
    /// State reported by the ShowCB callback.
    /// </summary>
    public enum ShowState
    {
        /// <summary>The dialog was shown.</summary>
        Show = 0,
        /// <summary>The dialog was restored from minimized or maximized.</summary>
        Restore = 1,
        /// <summary>The dialog was minimized.</summary>
        Minimize = 2,
        /// <summary>The dialog was maximized. Not received in Motif.</summary>
        Maximize = 3,
        /// <summary>The dialog was hidden.</summary>
        Hide = 4
    }

    /// <summary>
    /// Creates a dialog element. It manages user interaction with the interface
    /// elements. For any interface element to be shown, it must be encapsulated in a
    /// dialog. A dialog has only one child, but that child may be a container holding
    /// any number of further elements.
    /// </summary>
    /// <remarks>
    /// Do not associate a Dialog with the native "dialog" nomenclature in Windows, GTK
    /// or Motif. Dialog uses native standard windows in all drivers.
    /// </remarks>
    public class Dialog : ContainerControl
    {
        private Control _child;

        const string _destroyMessage = "IUPSHARP_DESTROY";

        private bool _destroyOnClose=false;

        /// <summary>
        /// Creates a new Dialog containing the specified child.
        /// </summary>
        /// <param name="child">
        /// The single child of the dialog. It can be null, in which case the dialog is
        /// created empty and a child can be added later with Append.
        /// </param>
        public Dialog(Control child)
            : base(IupNative.IupDialog(child == null ? IntPtr.Zero : child.Handle))
        {
            _child = child;

            // Registered unconditionally so DestroyOnClose works without the
            // application also setting CloseCB.
            _closeCBInternal = CloseCBInternal;
            SetCallback("CLOSE_CB", Utils.CastCallback<Icallback>(_closeCBInternal));
        }

        /// <summary>
        /// Creates a new empty Dialog. A child must be added with Append before the
        /// dialog is shown, otherwise it will have nothing to display.
        /// </summary>
        public Dialog() : this(null)
        {
        }

        /// <summary>
        /// Gets the single child of the dialog, or null if it has none.
        /// </summary>
        public Control Child => _child;

        /// <summary>
        /// Sets the single child of the dialog. A dialog accepts only one child; adding
        /// a second one is an error. Use a VBox or HBox to hold multiple controls.
        /// </summary>
        /// <exception cref="InvalidOperationException">The dialog already has a child.</exception>
        public override void Append(Control child)
        {
            if (_child != null)
                throw new InvalidOperationException(
                    "A Dialog accepts only one child. Wrap the controls in a container control such as VBox or HBox.");

            base.Append(child);
            _child = child;
        }

        protected override void OnDestroying()
        {
            _child = null;
            _iconReference = null;
            _trayImageReference = null;
            _menuReference = null;
            _defaultEnterReference = null;
            _defaultEscReference = null;
            _startFocusReference = null;
            base.OnDestroying();
        }

        #region SHOWING AND HIDING

        /// <summary>
        /// Displays the dialog in the current position, or in the default position if
        /// the dialog was not shown before. It will call Map for the element.
        /// </summary>
        /// <exception cref="IupException">The dialog could not be shown.</exception>
        public void Show()
        {
            CheckAlive();
            if (IupNative.IupShow(Handle) != IupNative.IUP_NOERROR)
                throw new IupException("Failed to show the dialog.");
        }

        /// <summary>
        /// Displays the dialog at the given screen position, in pixels. The special
        /// values in DialogPosition may also be used. It will call Map for the element.
        /// </summary>
        /// <param name="x">Horizontal position of the top-left corner, relative to the origin of the main screen.</param>
        /// <param name="y">Vertical position of the top-left corner, relative to the origin of the main screen.</param>
        /// <exception cref="IupException">The dialog could not be shown.</exception>
        public void ShowXY(int x, int y)
        {
            CheckAlive();
            if (IupNative.IupShowXY(Handle, x, y) != IupNative.IUP_NOERROR)
                throw new IupException("Failed to show the dialog.");
        }

        /// <summary>
        /// Shows the dialog as modal, inhibiting interaction with other dialogs, and
        /// does not return until a callback sets its Result to Close or the dialog is
        /// hidden. Although it interrupts processing, it does not destroy the dialog
        /// when it ends. Calling Popup for an already visible dialog will only update
        /// its position and will NOT change its modal state.
        /// </summary>
        /// <param name="x">Horizontal position. Defaults to centred on screen.</param>
        /// <param name="y">Vertical position. Defaults to centred on screen.</param>
        /// <exception cref="IupException">The dialog could not be shown.</exception>
        public void Popup(int x = IupNative.IUP_CENTER, int y = IupNative.IUP_CENTER)
        {
            CheckAlive();
            if (IupNative.IupPopup(Handle, x, y) != IupNative.IUP_NOERROR)
                throw new IupException("Failed to popup the dialog.");
        }

        /// <summary>
        /// Hides the dialog. Hiding the last visible dialog automatically ends the
        /// main loop.
        /// </summary>
        public void Hide()
        {
            CheckAlive();
            IupNative.IupHide(Handle);
        }

        

        #endregion

        #region APPEARANCE

        /// <summary>
        /// Gets or sets the dialog's title. Default: null. If you want to remove the
        /// title bar you must also set MenuBox, MaxBox and MinBox to false, before map.
        /// But in Motif and GTK it will hide it only if Resize is also false.
        /// (non inheritable)
        /// </summary>
        public string Title
        {
            get => GetAttribute("TITLE") ?? "";
            set => SetAttribute("TITLE", value);
        }

        /// <summary>
        /// Gets or sets the dialog's background color. Note that this will also affect
        /// all the controls inside the dialog.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; }

        private ImageRGBA _iconReference = null;
        /// <summary>
        /// Gets or sets the dialog's icon. The Windows SDK recommends that cursors and
        /// icons should be implemented as resources rather than created at run time.
        /// </summary>
        public virtual ImageRGBA Icon
        {
            set
            {
                CheckAlive();
                _iconReference = value;
                IupNative.SetAttributeHandle(Handle, "ICON", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _iconReference;
        }

        /// <summary>
        /// Gets or sets whether a resize border is shown around the dialog. Default: true.
        /// Border=false is useful only when Resize, MaxBox, MinBox and MenuBox are all
        /// false and Title is null; if any of these are defined there will always be
        /// some border.
        /// (creation only) (non inheritable)
        /// </summary>
        public virtual bool Border
        {
            get => GetAttribute("BORDER") != "NO";
            set => SetAttribute("BORDER", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets the border size in pixels.
        /// (read only)
        /// </summary>
        public int BorderSize
        {
            get { int.TryParse(GetAttribute("BORDERSIZE"), CultureInfo.InvariantCulture, out int size); return size; }
        }

        /// <summary>
        /// Gets or sets whether the dialog occupies the whole screen over any system
        /// bars in the main monitor. All dialog details, such as title bar, borders and
        /// the maximize button, are removed. In Motif you may have to click in the
        /// dialog to set its focus, and if set to true while the dialog is hidden it
        /// can not be changed after it becomes visible.
        /// </summary>
        public virtual bool FullScreen
        {
            get => GetAttribute("FULLSCREEN") == "YES";
            set => SetAttribute("FULLSCREEN", value ? "YES" : "NO");
        }

        static (string, Placement)[] _placements = new[]
        {
            ("NORMAL", Placement.Normal),
            ("MAXIMIZED", Placement.Maximized),
            ("MINIMIZED", Placement.Minimized),
            ("FULL", Placement.Full)
        };

        /// <summary>
        /// Gets or sets how the dialog will be shown. Default: Normal. After Show or
        /// Popup the attribute is set back to Normal.
        /// </summary>
        public virtual Placement Placement
        {
            get => Utils.MapAttrib(GetAttribute("PLACEMENT"), _placements);
            set => SetAttribute("PLACEMENT", Utils.MapEnum(value, _placements));
        }

        /// <summary>
        /// Gets or sets whether the dialog destroys itself once the user closes it.
        /// Default: false, matching IUP, where closing a dialog only hides it.
        ///
        /// <para>Set this for one-shot dialogs. For a dialog that will be shown again,
        /// leave it false and call Destroy when it is finally no longer needed.</para>
        ///
        /// <para>IMPORTANT: a dialog is never collected merely because the application
        /// drops its reference - IupSharp keeps every live element reachable until its
        /// native element is destroyed. A hidden dialog that is never destroyed leaks
        /// for the lifetime of the process.</para>
        ///
        /// <para>This does NOT fire when the dialog is destroyed as a side effect of
        /// its parent being destroyed: IUP does not call CloseCB in that case.
        /// Override OnDestroying for cleanup that must always run.</para>
        /// </summary>
        public bool DestroyOnClose
        {
            get => _destroyOnClose;
            set
            {
                _destroyOnClose = value;
            }
        }

        #endregion

        #region DECORATIONS

        /// <summary>
        /// Gets or sets whether a maximize button is requested from the window manager.
        /// If Resize is false then MaxBox will be set to false. Default: true. In Motif
        /// the decorations are controlled by the Window Manager and may not be possible
        /// to change from IUP. In Windows MaxBox is hidden only if MinBox is hidden as
        /// well, otherwise it will be just disabled.
        /// (creation only)
        /// </summary>
        public virtual bool MaxBox
        {
            get => GetAttribute("MAXBOX") != "NO";
            set => SetAttribute("MAXBOX", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether a minimize button is requested from the window manager.
        /// Default: true. In Motif the decorations are controlled by the Window Manager
        /// and may not be possible to change from IUP. In Windows MinBox is hidden only
        /// if MaxBox is hidden as well, otherwise it will be just disabled.
        /// (creation only)
        /// </summary>
        public virtual bool MinBox
        {
            get => GetAttribute("MINBOX") != "NO";
            set => SetAttribute("MINBOX", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether a system menu box is requested from the window manager.
        /// If hidden this will also remove the Close button. Default: true. In Motif the
        /// decorations are controlled by the Window Manager and may not be possible to
        /// change from IUP. In Windows if hidden it will hide MaxBox and MinBox too.
        /// (creation only)
        /// </summary>
        public virtual bool MenuBox
        {
            get => GetAttribute("MENUBOX") != "NO";
            set => SetAttribute("MENUBOX", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the dialog's size can be changed interactively.
        /// Default: true. If false then MaxBox will be set to false. In Motif the
        /// decorations are controlled by the Window Manager and may not be possible to
        /// change from IUP.
        /// (creation only)
        /// </summary>
        public virtual bool Resize
        {
            get => GetAttribute("RESIZE") != "NO";
            set => SetAttribute("RESIZE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Sets the common decorations for modal dialogs. This means Resize, MinBox and
        /// MaxBox all become false. In Windows, if ParentDialog is defined then the
        /// MenuBox is also removed, but the Close button remains.
        /// (write only)
        /// </summary>
        public bool DialogFrame
        {
            set => SetAttribute("DIALOGFRAME", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the dialog is kept in front of all other dialogs in all
        /// applications. Default: false.
        /// [Windows and GTK only]
        /// </summary>
        public virtual bool TopMost
        {
            get => GetAttribute("TOPMOST") == "YES";
            set => SetAttribute("TOPMOST", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the dialog transparency alpha value, from 0 (completely
        /// transparent) to 255 (opaque). In Windows this must be set before map so the
        /// native window is properly initialized when mapped.
        /// [Windows and GTK only]
        /// </summary>
        public virtual int Opacity
        {
            get { if (!int.TryParse(GetAttribute("OPACITY"), CultureInfo.InvariantCulture, out int o)) return 255; return o; }
            set => SetAttribute("OPACITY", value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region SIZE AND POSITION

       

        /// <summary>
        /// Gets the area available for controls, in pixels, excluding the decoration.
        /// </summary>
        public (int, int) ClientSize
        {
            get { IupNative.GetIntInt(Handle, "CLIENTSIZE", out int w, out int h); return (w, h); }
        }



        /// <summary>
        /// Gets or sets whether the elements' distribution may change when the dialog is
        /// smaller than the minimum size. Default: false.
        /// </summary>
        public virtual bool Shrink
        {
            get => GetAttribute("SHRINK") == "YES";
            set => SetAttribute("SHRINK", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets a position offset for the child, in pixels. It will not affect
        /// the natural size, and allows positioning controls outside the client area.
        /// Default: (0,0).
        /// </summary>
        public (int, int) ChildOffset
        {
            get { IupNative.GetIntInt(Handle, "CHILDOFFSET", out int x, out int y); return (x, y); }
            set => SetAttribute("CHILDOFFSET", Utils.FormatPadding(value));
        }

        #endregion

        #region STATE

        /// <summary>
        /// Gets whether the dialog was shown using Popup. It is false if Show was used
        /// or the dialog is not visible. At the first time the dialog is shown, this is
        /// not set yet when ShowCB is called.
        /// (read only)
        /// </summary>
        public bool Modal => GetAttribute("MODAL") == "YES";

        /// <summary>
        /// Gets whether the dialog is maximized.
        /// [Windows and GTK only] (read only)
        /// </summary>
        public bool Maximized => GetAttribute("MAXIMIZED") == "YES";

        /// <summary>
        /// Gets whether the dialog is minimized.
        /// [Windows and GTK only] (read only)
        /// </summary>
        public bool Minimized => GetAttribute("MINIMIZED") == "YES";

        /// <summary>
        /// Gets whether the dialog is the active window, that is, the window with focus.
        /// [Windows and GTK only] (read only)
        /// </summary>
        public bool ActiveWindow => GetAttribute("ACTIVEWINDOW") == "YES";

        /// <summary>
        /// Gets the native Windows window handle, or IntPtr.Zero if unavailable.
        /// Available in the Windows driver or in the GTK driver on Windows.
        /// [Windows only] (read only) (non inheritable)
        /// </summary>
        public IntPtr Hwnd
        {
            get
            {
                string wid = GetAttribute("HWND");
                if (string.IsNullOrEmpty(wid)) return IntPtr.Zero;
                return (IntPtr)long.Parse(wid, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region ASSOCIATED ELEMENTS

        private IupObject _menuReference = null;
        /// <summary>
        /// Gets or sets a menu associated with the dialog as a menu bar. The previous
        /// menu, if any, is unmapped.
        /// </summary>
        public virtual IupObject Menu
        {
            set
            {
                CheckAlive();
                _menuReference = value;
                IupNative.SetAttributeHandle(Handle, "MENU", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _menuReference;
        }

        private Control _defaultEnterReference = null;
        /// <summary>
        /// Gets or sets the button activated when the user presses Enter while focus is
        /// in another control of the dialog.
        /// </summary>
        public virtual Control DefaultEnter
        {
            set
            {
                CheckAlive();
                _defaultEnterReference = value;
                IupNative.SetAttributeHandle(Handle, "DEFAULTENTER", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _defaultEnterReference;
        }

        private Control _defaultEscReference = null;
        /// <summary>
        /// Gets or sets the button activated when the user presses Esc while focus is in
        /// another control of the dialog.
        /// </summary>
        public virtual Control DefaultEsc
        {
            set
            {
                CheckAlive();
                _defaultEscReference = value;
                IupNative.SetAttributeHandle(Handle, "DEFAULTESC", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _defaultEscReference;
        }

        private Control _startFocusReference = null;
        /// <summary>
        /// Gets or sets the element that must receive the focus right after the dialog
        /// is shown. If not defined then the first control that can receive the focus is
        /// selected. Updated after ShowCB is called, and only if the focus was not
        /// changed during the callback.
        /// </summary>
        public virtual Control StartFocus
        {
            set
            {
                CheckAlive();
                _startFocusReference = value;
                IupNative.SetAttributeHandle(Handle, "STARTFOCUS", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _startFocusReference;
        }

        /// <summary>
        /// Gets or sets whether focus is set after the dialog is shown. Default: false.
        /// </summary>
        public virtual bool ShowNoFocus
        {
            get => GetAttribute("SHOWNOFOCUS") == "YES";
            set => SetAttribute("SHOWNOFOCUS", value ? "YES" : "NO");
        }

        #endregion

        #region TRAY

        /// <summary>
        /// Gets or sets whether an icon is displayed on the system tray.
        /// [Windows and GTK only]
        /// </summary>
        public virtual bool Tray
        {
            get => GetAttribute("TRAY") == "YES";
            set => SetAttribute("TRAY", value ? "YES" : "NO");
        }

        private ImageRGBA _trayImageReference = null;
        /// <summary>
        /// Gets or sets the image used as the tray icon.
        /// [Windows and GTK only]
        /// </summary>
        public virtual ImageRGBA TrayImage
        {
            set
            {
                CheckAlive();
                _trayImageReference = value;
                IupNative.SetAttributeHandle(Handle, "TRAYIMAGE", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _trayImageReference;
        }

        /// <summary>
        /// Gets or sets the tray icon's tooltip text.
        /// [Windows and GTK only]
        /// </summary>
        public virtual string TrayTip
        {
            get => GetAttribute("TRAYTIP");
            set => SetAttribute("TRAYTIP", value);
        }

        /// <summary>
        /// Hides the dialog without decrementing the visible dialog count, without
        /// calling ShowCB and without marking the dialog as hidden inside IUP. Usually
        /// used to hide the dialog and keep the tray icon working without closing the
        /// main loop. IMPORTANT: when hidden this way, it must be shown this way too.
        /// [Windows and GTK only] (write only)
        /// </summary>
        public bool HideTaskbar
        {
            set => SetAttribute("HIDETASKBAR", value ? "YES" : "NO");
        }

        #endregion

        protected override bool OnPostMessage(string text, int value, double number, IntPtr pointer)
        {
            if (text == _destroyMessage) { 
                Destroy(); 
                return true; 
            }
            return base.OnPostMessage(text, value, number, pointer);
        }

        #region CALLBACKS

        private Callback _closeCB;
        private IFn _closeCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated right before the dialog is closed. Set the
        /// callback data's Result to Ignore to prevent the dialog from closing, or to
        /// Close to also end the main loop. Returning the default value hides the dialog,
        /// but does not destroy it.
        /// </summary>
        public Callback CloseCB
        {
            get => _closeCB;
            set
            {
                _closeCB = value;
            }
        }
        private int CloseCBInternal(nint ih)
        {
            CallbackResult result;

            try
            {
                var cb = new CallbackData(this);
                _closeCB?.Invoke(cb);
                result = cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in CloseCB callback: {ex}");
                return (int)CallbackResult.Ignore;   // don't close a dialog whose handler failed
            }

            if (DestroyOnClose && result != CallbackResult.Ignore && Handle != IntPtr.Zero)
                PostMessageInternal(_destroyMessage);   // deferred: IUP still uses the element here

            return (int)result;
        }
        private ShowCBCallback _showCB;
        private IFni _showCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated right after the dialog is shown, hidden,
        /// maximized, minimized, or restored from minimized or maximized. When the
        /// dialog is shown for the first time the order relative to ResizeCB is system
        /// dependent.
        /// </summary>
        public ShowCBCallback ShowCB
        {
            get => _showCB;
            set
            {
                _showCB = value;
                _showCBInternal = ShowCBInternal;
                SetCallback("SHOW_CB", Utils.CastCallback<Icallback>(_showCBInternal));
            }
        }
        private int ShowCBInternal(nint ih, int state)
        {
            try
            {
                var cb = new ShowCBData(this, state);
                _showCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ShowCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private ResizeCBCallback _resizeCB;
        private IFnii _resizeCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the dialog size is changed. Set the
        /// callback data's Result to Ignore to prevent the dialog layout from being
        /// recalculated.
        /// </summary>
        public ResizeCBCallback ResizeCB
        {
            get => _resizeCB;
            set
            {
                _resizeCB = value;
                _resizeCBInternal = ResizeCBInternal;
                SetCallback("RESIZE_CB", Utils.CastCallback<Icallback>(_resizeCBInternal));
            }
        }
        private int ResizeCBInternal(nint ih, int width, int height)
        {
            try
            {
                var cb = new ResizeCBData(this, width, height);
                _resizeCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ResizeCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private MoveCBCallback _moveCB;
        private IFnii _moveCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated after the dialog was moved on screen. The
        /// coordinates are the same as the ScreenPosition attribute.
        /// [Windows and GTK only]
        /// </summary>
        public MoveCBCallback MoveCB
        {
            get => _moveCB;
            set
            {
                _moveCB = value;
                _moveCBInternal = MoveCBInternal;
                SetCallback("MOVE_CB", Utils.CastCallback<Icallback>(_moveCBInternal));
            }
        }
        private int MoveCBInternal(nint ih, int x, int y)
        {
            try
            {
                var cb = new MoveCBData(this, x, y);
                _moveCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in MoveCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private TrayClickCBCallback _trayClickCB;
        private IFniii _trayClickCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated right after a mouse button is pressed or
        /// released over the tray icon. GTK does not report the middle button, and
        /// Pressed is always true in GTK. Setting the callback data's Result to Close
        /// will be processed.
        /// [Windows and GTK only]
        /// </summary>
        public TrayClickCBCallback TrayClickCB
        {
            get => _trayClickCB;
            set
            {
                _trayClickCB = value;
                _trayClickCBInternal = TrayClickCBInternal;
                SetCallback("TRAYCLICK_CB", Utils.CastCallback<Icallback>(_trayClickCBInternal));
            }
        }
        private int TrayClickCBInternal(nint ih, int but, int pressed, int dclick)
        {
            try
            {
                var cb = new TrayClickCBData(this, but, pressed, dclick);
                _trayClickCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in TrayClickCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private DropFilesCBCallback _dropFilesCB;
        private IFnsiii _dropFilesCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when one or more files are dropped on the
        /// dialog. Setting DropFilesTarget is not needed on a dialog; defining this
        /// callback before map enables it automatically.
        /// [Windows and GTK only]
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