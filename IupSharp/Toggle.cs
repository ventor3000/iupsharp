using System;
using System.Drawing;

namespace IupSharp
{
    /// <summary>
    /// Toggle's state, as reported by the VALUE attribute.
    /// </summary>
    public enum ToggleValue
    {
        /// <summary>The toggle is off.</summary>
        Off,
        /// <summary>The toggle is on.</summary>
        On,
        /// <summary>The toggle is in the indeterminate state. Only valid in 3-state mode.</summary>
        NotDef
    }

    /// <summary>
    /// Creates the toggle interface element. It is a two-state (on/off) button that,
    /// when selected, generates an action that activates a function in the associated
    /// application. Its visual representation can contain a text or an image.
    /// </summary>
    /// <remarks>
    /// Toggle with image or text can not change its behavior after mapped - the choice
    /// of image vs. text is a creation-time decision. After creation the image can be
    /// changed for another image, and the text for another text.
    /// Toggles are activated using the Space key.
    /// To build a set of mutually exclusive toggles, insert them in a Radio container.
    /// They must be inserted before creation, and their behavior can not be changed.
    /// </remarks>
    public class Toggle : Control
    {
        /// <summary>
        /// Creates a new Toggle with the specified title.
        /// </summary>
        /// <param name="title">Text to be shown on the toggle. It can be null.</param>
        public Toggle(string title) : base(NativeIup.Toggle(title, null))
        {
        }

        /// <summary>
        /// Gets or sets the horizontal and vertical alignment, used when Image is defined.
        /// Possible values: Left, Center and Right, combined to Top, Middle and Bottom.
        /// Default: MiddleCenter. Partial values are also accepted, like Right or Top,
        /// the other value will be obtained from the default value. In Motif, vertical
        /// alignment is restricted to Center. In Windows works only when Visual Styles
        /// is active. Text is always left aligned.
        /// (non inheritable)
        /// </summary>
        public virtual Alignment Alignment
        {
            get => Utils.ParseAlignment(GetAttribute("ALIGNMENT"));
            set => SetAttribute("ALIGNMENT", Utils.FormatAlignment(value));
        }

        /// <summary>
        /// Background color of the toggle mark when displaying a text. The text
        /// background is transparent and uses the background color of the native
        /// parent. When displaying an image in Windows the background is ignored and
        /// the system color is used. Default: the global property DlgBgColor.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; } // just to allow for extra documentation for Toggle class

        /// <summary>
        /// Gets or sets whether focus traversal of the control is enabled. In Windows
        /// the control will still get the focus when clicked. Default: Yes.
        /// (creation only) (non inheritable)
        /// </summary>
        public virtual bool CanFocus
        {
            get => GetAttribute("CANFOCUS") != "NO";
            set => SetAttribute("CANFOCUS", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the focus callback forwarding to the next native parent
        /// with FocusCB defined is enabled. Default: No.
        /// (non inheritable)
        /// </summary>
        public virtual bool PropagateFocus
        {
            get => GetAttribute("PROPAGATEFOCUS") == "YES";
            set => SetAttribute("PROPAGATEFOCUS", value ? "YES" : "NO");
        }

        /// <summary>
        /// Color of the text shown on the toggle. In Windows, when using Visual Styles
        /// FgColor is ignored. Default: the global property DlgFgColor.
        /// </summary>
        public override Color FgColor { get => base.FgColor; set => base.FgColor = value; } // just to allow for extra documentation for Toggle class

        /// <summary>
        /// Gets or sets whether the toggle borders are hidden until the mouse cursor
        /// enters the toggle area when the toggle is not checked. If the toggle is
        /// checked, the borders will be shown even if flat is enabled. Used only when
        /// Image is defined. Default: No.
        /// (creation only)
        /// </summary>
        public virtual bool Flat
        {
            get => GetAttribute("FLAT") == "YES";
            set => SetAttribute("FLAT", value ? "YES" : "NO");
        }

        private Image _imageReference = null;
        private string _imageName = null;
        /// <summary>
        /// Gets or sets the image. When defined the Title is not shown, making the
        /// toggle look just like a button with an image, but its behavior remains the
        /// same. [GTK 2.6]
        /// (non inheritable)
        /// </summary>
        public virtual Image Image
        {
            get => _imageReference;
            set => SetImageHandle("IMAGE", value, ref _imageReference, ref _imageName);
        }

        /// <summary>
        /// Gets or sets the image by name rather than by object. Accepts a stock
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
            set => SetImageName("IMAGE", value, ref _imageReference, ref _imageName);
        }

        private Image _iminactiveReference = null;
        private string _iminactiveName = null;
        /// <summary>
        /// Gets or sets the image name of the toggle when inactive. If it is not
        /// defined but Image is defined then for inactive toggles the colors will be
        /// replaced by a modified version of the background color creating the
        /// disabled effect. [GTK 2.6]
        /// (non inheritable)
        /// </summary>
        public virtual Image ImInactive
        {
            get => _iminactiveReference;
            set => SetImageHandle("IMINACTIVE", value, ref _iminactiveReference, ref _iminactiveName);
        }

        /// <summary>
        /// Gets or sets the inactive image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="ImInactive"/> set the same IUP attribute, so assigning
        /// either clears the other. Reading this returns null when the image was set
        /// as an object.
        /// </remarks>
        public virtual string ImInactiveName
        {
            get => _iminactiveName;
            set => SetImageName("IMINACTIVE", value, ref _iminactiveReference, ref _iminactiveName);
        }

        private Image _impressReference = null;
        private string _impressName = null;
        /// <summary>
        /// Gets or sets the image of the pressed toggle. Unlike buttons, toggles
        /// always display the button border when Image and ImPress are both defined.
        /// [GTK 2.6]
        /// (non inheritable)
        /// </summary>
        public virtual Image ImPress
        {
            get => _impressReference;
            set => SetImageHandle("IMPRESS", value, ref _impressReference, ref _impressName);
        }

        /// <summary>
        /// Gets or sets the pressed image by name rather than by object. Accepts a stock
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
            set => SetImageName("IMPRESS", value, ref _impressReference, ref _impressName);
        }

