using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates a button with a drop down arrow, which can act as a button, as a
    /// dropdown, or as both. Its visual presentation can contain a text and an image.
    /// (since 3.25)
    /// </summary>
    /// <remarks>
    /// <para>When dropped it shows an arbitrary element inside a dialog with no
    /// decorations, so it can imitate a dropdown list while containing any layout of
    /// IUP elements. Clicking outside the dialog closes it automatically.</para>
    ///
    /// <para>The drop child is NOT a regular child of the button. It is placed in a
    /// separate dialog, which is why it is set through the DropChild property rather
    /// than Append.</para>
    ///
    /// <para><b>Do not use the common Action, ButtonCB, MotionCB, FocusCB,
    /// EnterWindowCB or LeaveWindowCB on this control.</b> IUP uses them internally to
    /// implement the button. The equivalents are exposed here with a Flat prefix:
    /// FlatAction, FlatButtonCB, FlatMotionCB, FlatFocusCB, FlatEnterWindowCB and
    /// FlatLeaveWindowCB. They are called before the internal handlers, and setting
    /// the callback data's Result to Ignore stops the internal processing.</para>
    ///
    /// <para>Although IupDropButton inherits from IupCanvas natively, this wrapper
    /// derives from Control instead. Exposing the canvas surface - the drawing
    /// Action, the scrollbars, Dx and Dy - would offer members that either do nothing
    /// or actively break the control.</para>
    ///
    /// <para>The drop dialog does not close or update the title by itself. After the
    /// user picks something from the drop child, set ShowDropDown to false and update
    /// Title yourself.</para>
    /// </remarks>
    public class DropButton : Control
    {
        /// <summary>
        /// Creates a new drop button with no drop child. The arrow is disabled until a
        /// drop child is set.
        /// </summary>
        public DropButton() : base(NativeIup.IupDropButton(IntPtr.Zero))
        {
        }

        /// <summary>
        /// Creates a new drop button showing the given element when dropped.
        /// </summary>
        /// <param name="dropChild">
        /// The element shown when the button is dropped. It is placed in a separate
        /// undecorated dialog, not as a child of the button.
        /// </param>
        public DropButton(Control dropChild)
            : base(NativeIup.IupDropButton(dropChild == null ? IntPtr.Zero : dropChild.Handle))
        {
            CheckDropChild(dropChild);
            _dropChild = dropChild;
        }

        /// <summary>
        /// Creates a new drop button with a title and a drop child.
        /// </summary>
        public DropButton(string title, Control dropChild = null) : this(dropChild)
        {
            Title = title;
        }

        #region DROP CHILD


        private static Control CheckDropChild(Control dropChild)
        {
            if (dropChild is Dialog)
                throw new ArgumentException(
                    "The drop child must not be a Dialog. IUP creates its own undecorated " +
                    "dialog for it — pass the content directly, such as a VBox.",
                    nameof(dropChild));

            return dropChild;
        }

        private Control _dropChild;

        /// <summary>
        /// Gets or sets the element displayed when the button is dropped. It is placed
        /// inside a separate undecorated dialog rather than becoming a child of the
        /// button. If this is null the arrow is disabled automatically.
        /// </summary>
        public virtual Control DropChild
        {
            get => _dropChild;
            set
            {
                CheckAlive();
                CheckDropChild(value);
                _dropChild = value;
                NativeIup.SetAttributeHandle(Handle, "DROPCHILD_HANDLE",
                    value == null ? IntPtr.Zero : value.Handle);
            }
        }

        /// <summary>
        /// Gets the native handle of the dialog holding the drop child, or
        /// IntPtr.Zero when there is no drop child. The dialog is a regular IupDialog
        /// with no decorations, not resizable, and the application may reconfigure it.
        /// </summary>
        /// <remarks>
        /// This returns a raw handle rather than a Dialog because IUP creates the
        /// dialog internally, so there is no wrapper object for it.
        /// </remarks>
        public IntPtr DropDialogHandle
        {
            get
            {
                if (_dropChild == null || _dropChild.Handle == IntPtr.Zero)
                    return IntPtr.Zero;

                return NativeIup.IupGetDialog(_dropChild.Handle);
            }
        }

        /// <summary>
        /// Opens or closes the drop child. Ignored before the element is mapped.
        /// Reading always returns false, since IUP exposes this as write-only; use
        /// DropShowCB to track the actual state.
        /// (write only)
        /// </summary>
        public bool ShowDropDown
        {
            set => SetAttribute("SHOWDROPDOWN", value ? "YES" : "NO");
        }

        static readonly (string, DropPosition)[] _dropPositions = new[]
        {
            ("BOTTOMLEFT", DropPosition.BottomLeft),
            ("TOPLEFT", DropPosition.TopLeft),
            ("BOTTOMRIGHT", DropPosition.BottomRight),
            ("TOPRIGHT", DropPosition.TopRight)
        };

        /// <summary>
        /// Gets or sets where the drop child appears relative to the button.
        /// Default: BottomLeft.
        /// (non inheritable)
        /// </summary>
        public virtual DropPosition DropPosition
        {
            get => Utils.MapAttrib(GetAttribute("DROPPOSITION"), _dropPositions);
            set => SetAttribute("DROPPOSITION", Utils.MapEnum(value, _dropPositions));
        }

        /// <summary>
        /// Gets or sets whether only the arrow opens the drop child. When true the
        /// button has two separate areas and clicking the button part calls FlatAction.
        /// When false the whole button opens the drop child and FlatAction is never
        /// called. Default: true.
        /// (non inheritable)
        /// </summary>
        public virtual bool DropOnArrow
        {
            get => GetAttribute("DROPONARROW") != "NO";
            set => SetAttribute("DROPONARROW", value ? "YES" : "NO");
        }

        #endregion

        #region TEXT AND IMAGE

        /// <summary>
        /// Gets or sets the button's text. The '\n' character starts a new line.
        /// (non inheritable)
        /// </summary>
        public string Title
        {
            get => GetAttribute("TITLE") ?? "";
            set => SetAttribute("TITLE", value);
        }

        /// <summary>
        /// Gets or sets the horizontal and vertical alignment of the image and text
        /// together. Default: Left combined with Middle. Alignment does not include the
        /// padding area.
        /// (non inheritable)
        /// </summary>
        public virtual Alignment Alignment
        {
            get => Utils.ParseAlignment(GetAttribute("ALIGNMENT"));
            set => SetAttribute("ALIGNMENT", Utils.FormatAlignment(value));
        }

        static readonly (string, TextAlignment)[] _textAlignments = new[]
        {
            ("ALEFT", TextAlignment.Left),
            ("ACENTER", TextAlignment.Center),
            ("ARIGHT", TextAlignment.Right)
        };

        /// <summary>
        /// Gets or sets the horizontal alignment used for multiple lines of text.
        /// Default: Left.
        /// (non inheritable)
        /// </summary>
        public virtual TextAlignment TextAlignment
        {
            get => Utils.MapAttrib(GetAttribute("TEXTALIGNMENT"), _textAlignments);
            set => SetAttribute("TEXTALIGNMENT", Utils.MapEnum(value, _textAlignments));
        }

        /// <summary>
        /// Gets or sets whether a single line of text longer than its box is broken
        /// into several lines. The natural size still assumes one line, so use
        /// Expand.Vertical or set a size tall enough for the wrapped lines.
        /// (non inheritable) (since 3.25)
        /// </summary>
        public virtual bool TextWrap
        {
            get => GetAttribute("TEXTWRAP") == "YES";
            set => SetAttribute("TEXTWRAP", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether an ellipsis replaces the invisible part of a text
        /// longer than its box. Ignored when TextWrap is true.
        /// (non inheritable) (since 3.25)
        /// </summary>
        public virtual bool TextEllipsis
        {
            get => GetAttribute("TEXTELLIPSIS") == "YES";
            set => SetAttribute("TEXTELLIPSIS", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the text angle in degrees, counter-clockwise. The text size
        /// adapts to include the rotated space.
        /// (non inheritable) (since 3.25)
        /// </summary>
        public double TextOrientation
        {
            get => GetDouble("TEXTORIENTATION", 0.0);
            set => SetAttribute("TEXTORIENTATION", value.ToString("R", CultureInfo.InvariantCulture));
        }

        static readonly (string, ImagePosition)[] _imagePositions = new[]
        {
            ("LEFT", ImagePosition.Left),
            ("RIGHT", ImagePosition.Right),
            ("TOP", ImagePosition.Top),
            ("BOTTOM", ImagePosition.Bottom)
        };

        /// <summary>
        /// Gets or sets the position of the image relative to the text when both are
        /// shown. Default: Left.
        /// (non inheritable)
        /// </summary>
        public virtual ImagePosition ImagePosition
        {
            get => Utils.MapAttrib(GetAttribute("IMAGEPOSITION"), _imagePositions);
            set => SetAttribute("IMAGEPOSITION", Utils.MapEnum(value, _imagePositions));
        }

        private Image _image;
        private string _imageName;
        /// <summary>Gets or sets the button's image. (non inheritable)</summary>
        public virtual Image Image
        {
            get => _image;
            set => SetImageHandle("IMAGE", value, ref _image, ref _imageName);
        }

        /// <summary>
        /// Gets or sets the image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="Image"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string ImageName
        {
            get => _imageName;
            set => SetImageName("IMAGE", value, ref _image, ref _imageName);
        }


        private Image _imageHighlight;
        private string _imageHighlightName;
        /// <summary>
        /// Gets or sets the image shown in the highlight state. Falls back to Image
        /// when not set.
        /// (non inheritable)
        /// </summary>
        public virtual Image ImageHighlight
        {
            get => _imageHighlight;
            set => SetImageHandle("IMAGEHIGHLIGHT", value, ref _imageHighlight, ref _imageHighlightName);
        }

        /// <summary>
        /// Gets or sets the highlight image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="ImageHighlight"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string ImageHighlightName
        {
            get => _imageHighlightName;
            set => SetImageName("IMAGEHIGHLIGHT", value, ref _imageHighlight, ref _imageHighlightName);
        }


        private Image _imageInactive;
        private string _imageInactiveName;
        /// <summary>
        /// Gets or sets the image shown when inactive. Falls back to a greyed version
        /// of Image when not set.
        /// (non inheritable)
        /// </summary>
        public virtual Image ImageInactive
        {
            get => _imageInactive;
            set => SetImageHandle("IMAGEINACTIVE", value, ref _imageInactive, ref _imageInactiveName);
        }

        /// <summary>
        /// Gets or sets the inactive image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="ImageInactive"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string ImageInactiveName
        {
            get => _imageInactiveName;
            set => SetImageName("IMAGEINACTIVE", value, ref _imageInactive, ref _imageInactiveName);
        }


        private Image _imagePress;
        private string _imagePressName;
        /// <summary>
        /// Gets or sets the image shown in the pressed state. Falls back to Image when
        /// not set.
        /// (non inheritable)
        /// </summary>
        public virtual Image ImagePress
        {
            get => _imagePress;
            set => SetImageHandle("IMAGEPRESS", value, ref _imagePress, ref _imagePressName);
        }

        /// <summary>
        /// Gets or sets the pressed image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="ImagePress"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string ImagePressName
        {
            get => _imagePressName;
            set => SetImageName("IMAGEPRESS", value, ref _imagePress, ref _imagePressName);
        }


        #endregion

        #region BACKGROUND AND FOREGROUND IMAGES

        private Image _backImage;
        private string _backImageName;
        /// <summary>Gets or sets the background image. (non inheritable)</summary>
        public virtual Image BackImage
        {
            get => _backImage;
            set => SetImageHandle("BACKIMAGE", value, ref _backImage, ref _backImageName);
        }

        /// <summary>
        /// Gets or sets the background image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="BackImage"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string BackImageName
        {
            get => _backImageName;
            set => SetImageName("BACKIMAGE", value, ref _backImage, ref _backImageName);
        }


        private Image _backImageHighlight;
        private string _backImageHighlightName;
        /// <summary>
        /// Gets or sets the background image in the highlight state. Falls back to
        /// BackImage.
        /// (non inheritable)
        /// </summary>
        public virtual Image BackImageHighlight
        {
            get => _backImageHighlight;
            set => SetImageHandle("BACKIMAGEHIGHLIGHT", value, ref _backImageHighlight, ref _backImageHighlightName);
        }

        /// <summary>
        /// Gets or sets the highlight background image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="BackImageHighlight"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string BackImageHighlightName
        {
            get => _backImageHighlightName;
            set => SetImageName("BACKIMAGEHIGHLIGHT", value, ref _backImageHighlight, ref _backImageHighlightName);
        }


        private Image _backImageInactive;
        private string _backImageInactiveName;
        /// <summary>
        /// Gets or sets the background image when inactive. Falls back to a greyed
        /// BackImage.
        /// (non inheritable)
        /// </summary>
        public virtual Image BackImageInactive
        {
            get => _backImageInactive;
            set => SetImageHandle("BACKIMAGEINACTIVE", value, ref _backImageInactive, ref _backImageInactiveName);
        }

        /// <summary>
        /// Gets or sets the inactive background image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="BackImageInactive"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string BackImageInactiveName
        {
            get => _backImageInactiveName;
            set => SetImageName("BACKIMAGEINACTIVE", value, ref _backImageInactive, ref _backImageInactiveName);
        }


        private Image _backImagePress;
        private string _backImagePressName;
        /// <summary>
        /// Gets or sets the background image in the pressed state. Falls back to
        /// BackImage.
        /// (non inheritable)
        /// </summary>
        public virtual Image BackImagePress
        {
            get => _backImagePress;
            set => SetImageHandle("BACKIMAGEPRESS", value, ref _backImagePress, ref _backImagePressName);
        }

        /// <summary>
        /// Gets or sets the pressed background image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="BackImagePress"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string BackImagePressName
        {
            get => _backImagePressName;
            set => SetImageName("BACKIMAGEPRESS", value, ref _backImagePress, ref _backImagePressName);
        }


        /// <summary>
        /// Gets or sets whether the background image is stretched to fill the button.
        /// The aspect ratio is NOT preserved. Default: false.
        /// (non inheritable) (since 3.25)
        /// </summary>
        public virtual bool BackImageZoom
        {
            get => GetAttribute("BACKIMAGEZOOM") == "YES";
            set => SetAttribute("BACKIMAGEZOOM", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the natural size is computed from BackImage. Ignored
        /// when BackImage is not set. Default: false.
        /// (non inheritable)
        /// </summary>
        public virtual bool FitToBackImage
        {
            get => GetAttribute("FITTOBACKIMAGE") == "YES";
            set => SetAttribute("FITTOBACKIMAGE", value ? "YES" : "NO");
        }

        private Image _frontImage;
        private string _frontImageName;
        /// <summary>
        /// Gets or sets the foreground image, drawn in the same position as the
        /// background but painted last.
        /// (non inheritable)
        /// </summary>
        public virtual Image FrontImage
        {
            get => _frontImage;
            set => SetImageHandle("FRONTIMAGE", value, ref _frontImage, ref _frontImageName);
        }

        /// <summary>
        /// Gets or sets the foreground image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="FrontImage"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string FrontImageName
        {
            get => _frontImageName;
            set => SetImageName("FRONTIMAGE", value, ref _frontImage, ref _frontImageName);
        }


        private Image _frontImageHighlight;
        private string _frontImageHighlightName;
        /// <summary>
        /// Gets or sets the foreground image in the highlight state. Falls back to
        /// FrontImage.
        /// (non inheritable)
        /// </summary>
        public virtual Image FrontImageHighlight
        {
            get => _frontImageHighlight;
            set => SetImageHandle("FRONTIMAGEHIGHLIGHT", value, ref _frontImageHighlight, ref _frontImageHighlightName);
        }

        /// <summary>
        /// Gets or sets the highlight foreground image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="FrontImageHighlight"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string FrontImageHighlightName
        {
            get => _frontImageHighlightName;
            set => SetImageName("FRONTIMAGEHIGHLIGHT", value, ref _frontImageHighlight, ref _frontImageHighlightName);
        }


        private Image _frontImageInactive;
        private string _frontImageInactiveName;
        /// <summary>
        /// Gets or sets the foreground image when inactive. Falls back to a greyed
        /// FrontImage.
        /// (non inheritable)
        /// </summary>
        public virtual Image FrontImageInactive
        {
            get => _frontImageInactive;
            set => SetImageHandle("FRONTIMAGEINACTIVE", value, ref _frontImageInactive, ref _frontImageInactiveName);
        }

        /// <summary>
        /// Gets or sets the inactive foreground image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="FrontImageInactive"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string FrontImageInactiveName
        {
            get => _frontImageInactiveName;
            set => SetImageName("FRONTIMAGEINACTIVE", value, ref _frontImageInactive, ref _frontImageInactiveName);
        }


        private Image _frontImagePress;
        private string _frontImagePressName;
        /// <summary>
        /// Gets or sets the foreground image in the pressed state. Falls back to
        /// FrontImage.
        /// (non inheritable)
        /// </summary>
        public virtual Image FrontImagePress
        {
            get => _frontImagePress;
            set => SetImageHandle("FRONTIMAGEPRESS", value, ref _frontImagePress, ref _frontImagePressName);
        }

        /// <summary>
        /// Gets or sets the pressed foreground image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="FrontImagePress"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string FrontImagePressName
        {
            get => _frontImagePressName;
            set => SetImageName("FRONTIMAGEPRESS", value, ref _frontImagePress, ref _frontImagePressName);
        }


        #endregion

        #region ARROW

        /// <summary>
        /// Gets or sets whether the arrow is enabled while the button itself is
        /// enabled. The arrow is disabled automatically when there is no drop child.
        /// (non inheritable)
        /// </summary>
        public virtual bool ArrowActive
        {
            get => GetAttribute("ARROWACTIVE") != "NO";
            set => SetAttribute("ARROWACTIVE", value ? "YES" : "NO");
        }

        static readonly (string, VerticalAlignment)[] _arrowAligns = new[]
        {
            ("CENTER", VerticalAlignment.Center),
            ("TOP", VerticalAlignment.Top),
            ("BOTTOM", VerticalAlignment.Bottom)
        };

        /// <summary>
        /// Gets or sets the vertical alignment of the arrow. Default: Center.
        /// (non inheritable) (since 3.27)
        /// </summary>
        public virtual VerticalAlignment ArrowAlign
        {
            get => Utils.MapAttrib(GetAttribute("ARROWALIGN"), _arrowAligns);
            set => SetAttribute("ARROWALIGN", Utils.MapEnum(value, _arrowAligns));
        }

        /// <summary>
        /// Gets or sets the arrow color. Returns Color.Empty when not set, in which
        /// case FgColor is used.
        /// </summary>
        public Color ArrowColor
        {
            get => Utils.ParseColor(GetAttribute("ARROWCOLOR"));
            set => SetAttribute("ARROWCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets whether the drawn arrow is replaced by the arrow images. Make
        /// sure their sizes are no larger than ArrowSize. Default: false.
        /// (non inheritable)
        /// </summary>
        public virtual bool ArrowImages
        {
            get => GetAttribute("ARROWIMAGES") == "YES";
            set => SetAttribute("ARROWIMAGES", value ? "YES" : "NO");
        }

        private Image _arrowImage;
        private string _arrowImageName;
        /// <summary>
        /// Gets or sets the arrow image. Requires ArrowImages to be true.
        /// (non inheritable)
        /// </summary>
        public virtual Image ArrowImage
        {
            get => _arrowImage;
            set => SetImageHandle("ARROWIMAGE", value, ref _arrowImage, ref _arrowImageName);
        }

        /// <summary>
        /// Gets or sets the arrow image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="ArrowImage"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string ArrowImageName
        {
            get => _arrowImageName;
            set => SetImageName("ARROWIMAGE", value, ref _arrowImage, ref _arrowImageName);
        }


        private Image _arrowImageHighlight;
        private string _arrowImageHighlightName;
        /// <summary>
        /// Gets or sets the arrow image in the highlight state. Falls back to
        /// ArrowImage.
        /// (non inheritable)
        /// </summary>
        public virtual Image ArrowImageHighlight
        {
            get => _arrowImageHighlight;
            set => SetImageHandle("ARROWIMAGEHIGHLIGHT", value, ref _arrowImageHighlight, ref _arrowImageHighlightName);
        }

        /// <summary>
        /// Gets or sets the highlight arrow image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="ArrowImageHighlight"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string ArrowImageHighlightName
        {
            get => _arrowImageHighlightName;
            set => SetImageName("ARROWIMAGEHIGHLIGHT", value, ref _arrowImageHighlight, ref _arrowImageHighlightName);
        }


        private Image _arrowImageInactive;
        private string _arrowImageInactiveName;
        /// <summary>
        /// Gets or sets the arrow image when inactive. Falls back to a greyed
        /// ArrowImage.
        /// (non inheritable)
        /// </summary>
        public virtual Image ArrowImageInactive
        {
            get => _arrowImageInactive;
            set => SetImageHandle("ARROWIMAGEINACTIVE", value, ref _arrowImageInactive, ref _arrowImageInactiveName);
        }

        /// <summary>
        /// Gets or sets the inactive arrow image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="ArrowImageInactive"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string ArrowImageInactiveName
        {
            get => _arrowImageInactiveName;
            set => SetImageName("ARROWIMAGEINACTIVE", value, ref _arrowImageInactive, ref _arrowImageInactiveName);
        }


        private Image _arrowImagePress;
        private string _arrowImagePressName;
        /// <summary>
        /// Gets or sets the arrow image in the pressed state. Falls back to ArrowImage.
        /// (non inheritable)
        /// </summary>
        public virtual Image ArrowImagePress
        {
            get => _arrowImagePress;
            set => SetImageHandle("ARROWIMAGEPRESS", value, ref _arrowImagePress, ref _arrowImagePressName);
        }

        /// <summary>
        /// Gets or sets the pressed arrow image by name rather than by object. Accepts a stock
        /// image name (after IupImageLib.Open), a name registered with IupSetHandle,
        /// a system resource name, or a path to an image file.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// This and <see cref="ArrowImagePress"/> set the same IUP attribute, so assigning
        /// either clears the other.
        /// </remarks>
        public virtual string ArrowImagePressName
        {
            get => _arrowImagePressName;
            set => SetImageName("ARROWIMAGEPRESS", value, ref _arrowImagePress, ref _arrowImagePressName);
        }


        /// <summary>
        /// Gets or sets the internal margin for the arrow, inside ArrowSize.
        /// Default: 5.
        /// (non inheritable)
        /// </summary>
        public int ArrowPadding
        {
            get => GetInt("ARROWPADDING", 5);
            set => SetAttribute("ARROWPADDING", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the width of the area occupied by the arrow, including when
        /// images are used. Default: 24.
        /// (non inheritable)
        /// </summary>
        public int ArrowSize
        {
            get => GetInt("ARROWSIZE", 24);
            set => SetAttribute("ARROWSIZE", value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region COLORS AND BORDERS

        /// <summary>
        /// Gets or sets the background color. If neither text nor image is defined the
        /// button simply shows a color, in which case set its size because the natural
        /// size will be very small. Falls back to the background of the native parent.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; }

        /// <summary>
        /// Gets or sets the text color. Default: the global attribute DlgFgColor.
        /// </summary>
        public override Color FgColor { get => base.FgColor; set => base.FgColor = value; }

        /// <summary>
        /// Gets or sets the background color used for the highlight state.
        /// Pre-defined to (200,225,245). Assign Color.Empty to fall back to BgColor.
        /// </summary>
        public Color HlColor
        {
            get => Utils.ParseColor(GetAttribute("HLCOLOR"));
            set => SetAttribute("HLCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the background color used for the pressed state. Pre-defined to
        /// (150,200,235). Assign Color.Empty to fall back to BgColor.
        /// </summary>
        public Color PsColor
        {
            get => Utils.ParseColor(GetAttribute("PSCOLOR"));
            set => SetAttribute("PSCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the text color used for the highlight state. Falls back to
        /// FgColor.
        /// (since 3.26)
        /// </summary>
        public Color TextHlColor
        {
            get => Utils.ParseColor(GetAttribute("TEXTHLCOLOR"));
            set => SetAttribute("TEXTHLCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the text color used for the pressed state. Falls back to
        /// FgColor.
        /// (since 3.26)
        /// </summary>
        public Color TextPsColor
        {
            get => Utils.ParseColor(GetAttribute("TEXTPSCOLOR"));
            set => SetAttribute("TEXTPSCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the border color. Default: (50,150,255). This is the border
        /// drawn by IUP, not the canvas border.
        /// </summary>
        public Color BorderColor
        {
            get => Utils.ParseColor(GetAttribute("BORDERCOLOR"));
            set => SetAttribute("BORDERCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the border color when pressed or selected. Falls back to
        /// BorderColor.
        /// </summary>
        public Color BorderPsColor
        {
            get => Utils.ParseColor(GetAttribute("BORDERPSCOLOR"));
            set => SetAttribute("BORDERPSCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the border color when highlighted. Falls back to BorderColor.
        /// </summary>
        public Color BorderHlColor
        {
            get => Utils.ParseColor(GetAttribute("BORDERHLCOLOR"));
            set => SetAttribute("BORDERHLCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the border line width. Default: 1. Set to 0 to hide all
        /// borders.
        /// </summary>
        public int BorderWidth
        {
            get => GetInt("BORDERWIDTH", 1);
            set => SetAttribute("BORDERWIDTH", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets whether borders are always shown rather than only while
        /// highlighted. When true and BgColor is not set, the effective background is a
        /// darker version of the native parent's background.
        /// </summary>
        public virtual bool ShowBorder
        {
            get => GetAttribute("SHOWBORDER") == "YES";
            set => SetAttribute("SHOWBORDER", value ? "YES" : "NO");
        }

        #endregion

        #region LAYOUT AND STATE

        /// <summary>
        /// Gets or sets the internal margin in pixels. Alignment does not include the
        /// padding area. Default: (3,3).
        /// </summary>
        public (int, int) Padding
        {
            get { CheckAlive(); NativeIup.GetIntInt(Handle, "PADDING", out int x, out int y); return (x, y); }
            set => SetAttribute("PADDING", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets the spacing between the image and the text. Default: 2.
        /// (non inheritable)
        /// </summary>
        public int Spacing
        {
            get => GetInt("SPACING", 2);
            set => SetAttribute("SPACING", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the number of visible columns used for the natural size, which
        /// also acts as a minimum. Uses a wider character than Size does. Padding is
        /// applied around the visible columns.
        /// </summary>
        public int VisibleColumns
        {
            get => GetInt("VISIBLECOLUMNS", 0);
            set => SetAttribute("VISIBLECOLUMNS", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets whether focus traversal is enabled. Creation only. Unlike most
        /// controls, the button honours this in Windows. Default: true.
        /// (non inheritable)
        /// </summary>
        public virtual bool CanFocus
        {
            get => GetAttribute("CANFOCUS") != "NO";
            set => SetAttribute("CANFOCUS", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the focus feedback is drawn. Default: true.
        /// (non inheritable) (since 3.26)
        /// </summary>
        public virtual bool FocusFeedback
        {
            get => GetAttribute("FOCUSFEEDBACK") != "NO";
            set => SetAttribute("FOCUSFEEDBACK", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the focus callback is forwarded to the next native
        /// parent with FocusCB defined. Default: false.
        /// (non inheritable)
        /// </summary>
        public virtual bool PropagateFocus
        {
            get => GetAttribute("PROPAGATEFOCUS") == "YES";
            set => SetAttribute("PROPAGATEFOCUS", value ? "YES" : "NO");
        }

        /// <summary>Gets whether the button currently has the focus. (read only)</summary>
        public bool HasFocus => GetAttribute("HASFOCUS") == "YES";

        /// <summary>Gets whether the button is currently highlighted. (read only)</summary>
        public bool Highlighted => GetAttribute("HIGHLIGHTED") == "YES";

        /// <summary>Gets whether the button is currently pressed. (read only)</summary>
        public bool Pressed => GetAttribute("PRESSED") == "YES";

        #endregion

        #region HELPERS

        private int GetInt(string name, int fallback)
        {
            string v = GetAttribute(name);
            return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)
                ? i
                : fallback;
        }

        private double GetDouble(string name, double fallback)
        {
            string v = GetAttribute(name);
            return double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)
                ? d
                : fallback;
        }

        #endregion

        protected override void OnDestroying()
        {
            _dropChild = null;
            _image = _imageHighlight = _imageInactive = _imagePress = null;
            _backImage = _backImageHighlight = _backImageInactive = _backImagePress = null;
            _frontImage = _frontImageHighlight = _frontImageInactive = _frontImagePress = null;
            _arrowImage = _arrowImageHighlight = _arrowImageInactive = _arrowImagePress = null;
            _imageName = _imageHighlightName = _imageInactiveName = _imagePressName = null;
            _backImageName = _backImageHighlightName = _backImageInactiveName = _backImagePressName = null;
            _frontImageName = _frontImageHighlightName = _frontImageInactiveName = _frontImagePressName = null;
            _arrowImageName = _arrowImageHighlightName = _arrowImageInactiveName = _arrowImagePressName = null;
            base.OnDestroying();
        }

        #region CALLBACKS

        private Callback _flatAction;
        private IFn _flatActionInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when button 1 is released inside the
        /// button area. Called only when DropOnArrow is true, since otherwise the whole
        /// button opens the drop child. This is the equivalent of a normal Button's
        /// Action.
        /// </summary>
        public Callback FlatAction
        {
            get => _flatAction;
            set
            {
                _flatAction = value;
                _flatActionInternal = FlatActionInternal;
                SetCallback("FLAT_ACTION", Utils.CastCallback<Icallback>(_flatActionInternal));
            }
        }
        private int FlatActionInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _flatAction?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in FlatAction callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private DropStateCallback _dropDownCB;
        private IFni _dropDownCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated right BEFORE the drop child is shown or
        /// hidden. Also called when ShowDropDown is set.
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
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in DropDownCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private DropStateCallback _dropShowCB;
        private IFni _dropShowCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated right AFTER the drop child is shown or
        /// hidden. Also called when ShowDropDown is set.
        /// </summary>
        public DropStateCallback DropShowCB
        {
            get => _dropShowCB;
            set
            {
                _dropShowCB = value;
                _dropShowCBInternal = DropShowCBInternal;
                SetCallback("DROPSHOW_CB", Utils.CastCallback<Icallback>(_dropShowCBInternal));
            }
        }
        private int DropShowCBInternal(nint ih, int state)
        {
            try
            {
                var cb = new DropStateData(this, state);
                _dropShowCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in DropShowCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private ButtonCBCallback _flatButtonCB;
        private IFniiiis _flatButtonCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when any mouse button is pressed or
        /// released. Called before the internal handler; set the callback data's Result
        /// to Ignore to stop the internal processing.
        /// </summary>
        public ButtonCBCallback FlatButtonCB
        {
            get => _flatButtonCB;
            set
            {
                _flatButtonCB = value;
                _flatButtonCBInternal = FlatButtonCBInternal;
                SetCallback("FLAT_BUTTON_CB", Utils.CastCallback<Icallback>(_flatButtonCBInternal));
            }
        }
        private int FlatButtonCBInternal(nint ih, int but, int pressed, int x, int y, string status)
        {
            try
            {
                var cb = new ButtonCBData(this, but, pressed, x, y, status);
                _flatButtonCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in FlatButtonCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private MotionCBCallback _flatMotionCB;
        private IFniis _flatMotionCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse is moved. Called before the
        /// internal handler; set Result to Ignore to stop the internal processing.
        /// </summary>
        public MotionCBCallback FlatMotionCB
        {
            get => _flatMotionCB;
            set
            {
                _flatMotionCB = value;
                _flatMotionCBInternal = FlatMotionCBInternal;
                SetCallback("FLAT_MOTION_CB", Utils.CastCallback<Icallback>(_flatMotionCBInternal));
            }
        }
        private int FlatMotionCBInternal(nint ih, int x, int y, string status)
        {
            try
            {
                var cb = new MotionCBData(this, x, y, status);
                _flatMotionCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in FlatMotionCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private FocusCBCallback _flatFocusCB;
        private IFni _flatFocusCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when the button gains or loses the focus.
        /// Called before the internal handler.
        /// </summary>
        public FocusCBCallback FlatFocusCB
        {
            get => _flatFocusCB;
            set
            {
                _flatFocusCB = value;
                _flatFocusCBInternal = FlatFocusCBInternal;
                SetCallback("FLAT_FOCUS_CB", Utils.CastCallback<Icallback>(_flatFocusCBInternal));
            }
        }
        private int FlatFocusCBInternal(nint ih, int focus)
        {
            try
            {
                var cb = new FocusCBData(this, focus);
                _flatFocusCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in FlatFocusCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _flatEnterWindowCB;
        private IFn _flatEnterWindowCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse enters the button. Called
        /// before the internal handler.
        /// </summary>
        public Callback FlatEnterWindowCB
        {
            get => _flatEnterWindowCB;
            set
            {
                _flatEnterWindowCB = value;
                _flatEnterWindowCBInternal = FlatEnterWindowCBInternal;
                SetCallback("FLAT_ENTERWINDOW_CB", Utils.CastCallback<Icallback>(_flatEnterWindowCBInternal));
            }
        }
        private int FlatEnterWindowCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _flatEnterWindowCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in FlatEnterWindowCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _flatLeaveWindowCB;
        private IFn _flatLeaveWindowCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the mouse leaves the button. Called
        /// before the internal handler.
        /// </summary>
        public Callback FlatLeaveWindowCB
        {
            get => _flatLeaveWindowCB;
            set
            {
                _flatLeaveWindowCB = value;
                _flatLeaveWindowCBInternal = FlatLeaveWindowCBInternal;
                SetCallback("FLAT_LEAVEWINDOW_CB", Utils.CastCallback<Icallback>(_flatLeaveWindowCBInternal));
            }
        }
        private int FlatLeaveWindowCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _flatLeaveWindowCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in FlatLeaveWindowCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}