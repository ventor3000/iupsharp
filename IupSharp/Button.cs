using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates an interface element that is a button. When selected, this element
    /// activates a function in the application. Its visual presentation can contain a
    /// text and/or an image.
    /// </summary>
    public class Button:Control
    {
        /// <summary>
        /// Creates a new Button with the specified title.
        /// </summary>
        /// <param name="title">Text to be shown to the user. It can be null.</param>
        public Button(string title):base(IupNative.Button(title, null))
        {
        }

       

        /// <summary>
        /// Gets or sets the horizontal and vertical alignment. 
        /// Possible values: Left,Center and Right, combined to Top, Middle and Bottom.
        /// Default: MidelCenter. Partial values are also accepted, like Right or Top, 
        /// the other value will be obtained from the default value. In Motif, vertical alignment is 
        /// restricted to Center. In GTK, horizontal alignment for multiple lines will align only the text block.
        /// (non inheritable)
        /// </summary>
        public virtual Alignment Alignment
        {
            get => Utils.ParseAlignment(GetAttribute("ALIGNMENT"));
            set => SetAttribute("ALIGNMENT", Utils.FormatAlignment(value));
        }


        /// <summary>
        /// Background color. If text and image are not defined, the button is configured to
        /// simply show a color, in this case set the button size because the natural size
        /// will be very small. In Windows and in GTK 3, the BgColor property is ignored if
        /// text or image is defined. Default: the global property DlgBgColor. BgColor is
        /// ignored when Flat=Yes because it will be used the background from the native parent.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; } // just to allow for extra documentation for Button class

        /// <summary>
        /// Gets or sets whether focus traversal of the control is enabled. In Windows the button 
        /// will respect CanFocus differently to some other controls. Default: Yes.
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
        /// Gets or sets whether the button borders are hidden until the mouse cursor enters 
        /// the button area. The border space is always there. Default: No.
        /// (creation only)
        /// </summary>
        public virtual bool Flat
        {
            get => GetAttribute("FLAT") == "YES";
            set => SetAttribute("FLAT", value ? "YES" : "NO");
        }


        private ImageRGBA _imageReference=null;
        /// <summary>
        /// Gets or sets the image. If set before mapping defines the behavior of
        /// the button to contain an image. The natural size will be size of the image in
        /// pixels, plus the button borders. If Title is also defined and not empty both 
        /// will be shown (except in Motif).
        /// (non inheritable)
        /// </summary>
        public virtual ImageRGBA Image
        {
            set
            {
                _imageReference = value;
                IupNative.SetAttributeHandle(Handle, "IMAGE", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _imageReference;
        }

        

        private ImageRGBA _iminactiveReference = null;
        /// <summary>
        /// Gets or sets the image name of the element when inactive. If it is
        /// not defined then the Image is used and the colors will be replaced by a modified
        /// version of the background color creating the disabled effect. GTK will also
        /// change the inactive image to look like other inactive objects.
        /// (non inheritable)
        /// </summary>
        public virtual ImageRGBA ImInactive
        {
            set
            {
                _iminactiveReference = value;
                IupNative.SetAttributeHandle(Handle, "IMINACTIVE", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _iminactiveReference;
        }

        
        private ImageRGBA _impressReference = null;
        /// <summary>
        /// Gets or sets the image of the pressed button. If ImPress and
        /// Image are defined, the button borders are not shown and not computed in natural
        /// size. When the button is clicked the pressed image does not offset. In Motif the
        /// button will lose its focus feedback also.
        /// (non inheritable)
        /// </summary>
        public virtual ImageRGBA ImPress
        {
            set
            {
                _impressReference = value;
                IupNative.SetAttributeHandle(Handle, "IMPRESS", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _impressReference;
        }


        protected override void OnDestroying()
        {
            _impressReference = null;
            _iminactiveReference = null;
            _imageReference = null;
            base.OnDestroying();
        }

        /// <summary>
        /// Gets or sets whether the button borders will be shown and
        /// computed even if ImPress is defined. Default: No.
        /// (non inheritable)
        /// </summary>
        public virtual bool ImPressBorder
        {
            get => GetAttribute("IMPRESSBORDER") == "YES";
            set => SetAttribute("IMPRESSBORDER", value ? "YES" : "NO");
        }


        static (string, ImagePosition)[] _imagePositions = new[] { ("LEFT", ImagePosition.Left), ("RIGHT", ImagePosition.Right), ("TOP", ImagePosition.Top), ("BOTTOM", ImagePosition.Bottom) };
        /// <summary>
        /// Gets or sets the position of the image relative to the text when
        /// both are displayed. Can be: Left, Right, Top, Bottom. Default: Left.
        /// (non inheritable)
        /// </summary>
        public virtual ImagePosition ImagePosition {
            get => Utils.MapAttrib(GetAttribute("IMAGEPOSITION"), _imagePositions);
            set => SetAttribute("IMAGEPOSITION", Utils.MapEnum(value, _imagePositions));
        }

        /// <summary>
        /// Gets or sets whether the title string can contain pango markup commands (GTK only).
        /// Works only if a mnemonic is NOT defined in the title. Default: false.
        /// </summary>
        public virtual bool Markup
        {
            get => GetAttribute("MARKUP") == "YES";
            set => SetAttribute("MARKUP", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the internal margin. Works just like the Margin attribute of the Hbox
        /// and Vbox containers, but uses a different name to avoid inheritance problems.
        /// Default value: (0,0). 
        /// </summary>
        public (int, int) Padding
        {
            get { IupNative.GetIntInt(((Control)this).Handle, "PADDING", out int x, out int y); return (x, y); }
            set => ((Control)this).SetAttribute("PADDING", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets the padding using the units of the Size property. 
        /// It will actually set the Padding property.
        /// </summary>
        public (int, int) CPadding
        {
            get { IupNative.GetIntInt(Handle, "CPADDING", out int x, out int y); return (x, y); }
            set => SetAttribute("CPADDING", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets the spacing between the image associated and
        /// the button's text. Default: 2.
        /// (creation only)
        /// </summary>
        public int Spacing
        {
            get { int.TryParse(GetAttribute("SPACING"), CultureInfo.InvariantCulture, out int space); return space; }
            set { SetAttribute("SPACING", value.ToString(CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// Gets or sets the spacing using the units of the vertical part of the Size
        /// attribute. It will actually set the Spacing property.
        /// </summary>
        public int CSpacing
        {
            get { int.TryParse(GetAttribute("CSPACING"), CultureInfo.InvariantCulture, out int space); return space; }
            set { SetAttribute("CSPACING", value.ToString(CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// Gets or sets the button's text. If Image is not defined before map, then the
        /// default behavior is to contain only a text. The button behavior can not be
        /// changed after map. The natural size will be large enough to include all the
        /// text in the selected font, even using multiple lines, plus the button borders.
        /// The '\n' character is accepted for line change. The "&" character can be used to
        /// define a mnemonic, the next character will be used as key. Use "&&" to show the
        /// "&" character instead on defining a mnemonic. The button can be activated from
        /// any control in the dialog using the "Alt+key" combination.
        /// (non inheritable)
        /// </summary>
        public string Title
        {
            get => GetAttribute("TITLE")??"";
            set => SetAttribute("TITLE",value);
        }

        #region CALLBACKS

        private Callback _action; // users callback function for Action
        private IFn _actionInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the button 1 (usually left) is selected. 
        /// This callback is called only after the mouse is released and when it is released 
        /// inside the button area.
        /// </summary>
        public Callback Action
        {
            get => _action;
            set
            {
                _action = value;
                _actionInternal = ActionInternal;
                SetCallback( "ACTION", Utils.CastCallback<Icallback>(_actionInternal));
            }
        }

        private int ActionInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _action?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in Action callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }


        private ButtonCBCallback _buttonCB; // users callback function for ButtonCB
        private IFniiiis _buttonCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when any mouse button is pressed and when it is released. 
        /// Both calls occur before the ACTION callback when button 1 is being used.
        /// </summary>
        public ButtonCBCallback ButtonCB
        {
            get => _buttonCB;
            set {
                _buttonCB = value;
                _buttonCBInternal = ButtonCBInternal;
                SetCallback("BUTTON_CB", Utils.CastCallback<Icallback>(_buttonCBInternal));
            }
        }
        private int ButtonCBInternal(nint ih, int i1, int i2, int i3, int i4, string s)
        {
            try
            {
                var cb = new ButtonCBData(this, i1, i2, i3, i4, s);
                _buttonCB?.Invoke(cb);
                return cb.Result;
            }
            
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ButtonCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
}

        
        #endregion
    }
}