        protected override void OnDestroying()
        {
            _impressReference = null;
            _impressName = null;
            _iminactiveReference = null;
            _iminactiveName = null;
            _imageReference = null;
            _imageName = null;
            base.OnDestroying();
        }

        /// <summary>
        /// Gets or sets whether the title string can contain pango markup commands.
        /// Works only if a mnemonic is NOT defined in the title. Default: false.
        /// [GTK only]
        /// </summary>
        public virtual bool Markup
        {
            get => GetAttribute("MARKUP") == "YES";
            set => SetAttribute("MARKUP", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the internal margin, used when Image is defined. Works just
        /// like the Margin attribute of the Hbox and Vbox containers, but uses a
        /// different name to avoid inheritance problems. Default value: (0, 0).
        /// </summary>
        public (int, int) Padding
        {
            get { NativeIup.GetIntInt(Handle, "PADDING", out int x, out int y); return (x, y); }
            set => SetAttribute("PADDING", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets whether the toggle is inside a Radio container. Valid only after the
        /// element is mapped; returns false before that.
        /// (read-only)
        /// </summary>
        public bool Radio => GetAttribute("RADIO") == "YES";

        /// <summary>
        /// Gets or sets whether the toggle will not behave as a radio button even
        /// when inside a Radio hierarchy. Default: No.
        /// (non inheritable)
        /// </summary>
        public virtual bool IgnoreRadio
        {
            get => GetAttribute("IGNORERADIO") == "YES";
            set => SetAttribute("IGNORERADIO", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the check mark is placed at the right of the text
        /// instead of the left. Default: No.
        /// (creation only) [Windows only]
        /// </summary>
        public virtual bool RightButton
        {
            get => GetAttribute("RIGHTBUTTON") == "YES";
            set => SetAttribute("RIGHTBUTTON", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether a three state toggle is enabled, allowing the value
        /// NotDef in addition to On and Off. Valid only for toggles with text and
        /// that do not belong to a Radio. Default: false.
        /// (creation only)
        /// </summary>
        public virtual bool ThreeState
        {
            get => GetAttribute("3STATE") == "YES";
            set => SetAttribute("3STATE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the toggle's text. If Image is not defined before map, the
        /// default behavior is to contain a text; this can not be changed after map.
        /// The natural size will be large enough to include all the text in the
        /// selected font, even using multiple lines, plus the button borders or check
        /// box if any. The '\n' character is accepted for line change. The "&amp;"
        /// character can be used to define a mnemonic, the next character will be
        /// used as key; use "&amp;&amp;" to show the "&amp;" character instead. The
        /// toggle can be activated from any control in the dialog using the "Alt+key"
        /// combination.
        /// (non inheritable)
        /// </summary>
        public string Title
        {
            get => GetAttribute("TITLE") ?? "";
            set => SetAttribute("TITLE", value);
        }

        static readonly (string, ToggleValue)[] toggleValues = new[]
        {
            ("OFF", ToggleValue.Off),
            ("ON", ToggleValue.On),
            ("NOTDEF", ToggleValue.NotDef)
        };

        /// <summary>
        /// Gets or sets the toggle's state. If 3State is true, NotDef is also
        /// available. Default: Off. Can only be set to On if the toggle is inside a
        /// Radio, in which case it automatically sets to Off the toggle that was
        /// previously On in the radio. The first toggle inside a Radio has its value
        /// set to On after map.
        /// (non inheritable)
        /// </summary>
        public ToggleValue Value
        {
            get => Utils.MapAttrib(GetAttribute("VALUE"), toggleValues);
            set => SetAttribute("VALUE", Utils.MapEnum(value, toggleValues));
        }

        /// <summary>
        /// Gets or sets whether the toggle is checked, as a simplified boolean view
        /// of Value for toggles that are not in 3-state mode.
        /// </summary>
        public bool Checked
        {
            get => Value == ToggleValue.On;
            set => Value = (value ? ToggleValue.On : ToggleValue.Off);
        }

        /// <summary>
        /// Inverts the toggle's current state (On becomes Off and vice-versa), by
        /// setting the native VALUE attribute to "TOGGLE".
        /// </summary>
        public void Invert() => SetAttribute("VALUE", "TOGGLE");

        #region CALLBACKS

        private ToggleActionCallback _action; // users callback function for Action
        private IFni _actionInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the toggle's state (on/off) is
        /// changed. The callback also receives the toggle's new state.
        /// </summary>
        public ToggleActionCallback Action
        {
            get => _action;
            set
            {
                _action = value;
                _actionInternal = ActionInternal;
                SetCallback("ACTION", Utils.CastCallback<Icallback>(_actionInternal));
            }
        }

        private int ActionInternal(nint ih, int status)
        {
            try
            {
                var cb = new ToggleActionData(this, status);
                _action?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in Action callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _valueChangedCB; // users callback function for ValueChangedCB
        private IFn _valueChangedCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the callback called after the value was interactively
        /// changed by the user. Called after the Action callback, but under the same
        /// context.
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

        #endregion
    }
}