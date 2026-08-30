using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    public class Control : MappableObject
    {

        public Control(nint handle) : base(handle)
        {

            // Registered unconditionally: the library's own features depend on posted
            // messages arriving, and a feature that forgets to register fails silently.
            _postMessageCBInternal = PostMessageCBInternal;
            SetCallback("POSTMESSAGE_CB", Utils.CastCallback<Icallback>(_postMessageCBInternal));
        }



        public virtual Color BgColor
        {
            get => Utils.ParseColor(GetAttribute("BGCOLOR"));
            set => SetAttribute("BGCOLOR", Utils.FormatColor(value));
        }

        public virtual Color FgColor
        {
            get => Utils.ParseColor(GetAttribute("FGCOLOR"));
            set => SetAttribute("FGCOLOR", Utils.FormatColor(value));
        }

        // (attributes meaningful only when this control is a child of a VBox/HBox/Zbox; harmless no-ops otherwise)
        #region BOX LAYOUT 

        /// <summary>
        /// Gets or sets whether this control's size and position are ignored by the
        /// layout processing of its parent VBox/HBox/Zbox. Default: false.
        /// (non inheritable) (at children only)
        /// </summary>
        public virtual bool Floating
        {
            get => GetAttribute("FLOATING") == "YES";
            set => SetAttribute("FLOATING", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the expand weight used to multiply the free space this
        /// control receives when its parent VBox/HBox distributes extra space
        /// among expandable children. Default: 1 (native default when unset).
        /// (non inheritable) (at children only)
        /// </summary>
        public virtual double ExpandWeight
        {
            get
            {
                double.TryParse(GetAttribute("EXPANDWEIGHT"), NumberStyles.Float, CultureInfo.InvariantCulture, out double w);
                return w;
            }
            set => SetAttribute("EXPANDWEIGHT", value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region STATE

        /// <summary>
        /// Gets or sets whether the element is active. When inactive the element does
        /// not interact with the user and its visual presentation is changed to show
        /// that. Default: true. (inheritable)
        /// </summary>
        public virtual bool Active
        {
            get => GetAttribute("ACTIVE") != "NO";
            set => SetAttribute("ACTIVE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the element is visible. An element is only actually
        /// shown if its dialog is also visible. Default: true. Note that Dialog
        /// overrides this: for a dialog, setting it simply calls Show or Hide.
        /// (inheritable)
        /// </summary>
        public virtual bool Visible
        {
            get => GetAttribute("VISIBLE") != "NO";
            set => SetAttribute("VISIBLE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets the native handle of the element (HWND in Windows, X-Windows Drawable
        /// in Motif, GdkWindow in GTK), or IntPtr.Zero if the element is not mapped or
        /// has no native representation.
        /// (read only) (non inheritable)
        /// </summary>
        public IntPtr Wid
        {
            get
            {
                string wid = GetAttribute("WID");
                if (string.IsNullOrEmpty(wid))
                    return IntPtr.Zero;

                // IUP formats pointer attributes with %p: hex, usually with no prefix.
                if (wid.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    wid = wid.Substring(2);

                if (long.TryParse(wid, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long v))
                    return (IntPtr)v;

                return IntPtr.Zero;
            }
        }

        #endregion

        #region LAYOUT

        private static readonly (string, Expand)[] _expandModes =
        {
            ("NO", Expand.No),
            ("YES", Expand.Yes),
            ("HORIZONTAL", Expand.Horizontal),
            ("VERTICAL", Expand.Vertical),
            ("HORIZONTALFREE", Expand.HorizontalFree),
            ("VERTICALFREE", Expand.VerticalFree)
        };

        /// <summary>
        /// Gets or sets whether the element will expand to occupy empty space in the
        /// box that contains it. Default: No for most controls, Yes for containers and
        /// dialogs. (inheritable)
        /// </summary>
        public virtual Expand Expand
        {
            get => Utils.MapAttrib(GetAttribute("EXPAND"), _expandModes);
            set => SetAttribute("EXPAND", Utils.MapEnum(value, _expandModes));
        }

        /// <summary>
        /// Gets or sets the element size in characters, which depends on the current
        /// font. Set to (0,0) to remove the user size so the natural size is used.
        /// See also RasterSize for a size in pixels.
        /// (non inheritable)
        /// </summary>
        public (int, int) Size
        {
            get { IupNative.GetIntInt(Handle, "SIZE", out int w, out int h); return (w, h); }
            set => SetAttribute("SIZE", value == (0, 0) ? null : Utils.FormatSize(value));
        }

        /// <summary>
        /// Gets or sets the element size in pixels. Set to (0,0) to remove the user
        /// size so the natural size is used.
        /// (non inheritable)
        /// </summary>
        public (int x, int y) RasterSize
        {
            get { IupNative.GetIntInt(Handle, "RASTERSIZE", out int w, out int h); return (w, h); }
            set => SetAttribute("RASTERSIZE", value == (0, 0) ? null : Utils.FormatSize(value));
        }

        /// <summary>
        /// Gets or sets the minimum size of the element in pixels. The layout will not
        /// shrink the element below this. Default: (1,1).
        /// </summary>
        public (int, int) MinSize
        {
            get { IupNative.GetIntInt(Handle, "MINSIZE", out int w, out int h); return (w, h); }
            set => SetAttribute("MINSIZE", Utils.FormatSize(value));
        }

        /// <summary>
        /// Gets or sets the maximum size of the element in pixels. The layout will not
        /// grow the element beyond this. Default: (65535,65535).
        /// </summary>
        public (int, int) MaxSize
        {
            get { IupNative.GetIntInt(Handle, "MAXSIZE", out int w, out int h); return (w, h); }
            set => SetAttribute("MAXSIZE", Utils.FormatSize(value));
        }

        /// <summary>
        /// Gets the element position in pixels relative to the client area of the
        /// dialog. Setting it is normally done by the layout, not by the application.
        /// (non inheritable)
        /// </summary>
        public (int, int) Position
        {
            get { IupNative.GetIntInt(Handle, "POSITION", out int x, out int y); return (x, y); }
            set => SetAttribute("POSITION", Utils.FormatSize(value));
        }

        /// <summary>
        /// Gets the element position in pixels relative to the origin of the main
        /// screen.
        /// (read only) (non inheritable)
        /// </summary>
        public (int, int) ScreenPosition
        {
            get { IupNative.GetIntInt(Handle, "SCREENPOSITION", out int x, out int y); return (x, y); }
        }

        /// <summary>
        /// Gets the element horizontal position in pixels relative to the client area
        /// of the dialog.
        /// (read only) (non inheritable)
        /// </summary>
        public int X => IupNative.GetInt(Handle, "X");

        /// <summary>
        /// Gets the element vertical position in pixels relative to the client area of
        /// the dialog.
        /// (read only) (non inheritable)
        /// </summary>
        public int Y => IupNative.GetInt(Handle, "Y");

        /// <summary>
        /// Sets the position of the element in the Z order, relative to its siblings.
        /// Use ZOrder.Top to bring it to the front, ZOrder.Bottom to send it to the
        /// back. Only meaningful for elements that overlap.
        /// (write only) (non inheritable)
        /// </summary>
        public ZOrder ZOrder
        {
            set => SetAttribute("ZORDER", value == ZOrder.Top ? "TOP" : "BOTTOM");
        }

        #endregion

        #region FONT

        /// <summary>
        /// Gets or sets the font used to render text in the element, in IUP's common
        /// format: "Face, Style Size", for example "Helvetica, Bold 12". Style can
        /// combine Bold, Italic, Underline and Strikeout. A negative size is in pixels,
        /// a positive size is in points. For portable face names prefer Courier, Times
        /// or Helvetica, which IUP maps to the native equivalent on each system.
        /// (inheritable)
        /// </summary>
        public virtual string Font
        {
            get => GetAttribute("FONT");
            set => SetAttribute("FONT", value);
        }

        /// <summary>
        /// Gets or sets only the face part of the Font attribute. Setting it parses,
        /// changes and updates Font.
        /// </summary>
        public string FontFace
        {
            get => GetAttribute("FONTFACE");
            set => SetAttribute("FONTFACE", value);
        }

        /// <summary>
        /// Gets or sets only the size part of the Font attribute. Setting it parses,
        /// changes and updates Font.
        /// </summary>
        public int FontSize
        {
            get { int.TryParse(GetAttribute("FONTSIZE"), CultureInfo.InvariantCulture, out int s); return s; }
            set => SetAttribute("FONTSIZE", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets only the style part of the Font attribute, for example
        /// "Bold Italic". Setting it parses, changes and updates Font.
        /// </summary>
        public string FontStyle
        {
            get => GetAttribute("FONTSTYLE");
            set => SetAttribute("FONTSTYLE", value);
        }

        #endregion

        #region TIP

        /// <summary>
        /// Gets or sets the text of a tip (tooltip) shown when the mouse rests over the
        /// element. Set to null to remove it. Use '\n' for multiple lines.
        /// (non inheritable)
        /// </summary>
        public virtual string Tip
        {
            get => GetAttribute("TIP");
            set => SetAttribute("TIP", value);
        }

        /// <summary>
        /// Gets or sets the tip background color. Default: light yellow (255,255,225).
        /// [Windows and Motif only]
        /// </summary>
        public Color TipBgColor
        {
            get => Utils.ParseColor(GetAttribute("TIPBGCOLOR"));
            set => SetAttribute("TIPBGCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the tip text color. Default: black.
        /// [Windows and Motif only]
        /// </summary>
        public Color TipFgColor
        {
            get => Utils.ParseColor(GetAttribute("TIPFGCOLOR"));
            set => SetAttribute("TIPFGCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the time in milliseconds the tip remains visible.
        /// Default: 5000. In Windows the maximum is 32767.
        /// [Windows and Motif only]
        /// </summary>
        public int TipDelay
        {
            get { if (!int.TryParse(GetAttribute("TIPDELAY"), CultureInfo.InvariantCulture, out int d)) return 5000; return d; }
            set => SetAttribute("TIPDELAY", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the font used for the tip text. If not defined the element's
        /// Font is used. The value "SYSTEM" selects the default system tip font.
        /// [Windows and Motif only]
        /// </summary>
        public string TipFont
        {
            get => GetAttribute("TIPFONT");
            set => SetAttribute("TIPFONT", value);
        }

        /// <summary>
        /// Shows or hides the tip under the mouse cursor. In GTK this toggles the tip
        /// state and the given value is ignored.
        /// </summary>
        public bool TipVisible
        {
            get => GetAttribute("TIPVISIBLE") == "YES";
            set => SetAttribute("TIPVISIBLE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the tip window has the appearance of a cartoon balloon
        /// with rounded corners and a stem. Default: false. Must be set before setting
        /// Tip.
        /// [Windows only]
        /// </summary>
        public bool TipBalloon
        {
            get => GetAttribute("TIPBALLOON") == "YES";
            set => SetAttribute("TIPBALLOON", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the title shown in a separate area when using the balloon tip
        /// format. Must be set before setting Tip.
        /// [Windows only]
        /// </summary>
        public string TipBalloonTitle
        {
            get => GetAttribute("TIPBALLOONTITLE");
            set => SetAttribute("TIPBALLOONTITLE", value);
        }

        /// <summary>
        /// Gets or sets the pre-defined icon shown in the title area when using the
        /// balloon tip format. Must be set before setting Tip.
        /// [Windows only]
        /// </summary>
        public TipIcon TipBalloonTitleIcon
        {
            get { int.TryParse(GetAttribute("TIPBALLOONTITLEICON"), CultureInfo.InvariantCulture, out int i); return (TipIcon)i; }
            set => SetAttribute("TIPBALLOONTITLEICON", ((int)value).ToString(CultureInfo.InvariantCulture));
        }

        private const string ReservedPrefix = "IUPSHARP_";


        /// <summary>
        /// Posts a message to this element, to be processed once IUP is back in its
        /// loop. This is IUP's thread-safe entry point, so unlike the rest of the API
        /// it may be called from any thread in order to marshal work back to the UI
        /// thread.
        /// </summary>
        /// <param name="text">
        /// A name identifying the message. Must not begin with the IUPSHARP_ prefix.
        /// </param>
        /// <param name="value">An arbitrary integer passed through to the callback.</param>
        /// <param name="number">An arbitrary double passed through to the callback.</param>
        /// <param name="pointer">
        /// An arbitrary pointer passed through to the callback. The caller owns
        /// whatever it points at and must keep it alive until the message is handled.
        /// </param>
        /// <returns>
        /// False if the element has already been destroyed, in which case nothing is
        /// posted. Returns false rather than throwing because a worker thread has no
        /// good way to handle an exception, and the element may legitimately be
        /// destroyed while a background task is still running.
        /// </returns>
        /// <exception cref="ArgumentException">The name uses the reserved prefix.</exception>
        /// <remarks>
        /// There is an inherent race: the element can be destroyed between the check
        /// and the native call. IUP offers no atomic alternative, so only post to
        /// elements the caller knows will outlive the post - typically a long-lived
        /// dialog rather than a transient control.
        /// </remarks>
        public bool PostMessage(string text, int value = 0, double number = 0.0, IntPtr pointer = default)
        {
            if (text != null && text.StartsWith(ReservedPrefix, StringComparison.Ordinal))
                throw new ArgumentException(
                    $"Message names starting with {ReservedPrefix} are reserved for IupSharp.",
                    nameof(text));

            return PostMessageInternal(text, value, number, pointer);
        }

        /// <summary>
        /// Posts a message without the reserved-name check, for IupSharp's own
        /// deferred operations. Returns false if the element is already destroyed.
        /// </summary>
        protected bool PostMessageInternal(string text, int value = 0, double number = 0.0, IntPtr pointer = default)
        {
            if (Handle == IntPtr.Zero)
                return false;

            IupNative.PostMessage(Handle, text, value, number, pointer);
            return true;
        }


        /// <summary>
        /// Called for each posted message before the public callback. Return true if
        /// the message was handled internally and should not be forwarded.
        /// </summary>
        protected virtual bool OnPostMessage(string text, int value, double number, IntPtr pointer) => false;


        #endregion



        #region METHODS

        /// <summary>
        /// Updates the size and layout of all controls in the same dialog. Use after
        /// changing size attributes, or attributes that affect the size of a control.
        /// It can be used for any element inside a dialog, but the layout of the whole
        /// dialog will be updated.
        /// </summary>
        public void Refresh()
        {
            CheckAlive();
            IupNative.IupRefresh(Handle);
        }

        /// <summary>
        /// Updates the layout of the children of this element only, without changing
        /// the size of the dialog.
        /// </summary>
        public void RefreshChildren()
        {
            CheckAlive();
            IupNative.IupRefreshChildren(Handle);
        }

        /// <summary>
        /// Marks the element to be redrawn when the main loop next becomes idle. Use
        /// Redraw for an immediate repaint.
        /// </summary>
        public void Update()
        {
            CheckAlive();
            IupNative.IupUpdate(Handle);
        }

        /// <summary>
        /// Marks this element and all its children to be redrawn when the main loop
        /// next becomes idle.
        /// </summary>
        public void UpdateChildren()
        {
            CheckAlive();
            IupNative.IupUpdateChildren(Handle);
        }

        /// <summary>
        /// Forces the element to be redrawn immediately.
        /// </summary>
        /// <param name="children">Also redraw the children of the element.</param>
        public void Redraw(bool children = false)
        {
            CheckAlive();
            IupNative.IupRedraw(Handle, children ? 1 : 0);
        }

        /// <summary>
        /// Sets the keyboard focus to this element. The element must be mapped and be
        /// able to receive the focus.
        /// </summary>
        public void SetFocus()
        {
            CheckAlive();
            IupNative.IupSetFocus(Handle);
        }


        #endregion

        #region IMAGE ATTRIBUTE HELPERS

        /// <summary>
        /// Sets an image attribute from an Image object, keeping the managed reference
        /// so the image stays reachable, and clearing any name previously set for the
        /// same attribute.
        /// </summary>
        /// <remarks>
        /// An image attribute holds one value, so the object form and the name form
        /// are mutually exclusive. These two helpers keep the cached fields consistent
        /// with whichever was assigned last.
        /// </remarks>
        protected void SetImageHandle(string attribute, Image image,
                                      ref Image imageField, ref string nameField)
        {
            CheckAlive();

            imageField = image;
            nameField = null;

            IupNative.SetAttributeHandle(Handle, attribute,
                image == null ? IntPtr.Zero : image.Handle);
        }

        /// <summary>
        /// Sets an image attribute from a name - a stock image, a name registered with
        /// IupSetHandle, a system resource name, or a path to an image file - and
        /// clears any Image object previously set for the same attribute.
        /// </summary>
        protected void SetImageName(string attribute, string imageName,
                                    ref Image imageField, ref string nameField)
        {
            CheckAlive();

            nameField = imageName;
            imageField = null;

            // SetAttribute copies the string, which IUP needs since it keeps the name.
            SetAttribute(attribute, imageName);
        }

        #endregion

        #region CALLBACKS
        private Callback _getFocusCB;
        private IFn _getFocusCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the element is given the keyboard
        /// focus. Called after ShowCB of the dialog.
        /// </summary>
        public Callback GetFocusCB
        {
            get => _getFocusCB;
            set
            {
                _getFocusCB = value;
                _getFocusCBInternal = GetFocusCBInternal;
                SetCallback("GETFOCUS_CB", Utils.CastCallback<Icallback>(_getFocusCBInternal));
            }
        }
        private int GetFocusCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _getFocusCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in GetFocusCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _killFocusCB;
        private IFn _killFocusCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the element loses the keyboard focus.
        /// Called before GetFocusCB of the element that receives it.
        /// </summary>
        public Callback KillFocusCB
        {
            get => _killFocusCB;
            set
            {
                _killFocusCB = value;
                _killFocusCBInternal = KillFocusCBInternal;
                SetCallback("KILLFOCUS_CB", Utils.CastCallback<Icallback>(_killFocusCBInternal));
            }
        }
        private int KillFocusCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _killFocusCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in KillFocusCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _enterWindowCB;
        private IFn _enterWindowCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse enters the element.
        /// </summary>
        public Callback EnterWindowCB
        {
            get => _enterWindowCB;
            set
            {
                _enterWindowCB = value;
                _enterWindowCBInternal = EnterWindowCBInternal;
                SetCallback("ENTERWINDOW_CB", Utils.CastCallback<Icallback>(_enterWindowCBInternal));
            }
        }
        private int EnterWindowCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _enterWindowCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in EnterWindowCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _leaveWindowCB;
        private IFn _leaveWindowCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse leaves the element.
        /// </summary>
        public Callback LeaveWindowCB
        {
            get => _leaveWindowCB;
            set
            {
                _leaveWindowCB = value;
                _leaveWindowCBInternal = LeaveWindowCBInternal;
                SetCallback("LEAVEWINDOW_CB", Utils.CastCallback<Icallback>(_leaveWindowCBInternal));
            }
        }
        private int LeaveWindowCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _leaveWindowCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in LeaveWindowCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private KeyCBCallback _kAny;
        private IFni _kAnyInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when a keyboard event occurs while the
        /// element has the focus. Set the callback data's Result to Ignore to make the
        /// system ignore the key, or to Continue to pass it to the parent element.
        /// When the element does not handle a key it is propagated to its parent, so a
        /// dialog can handle keys for all its children.
        /// </summary>
        public KeyCBCallback KAny
        {
            get => _kAny;
            set
            {
                _kAny = value;
                _kAnyInternal = KAnyInternal;
                SetCallback("K_ANY", Utils.CastCallback<Icallback>(_kAnyInternal));
            }
        }
        private int KAnyInternal(nint ih, int c)
        {
            try
            {
                var cb = new KeyCBData(this, (Key)c);
                _kAny?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in KAny callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _helpCB;
        private IFn _helpCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the user presses F1, or clicks a
        /// control while the dialog's help button is active. Setting the callback
        /// data's Result to Close will be processed.
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
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in HelpCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }


        private PostMessageCallback _postMessageCB;

        /// <summary>
        /// Keeps the native thunk alive for the GC. Non-null exactly when
        /// POSTMESSAGE_CB has been registered, so it doubles as the installed flag -
        /// never clear it without also unregistering the callback.
        /// </summary>
        private IFnsidv _postMessageCBInternal;

        /// <summary>
        /// Gets or sets the action generated when a message posted with PostMessage is
        /// processed. Messages consumed internally by IupSharp are not forwarded here.
        /// </summary>
        public PostMessageCallback PostMessageCB
        {
            get => _postMessageCB;
            set => _postMessageCB = value;   // hook already installed in constructor, so no need to set the native callback here
        }


        private int PostMessageCBInternal(nint ih, string text, int value, double number, IntPtr pointer)
        {
            try
            {
                if (OnPostMessage(text, value, number, pointer))
                    return (int)CallbackResult.Default;

                var cb = new PostMessageData(this, text, value, number, pointer);
                _postMessageCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in PostMessageCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }


        #endregion

    }
}