using System;
using System.Drawing;

namespace IupSharp
{

    /// <summary>
    /// Creates a label interface element, which displays a separator, a text or an
    /// image.
    /// </summary>
    /// <remarks>
    /// Labels with images, texts or line separator can not change its behavior after
    /// mapped. But after map the image can be changed for another image, and the text
    /// for another text.
    /// </remarks>
    public class Label : Control
    {
        /// <summary>
        /// Creates a new Label with the specified title.
        /// </summary>
        /// <param name="title">Text to be shown on the label. It can be null.</param>
        public Label(string title) : base(IupNative.Label(title))
        {

        }


        /// <summary>
        /// Gets or sets the horizontal and vertical alignment.
        /// Possible values: Left,Center and Right, combined to Top, Middle and Bottom.
        /// Default: MidelCenter. Partial values are also accepted, like Right or Top,
        /// the other value will be obtained from the default value. In Motif, vertical
        /// alignment is restricted to Center.
        /// (non inheritable)
        /// </summary>
        public virtual Alignment Alignment
        {
            get => Utils.ParseAlignment(GetAttribute("ALIGNMENT"));
            set => SetAttribute("ALIGNMENT", Utils.FormatAlignment(value));
        }


        /// <summary>
        /// Gets or sets the background color. Ignored, transparent in all systems;
        /// the background color of the native parent is used instead.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; } // just to enable special comments for label

