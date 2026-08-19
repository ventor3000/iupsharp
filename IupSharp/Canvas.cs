using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates an interface element that is a canvas - a drawing area for the
    /// application.
    /// </summary>
    /// <remarks>
    /// <para>The Action callback can NOT be invoked manually from the application; it
    /// must be triggered by the system. Call Redraw or Update to request a repaint.</para>
    /// <para>When the canvas is displayed for the first time the callback order is
    /// always MapCB, then ResizeCB, then Action. When the canvas is resized, Action is
    /// always called after ResizeCB.</para>
    /// <para>In GTK this uses GtkFixed, in Windows a custom window class called
    /// "IupCanvas", and in Motif xmDrawingArea.</para>
    /// </remarks>
    public class Canvas : Control
    {
        /// <summary>
        /// Creates a new Canvas.
        /// </summary>
        public Canvas() : base(IupNative.Canvas(null))
        {
        }

        #region APPEARANCE

        /// <summary>
        /// Gets or sets the background color. The background is painted only if the
        /// Action callback is not defined; if it is, the application must draw all the
        /// canvas contents. In GTK and Motif, if Action is set after map then BgColor
        /// should also be set to any value just after setting the callback, or the
        /// first redraw will be lost. Default: white.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; }

        /// <summary>
        /// Gets or sets whether a border is shown around the canvas. Default: true.
        /// (creation only)
        /// </summary>
        public virtual bool Border
        {
            get => GetAttribute("BORDER") != "NO";
            set => SetAttribute("BORDER", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the name of the cursor for the canvas. The Windows SDK
        /// recommends that cursors and icons be implemented as resources rather than
        /// created at run time.
        /// (non inheritable)
        /// </summary>
        public virtual string Cursor
        {
            get => GetAttribute("CURSOR");
            set => SetAttribute("CURSOR", value);
        }

        /// <summary>
        /// Gets the size of the drawing area in pixels. This is the size also passed to
        /// the ResizeCB callback, and is NOT the same as RasterSize - the ScrollBar and
        /// Border attributes both affect it.
        /// (non inheritable)
        /// </summary>
        public (int, int) DrawSize
        {
            get { IupNative.GetIntInt(Handle, "DRAWSIZE", out int w, out int h); return (w, h); }
        }

        /// <summary>
        /// Gets the name of the draw driver in use by the IupDraw API. Can be X11
        /// (Motif), GDK or Cairo (GTK), or D2D, GDI+ or GDI (Windows).
        /// (read only) (since 3.25)
        /// </summary>
        public string DrawDriver => GetAttribute("DRAWDRIVER");

        #endregion

        #region FOCUS AND INPUT

        /// <summary>
        /// Gets or sets whether focus traversal of the control is enabled. In Windows
        /// the canvas will respect CanFocus differently to some other controls.
        /// Default: true.
        /// (creation only) (non inheritable)
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

        /// <summary>
        /// Gets or sets whether dropping files onto the canvas is enabled. Default:
        /// false, but if DropFilesCB is defined when the element is mapped then it is
        /// enabled automatically.
        /// [Windows and GTK only] (non inheritable)
        /// </summary>
        public virtual bool DropFilesTarget
        {
            get => GetAttribute("DROPFILESTARGET") == "YES";
            set => SetAttribute("DROPFILESTARGET", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether touch processing is enabled, if touch support is
        /// available. Must be true to receive TouchCB and MultiTouchCB.
        /// [Windows only] (since 3.3)
        /// </summary>
        public virtual bool Touch
        {
            get => GetAttribute("TOUCH") == "YES";
            set => SetAttribute("TOUCH", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the control with the focus receives SHOWDROPDOWN=No
        /// when the wheel is used. Default: false.
        /// (non inheritable) (since 3.28)
        /// </summary>
        public virtual bool WheelDropFocus
        {
            get => GetAttribute("WHEELDROPFOCUS") == "YES";
            set => SetAttribute("WHEELDROPFOCUS", value ? "YES" : "NO");
        }

        #endregion

        #region SCROLLBAR

        static readonly (string, ScrollBars)[] _scrollBars = new[]
        {
            ("NO", ScrollBars.No),
            ("HORIZONTAL", ScrollBars.Horizontal),
            ("VERTICAL", ScrollBars.Vertical),
            ("YES", ScrollBars.Both)
        };

        /// <summary>
        /// Gets or sets which scrollbars are associated with the canvas.
        /// Default: No. The secondary scrollbar attributes are all non inheritable.
        /// (creation only)
        /// </summary>
        public virtual ScrollBars ScrollBar
        {
            get => Utils.MapAttrib(GetAttribute("SCROLLBAR"), _scrollBars);
            set => SetAttribute("SCROLLBAR", Utils.MapEnum(value, _scrollBars));
        }

        /// <summary>
        /// Gets which scrollbars are visible at the moment.
        /// [Windows only] (read only) (since 3.31)
        /// </summary>
        public ScrollBars ScrollVisible =>
            Utils.MapAttrib(GetAttribute("SCROLLVISIBLE"), _scrollBars);

        /// <summary>
        /// Gets or sets the size of the thumb in the horizontal scrollbar, which is
        /// also the horizontal page size. Default: 0.1.
        /// </summary>
        /// <remarks>
        /// Setting this is what refreshes LineX, XMin and XMax in the scrollbar, so
        /// assign Dx last when updating several of them together.
        /// </remarks>
        public double Dx
        {
            get => GetDouble("DX", 0.1);
            set => SetDouble("DX", value);
        }

        /// <summary>
        /// Gets or sets the size of the thumb in the vertical scrollbar, which is also
        /// the vertical page size. Default: 0.1.
        /// </summary>
        /// <remarks>
        /// Setting this is what refreshes LineY, YMin and YMax in the scrollbar, so
        /// assign Dy last when updating several of them together.
        /// </remarks>
        public double Dy
        {
            get => GetDouble("DY", 0.1);
            set => SetDouble("DY", value);
        }

        /// <summary>
        /// Gets or sets the position of the thumb in the horizontal scrollbar.
        /// Default: 0.0. Limited to the range XMin to XMax - Dx.
        /// </summary>
        public double PosX
        {
            get => GetDouble("POSX", 0.0);
            set => SetDouble("POSX", value);
        }

        /// <summary>
        /// Gets or sets the position of the thumb in the vertical scrollbar.
        /// Default: 0.0. Limited to the range YMin to YMax - Dy.
        /// </summary>
        public double PosY
        {
            get => GetDouble("POSY", 0.0);
            set => SetDouble("POSY", value);
        }

        /// <summary>
        /// Gets or sets the minimum value of the horizontal scrollbar. Default: 0.0.
        /// </summary>
        public double XMin
        {
            get => GetDouble("XMIN", 0.0);
            set => SetDouble("XMIN", value);
        }

        /// <summary>
        /// Gets or sets the maximum value of the horizontal scrollbar. Default: 1.0.
        /// </summary>
        /// <remarks>
        /// When working in a virtual space with integer coordinates, set this to the
        /// size of that space and NOT to width - 1, or the last pixel will never be
        /// visible.
        /// </remarks>
        public double XMax
        {
            get => GetDouble("XMAX", 1.0);
            set => SetDouble("XMAX", value);
        }

        /// <summary>
        /// Gets or sets the minimum value of the vertical scrollbar. Default: 0.0.
        /// </summary>
        public double YMin
        {
            get => GetDouble("YMIN", 0.0);
            set => SetDouble("YMIN", value);
        }

        /// <summary>
        /// Gets or sets the maximum value of the vertical scrollbar. Default: 1.0.
        /// </summary>
        public double YMax
        {
            get => GetDouble("YMAX", 1.0);
            set => SetDouble("YMAX", value);
        }

        /// <summary>
        /// Gets or sets how far the thumb moves when a horizontal step is performed.
        /// Default: one tenth of Dx.
        /// (since 3.0)
        /// </summary>
        public double LineX
        {
            get => GetDouble("LINEX", Dx / 10.0);
            set => SetDouble("LINEX", value);
        }

        /// <summary>
        /// Gets or sets how far the thumb moves when a vertical step is performed.
        /// Default: one tenth of Dy.
        /// (since 3.0)
        /// </summary>
        public double LineY
        {
            get => GetDouble("LINEY", Dy / 10.0);
            set => SetDouble("LINEY", value);
        }

        /// <summary>
        /// Gets or sets whether the horizontal scrollbar is hidden when Dx is greater
        /// than or equal to XMax - XMin. Default: true.
        /// (since 3.0)
        /// </summary>
        public virtual bool XAutoHide
        {
            get => GetAttribute("XAUTOHIDE") != "NO";
            set => SetAttribute("XAUTOHIDE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the vertical scrollbar is hidden when Dy is greater
        /// than or equal to YMax - YMin. Default: true.
        /// (since 3.0)
        /// </summary>
        public virtual bool YAutoHide
        {
            get => GetAttribute("YAUTOHIDE") != "NO";
            set => SetAttribute("YAUTOHIDE", value ? "YES" : "NO");
        }

        #endregion

        #region PLATFORM SPECIFIC

        /// <summary>
        /// Gets or sets the canvas backing store flag. Default: true.
        /// [Motif only]
        /// </summary>
        public virtual bool BackingStore
        {
            get => GetAttribute("BACKINGSTORE") != "NO";
            set => SetAttribute("BACKINGSTORE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether to force the old GDI driver instead of the newer
        /// Direct2D driver. Used internally by IupGauge, IupMatrix and the flat
        /// scrollbars for performance and backward compatibility.
        /// [Windows only] (non inheritable) (since 3.26)
        /// </summary>
        public virtual bool DrawUseGDI
        {
            get => GetAttribute("DRAWUSEGDI") == "YES";
            set => SetAttribute("DRAWUSEGDI", value ? "YES" : "NO");
        }

        /// <summary>
        /// Disables gesture support for touch interfaces. Only false is accepted;
        /// setting true does nothing.
        /// [Windows only] (write only) (since 3.31)
        /// </summary>
        public bool Gesture
        {
            set { if (!value) SetAttribute("GESTURE", "NO"); }
        }

        /// <summary>
        /// Gets the rectangle whose region has been invalidated for painting, as
        /// (x1, y1, x2, y2). Useful for clipping. Valid only during the Action
        /// callback; returns all zeros otherwise.
        /// [Windows and GTK only] (read only)
        /// </summary>
        public (int, int, int, int) ClipRect
        {
            get
            {
                string v = GetAttribute("CLIPRECT");
                if (string.IsNullOrWhiteSpace(v))
                    return (0, 0, 0, 0);

                string[] p = v.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 4)
                    return (0, 0, 0, 0);

                int.TryParse(p[0], CultureInfo.InvariantCulture, out int x1);
                int.TryParse(p[1], CultureInfo.InvariantCulture, out int y1);
                int.TryParse(p[2], CultureInfo.InvariantCulture, out int x2);
                int.TryParse(p[3], CultureInfo.InvariantCulture, out int y2);
                return (x1, y1, x2, y2);
            }
        }

        /// <summary>
        /// Gets the HDC created by BeginPaint inside the WM_PAINT message. Valid only
        /// during the Action callback.
        /// [Windows only] (read only) (non inheritable)
        /// </summary>
        public IntPtr HdcWmPaint => GetAttributePtr("HDC_WMPAINT");

        /// <summary>
        /// Gets the cairo_t* of the internal GTK callback. Valid only during the Action
        /// callback, and only when using GTK 3.
        /// [GTK only] (read only) (non inheritable) (since 3.7)
        /// </summary>
        public IntPtr CairoCr => GetAttributePtr("CAIRO_CR");

        /// <summary>
        /// Gets the Windows window handle. Available in the Windows driver, or in the
        /// GTK driver on Windows.
        /// [Windows only] (read only) (non inheritable)
        /// </summary>
        public IntPtr Hwnd => GetAttributePtr("HWND");

        /// <summary>
        /// Gets the X-Windows Display. Available in the Motif driver, or in the GTK
        /// driver on UNIX.
        /// [UNIX only] (read only) (non inheritable)
        /// </summary>
        public IntPtr XDisplay => GetAttributePtr("XDISPLAY");

        /// <summary>
        /// Gets the X-Windows Window (Drawable). Available in the Motif driver, or in
        /// the GTK driver on UNIX.
        /// [UNIX only] (read only) (non inheritable)
        /// </summary>
        public IntPtr XWindow => GetAttributePtr("XWINDOW");

        #endregion

        #region HELPERS

        private double GetDouble(string name, double fallback)
        {
            string v = GetAttribute(name);
            if (string.IsNullOrWhiteSpace(v))
                return fallback;

            return double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)
                ? d
                : fallback;
        }

        private void SetDouble(string name, double value) =>
            SetAttribute(name, value.ToString("R", CultureInfo.InvariantCulture));

        #endregion

        #region CALLBACKS

        private CanvasActionCallback _action;
        private IFnff _actionInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the canvas needs to be redrawn. This
        /// cannot be invoked directly from the application; call Redraw or Update to
        /// request a repaint and let the system call it.
        /// </summary>
        public CanvasActionCallback Action
        {
            get => _action;
            set
            {
                _action = value;
                _actionInternal = ActionInternal;
                SetCallback("ACTION", Utils.CastCallback<Icallback>(_actionInternal));
            }
        }
        private int ActionInternal(nint ih, float posx, float posy)
        {
            try
            {
                var cb = new CanvasActionData(this, posx, posy);
                _action?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in Action callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private ButtonCBCallback _buttonCB;
        private IFniiiis _buttonCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when any mouse button is pressed or
        /// released.
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
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ButtonCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private MotionCBCallback _motionCB;
        private IFniis _motionCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse is moved over the canvas.
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
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in MotionCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private ResizeCBCallback _resizeCB;
        private IFnii _resizeCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the canvas size is changed. The size
        /// reported is the drawing area size, the same as DrawSize. Action is always
        /// called after this callback.
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
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ResizeCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private ScrollCBCallback _scrollCB;
        private IFniff _scrollCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the scrollbar is manipulated. If this
        /// callback is defined the canvas must be redrawn manually - call Update or
        /// Redraw. If it is not defined the canvas is redrawn automatically.
        /// </summary>
        /// <remarks>
        /// Requires GTK 2.8 or later; on older GTK versions PosX and PosY will not be
        /// updated correctly.
        /// </remarks>
        public ScrollCBCallback ScrollCB
        {
            get => _scrollCB;
            set
            {
                _scrollCB = value;
                _scrollCBInternal = ScrollCBInternal;
                SetCallback("SCROLL_CB", Utils.CastCallback<Icallback>(_scrollCBInternal));
            }
        }
        private int ScrollCBInternal(nint ih, int op, float posx, float posy)
        {
            try
            {
                var cb = new ScrollCBData(this, op, posx, posy);
                _scrollCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ScrollCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private FocusCBCallback _focusCB;
        private IFni _focusCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when the canvas gains or loses the focus. It
        /// is called after the common GetFocusCB and KillFocusCB callbacks.
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
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in FocusCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private KeyPressCBCallback _keyPressCB;
        private IFnii _keyPressCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when a key is pressed or released. It is
        /// called after the common KAny callback.
        /// </summary>
        /// <remarks>
        /// When the canvas has the focus, pressing the arrow keys may move the focus to
        /// another control on some systems. If the handler processes the arrow keys,
        /// set the callback data's Result to Ignore so the canvas keeps the focus.
        /// </remarks>
        public KeyPressCBCallback KeyPressCB
        {
            get => _keyPressCB;
            set
            {
                _keyPressCB = value;
                _keyPressCBInternal = KeyPressCBInternal;
                SetCallback("KEYPRESS_CB", Utils.CastCallback<Icallback>(_keyPressCBInternal));
            }
        }
        private int KeyPressCBInternal(nint ih, int c, int press)
        {
            try
            {
                var cb = new KeyPressCBData(this, c, press);
                _keyPressCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in KeyPressCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private WheelCBCallback _wheelCB;
        private IFnfiis _wheelCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse wheel is rotated.
        /// </summary>
        public WheelCBCallback WheelCB
        {
            get => _wheelCB;
            set
            {
                _wheelCB = value;
                _wheelCBInternal = WheelCBInternal;
                SetCallback("WHEEL_CB", Utils.CastCallback<Icallback>(_wheelCBInternal));
            }
        }
        private int WheelCBInternal(nint ih, float delta, int x, int y, string status)
        {
            try
            {
                var cb = new WheelCBData(this, delta, x, y, status);
                _wheelCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in WheelCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private DropFilesCBCallback _dropFilesCB;
        private IFnsiii _dropFilesCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when one or more files are dropped on the
        /// canvas. Defining this before the element is mapped enables DropFilesTarget
        /// automatically.
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
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in DropFilesCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private TouchCBCallback _touchCB;
        private IFniiis _touchCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when a touch event occurs. Multiple touch
        /// events trigger several calls. Touch must be true to receive this.
        /// Setting the callback data's Result to Close will be processed.
        /// [Windows only] (since 3.3)
        /// </summary>
        public TouchCBCallback TouchCB
        {
            get => _touchCB;
            set
            {
                _touchCB = value;
                _touchCBInternal = TouchCBInternal;
                SetCallback("TOUCH_CB", Utils.CastCallback<Icallback>(_touchCBInternal));
            }
        }
        private int TouchCBInternal(nint ih, int id, int x, int y, string state)
        {
            try
            {
                var cb = new TouchCBData(this, id, x, y, state);
                _touchCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in TouchCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private MultiTouchCBCallback _multiTouchCB;
        private IFniIIII _multiTouchCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when multiple touch events occur. Touch
        /// must be true to receive this. Setting the callback data's Result to Close
        /// will be processed.
        /// [Windows only] (since 3.3)
        /// </summary>
        public MultiTouchCBCallback MultiTouchCB
        {
            get => _multiTouchCB;
            set
            {
                _multiTouchCB = value;
                _multiTouchCBInternal = MultiTouchCBInternal;
                SetCallback("MULTITOUCH_CB", Utils.CastCallback<Icallback>(_multiTouchCBInternal));
            }
        }
        private int MultiTouchCBInternal(nint ih, int count, IntPtr pid, IntPtr px, IntPtr py, IntPtr pstate)
        {
            try
            {
                var cb = new MultiTouchCBData(this, count, pid, px, py, pstate);
                _multiTouchCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in MultiTouchCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        private WomCBCallback _womCB;
        private IFni _womCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when an audio device receives an event.
        /// [Windows only]
        /// </summary>
        public WomCBCallback WomCB
        {
            get => _womCB;
            set
            {
                _womCB = value;
                _womCBInternal = WomCBInternal;
                SetCallback("WOM_CB", Utils.CastCallback<Icallback>(_womCBInternal));
            }
        }
        private int WomCBInternal(nint ih, int state)
        {
            try
            {
                var cb = new WomCBData(this, state);
                _womCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in WomCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }

        #endregion
    }
}