        /// <summary>
        /// Gets or sets whether the drop of files is enabled. Default: No, but if
        /// DropFilesCB is defined when the element is mapped then it will be
        /// automatically enabled.
        /// [Windows and GTK only] (non inheritable)
        /// </summary>
        public virtual bool DropFilesTarget
        {
            get => GetAttribute("DROPFILESTARGET") == "YES";
            set => SetAttribute("DROPFILESTARGET", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether an ellipsis ("...") is added to the text if there is
        /// not enough space to render the entire string. Default: No.
        /// [Windows and GTK only]
        /// </summary>
        public virtual bool Ellipsis
        {
            get => GetAttribute("ELLIPSIS") == "YES";
            set => SetAttribute("ELLIPSIS", value ? "YES" : "NO");
        }

        private ImageRGBA _imageReference = null;
        /// <summary>
        /// Gets or sets the image. If set before mapping defines the behavior of
        /// the label to contain an image. The natural size will be the size of the
        /// image in pixels.
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
        /// Gets or sets the image of the element when inactive. If it is not defined
        /// then the Image is used and the colors will be replaced by a modified
        /// version of the background color creating the disabled effect.
        /// [GTK and Motif only] (non inheritable)
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

        protected override void OnDestroying()
        {
            _iminactiveReference = null;
            _imageReference = null;
            base.OnDestroying();
        }

        /// <summary>
        /// Gets or sets whether the title string can contain pango markup commands.
        /// Works only if a mnemonic is NOT defined in the title. Default: No.
        /// [GTK only]
        /// </summary>
        public virtual bool Markup
        {
            get => GetAttribute("MARKUP") == "YES";
            set => SetAttribute("MARKUP", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the internal margin. Works just like the Margin attribute of
        /// the Hbox and Vbox containers, but uses a different name to avoid
        /// inheritance problems. Not used when Separator is used. Default: (0,0).
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
        /// Gets or sets whether the label is turned into a line separator, and if so
        /// whether it is horizontal or vertical. When changed before mapping the
        /// Expand attribute is set accordingly.
        /// (creation only) (non inheritable)
        /// </summary>
        public virtual SeparatorOrientation SeparatorOrientation
        {
            get
            {
                string sep = GetAttribute("SEPARATOR");
                if (sep == "HORIZONTAL")
                    return SeparatorOrientation.Horizontal;
                else if (sep == "VERTICAL")
                    return SeparatorOrientation.Vertical;
                else
                    return SeparatorOrientation.No;

            }
            set
            {
                switch (value)
                {
                    case SeparatorOrientation.Horizontal:
                        SetAttribute("SEPARATOR", "HORIZONTAL");
                        break;
                    case SeparatorOrientation.Vertical:
                        SetAttribute("SEPARATOR", "VERTICAL");
                        break;
                    case SeparatorOrientation.No:
                    default:
                        SetAttribute("SEPARATOR", null); // separator off
                        break;

                }
            }
        }

        /// <summary>
        /// Gets or sets the label's text. If Separator or Image are not defined
        /// before map, then the default behavior is to contain a text. The label
        /// behavior can not be changed after map. The natural size will be large
        /// enough to include all the text in the selected font, even using multiple
        /// lines. The '\n' character is accepted for line change. The "&" character
        /// can be used to define a mnemonic, the next character will be used as key.
        /// Use "&&" to show the "&" character instead of defining a mnemonic.
        /// (non inheritable)
        /// </summary>
        public string Title
        {
            get => GetAttribute("TITLE") ?? "";
            set => SetAttribute("TITLE", value);
        }

        /// <summary>
        /// Gets or sets whether the wrapping of lines that do not fit in the label
        /// is enabled. Can only be set to true if Alignment is left-aligned.
        /// Default: No.
        /// [Windows and GTK only]
        /// </summary>
        public virtual bool WordWrap
        {
            get => GetAttribute("WORDWRAP") == "YES";
            set => SetAttribute("WORDWRAP", value ? "YES" : "NO");
        }


        #region CALLBACKS

        private ButtonCBCallback _buttonCB; // users callback function
        private IFniiiis _buttonCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when any mouse button is pressed and when it is released. 
        /// Both calls occur before the ACTION callback when button 1 is being used.
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
        private int ButtonCBInternal(nint ih, int i1, int i2, int i3, int i4, string s)
        {
            try
            {
                var cb = new ButtonCBData(this, i1, i2, i3, i4, s);
                _buttonCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ButtonCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }


        private MotionCBCallback _motionCB; // users callback function
        private IFniis _motionCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse is moved over the label.
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



        private DropFilesCBCallback _dropfilesCB; // users callback function
        private IFnsiii _dropfilesCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when one or more files are dropped on
        /// the label. Defining this callback before map enables DropFilesTarget
        /// automatically.
        /// [Windows and GTK only]
        /// </summary>
        public DropFilesCBCallback DropFilesCB
        {
            get => _dropfilesCB;
            set
            {
                _dropfilesCB = value;
                _dropfilesCBInternal = DropFilesCBInternal;
                SetCallback("DROPFILES_CB", Utils.CastCallback<Icallback>(_dropfilesCBInternal));
            }
        }

        private int DropFilesCBInternal(nint ih, string filename, int num, int x, int y)
        {
            try
            {
                var cb = new DropFilesCBData(this, filename, num, x, y);
                _dropfilesCB?.Invoke(cb);
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

    /*
    public class Label : Control
    {
        public Label(string title) : base(IupNative.Label(title))
        {

        }

        
        public virtual Alignment Alignment
        {
            get => Utils.ParseAlignment(GetAttribute("ALIGNMENT"));
            set => SetAttribute("ALIGNMENT", Utils.FormatAlignment(value));
        }

        
        public override uint BgColor { get => base.BgColor; set => base.BgColor = value; } // just to enable special commentts for label

        public virtual bool DropFilesTarget
        {
            get => GetAttribute("DROPFILESTARGET") == "YES";
            set => SetAttribute("DROPFILESTARGET", value ? "YES" : "NO");
        }

        public virtual bool Ellipsis
        {
            get => GetAttribute("ELLIPSIS") == "YES";
            set => SetAttribute("ELLIPSIS", value ? "YES" : "NO");
        }

        private ImageRGBA _imageReference = null;
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
        public virtual ImageRGBA ImInactive
        {
            set
            {
                _iminactiveReference = value;
                IupNative.SetAttributeHandle(Handle, "IMINACTIVE", value == null ? IntPtr.Zero : value.Handle);
            }
            get => _iminactiveReference;
        }

        public virtual bool Markup
        {
            get => GetAttribute("MARKUP") == "YES";
            set => SetAttribute("MARKUP", value ? "YES" : "NO");
        }

        public (int, int) Padding
        {
            get { IupNative.GetIntInt(((Control)this).Handle, "PADDING", out int x, out int y); return (x, y); }
            set => ((Control)this).SetAttribute("PADDING", Utils.FormatPadding(value));
        }

        public (int, int) CPadding
        {
            get { IupNative.GetIntInt(Handle, "CPADDING", out int x, out int y); return (x, y); }
            set => SetAttribute("CPADDING", Utils.FormatPadding(value));
        }

        public virtual Separator Separator
        {
            get
            {
                string sep = GetAttribute("SEPARATOR");
                if (sep == "HORIZONTAL")
                    return Separator.Horizontal;
                else if (sep == "VERTICAL")
                    return Separator.Vertical;
                else
                    return Separator.No;
                
            }
            set
            {
                switch(value)
                {
                    case Separator.Horizontal:
                        SetAttribute("SEPARATOR", "HORIZONTAL");
                        break;
                    case Separator.Vertical:
                        SetAttribute("SEPARATOR", "VERTICAL");
                        break;
                    case Separator.No:
                    default:
                        SetAttribute("SEPARATOR", null); // separator off
                        break;
                        
                }
            }
        }

        public string Title
        {
            get => GetAttribute("TITLE");
            set => SetAttribute("TITLE", value);
        }


        #region CALLBACKS

        private ButtonCBCallback _buttonCB; // users callback function
        private IFniiiis _buttonCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when any mouse button is pressed and when it is released. 
        /// Both calls occur before the ACTION callback when button 1 is being used.
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
        private int ButtonCBInternal(nint ih, int i1, int i2, int i3, int i4, string s)
        {
            try { 
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


        private MotionCBCallback _motionCB; // users callback function
        private IFniis _motionCBInternal; // need reference to keep alive in GC
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



        private DropFilesCBCallback _dropfilesCB; // users callback function
        private IFnsiii _dropfilesCBInternal; // need reference to keep alive in GC
        public DropFilesCBCallback DropFilesCB
        {
            get => _dropfilesCB;
            set
            {
                _dropfilesCB = value;
                _dropfilesCBInternal = DropFilesCBInternal;
                SetCallback("DROPFILES_CB", Utils.CastCallback<Icallback>(_dropfilesCBInternal));
            }
        }

        private int DropFilesCBInternal(nint ih, string filename, int num, int x, int y)
        {
            try
            {
                var cb = new DropFilesCBData(this, filename, num, x, y);
                _dropfilesCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in DropFilesCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }


        #endregion
    }*/

