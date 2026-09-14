using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>Where an <see cref="Expander"/>'s bar handler sits.</summary>
    public enum BarPosition
    {
        /// <summary>
        /// Along the top edge. This is the default, and the only position that
        /// supports a title, a title image, extra buttons or animation.
        /// </summary>
        Top,
        /// <summary>Along the bottom edge.</summary>
        Bottom,
        /// <summary>Down the left side.</summary>
        Left,
        /// <summary>Down the right side.</summary>
        Right
    }

    /// <summary>How an <see cref="Expander"/> animates as it opens and closes.</summary>
    public enum ExpanderAnimation
    {
        /// <summary>No animation. This is the default.</summary>
        None,
        /// <summary>The child slides down into place.</summary>
        Slide,
        /// <summary>The child appears as though a curtain were being pulled.</summary>
        Curtain
    }

    /// <summary>
    /// A void container that can interactively show or hide its single child, with a
    /// bar handler the user clicks to expand or collapse it. (since 3.8)
    /// </summary>
    /// <remarks>
    /// <para><b>BarPosition and ExtraButtons are creation-only</b>, so they are
    /// constructor arguments. Almost everything decorative - the title, the title
    /// image, the extra buttons and animation - works only when the bar is at the
    /// top.</para>
    ///
    /// <para>The bar handler is built from ordinary IUP elements rather than being
    /// drawn: a BackgroundBox is always the expander's first child, holding an HBox or
    /// VBox of labels. That is why <see cref="Count"/> and child navigation report one
    /// more child than you added.</para>
    ///
    /// <para>Title and TitleImage are mutually exclusive - setting either resets the
    /// other.</para>
    ///
    /// <example>
    /// <code>
    /// var section = new Expander(new VBox(field1, field2)) { Title = "Advanced" };
    /// section.State = ExpanderState.Close;
    ///
    /// section.OpenCloseCB = d =&gt;
    /// {
    ///     if (!CanCollapse) d.Result = CallbackResult.Ignore;   // veto the change
    /// };
    /// </code>
    /// </example>
    /// </remarks>
    public class Expander : ContainerControl
    {
        /// <summary>The highest extra button id IUP allows.</summary>
        private const int MaxExtraButtons = 3;

        private Control _child;

        /// <summary>
        /// Creates a new expander.
        /// </summary>
        /// <param name="child">
        /// The element to show and hide. It can be null, in which case a child can be
        /// added later with Append.
        /// </param>
        /// <param name="barPosition">
        /// Where the bar handler sits. Creation only. Default: Top, which is the only
        /// position supporting a title, images, extra buttons or animation.
        /// </param>
        /// <param name="extraButtons">
        /// How many extra image buttons appear at the right of the bar, from 0 to 3.
        /// Creation only, and only when barPosition is Top. See ExtraButtonCB.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">extraButtons is outside 0 to 3.</exception>
        public Expander(Control child = null,
                        BarPosition barPosition = BarPosition.Top,
                        int extraButtons = 0)
            : base(NativeIup.IupExpander(child == null ? IntPtr.Zero : child.Handle))
        {
            if (extraButtons < 0 || extraButtons > MaxExtraButtons)
                throw new ArgumentOutOfRangeException(nameof(extraButtons),
                    $"An expander supports 0 to {MaxExtraButtons} extra buttons.");

            _child = child;

            // Both are creation-only, so they must be set before the element is mapped.
            if (barPosition != BarPosition.Top)
                SetAttribute("BARPOSITION", BarPositionToString(barPosition));

            if (extraButtons > 0)
                SetAttribute("EXTRABUTTONS", extraButtons.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets the element being shown and hidden, or null if there is none. This is
        /// the child you supplied, not the bar handler IUP adds.
        /// </summary>
        public Control Child => _child;

        /// <summary>
        /// Sets the element to show and hide. An expander manages one child besides
        /// its own bar handler; wrap several controls in a VBox or HBox.
        /// </summary>
        /// <exception cref="InvalidOperationException">The expander already has a child.</exception>
        public override void Append(Control child)
        {
            if (_child != null)
                throw new InvalidOperationException(
                    "An Expander manages one child. Wrap the controls in a VBox or HBox.");

            base.Append(child);
            _child = child;
        }

        #region STATE

        /// <summary>
        /// Gets or sets whether the child is shown. Default: Open. Setting this
        /// relayouts the whole dialog so the child can be recomposed.
        /// (non inheritable)
        /// </summary>
        public virtual ExpanderState State
        {
            get => GetAttribute("STATE") == "CLOSE" ? ExpanderState.Close : ExpanderState.Open;
            set => SetAttribute("STATE", value == ExpanderState.Close ? "CLOSE" : "OPEN");
        }

        /// <summary>Gets whether the child is currently shown.</summary>
        public bool IsOpen => State == ExpanderState.Open;

        /// <summary>Shows the child.</summary>
        public void Open() => State = ExpanderState.Open;

        /// <summary>Hides the child.</summary>
        public void Close() => State = ExpanderState.Close;

        /// <summary>Swaps between open and closed.</summary>
        public void Toggle() =>
            State = IsOpen ? ExpanderState.Close : ExpanderState.Open;

        /// <summary>
        /// Gets or sets whether Refresh is called automatically when the state
        /// changes. Default: true. Turn it off to batch several layout changes.
        /// (non inheritable) (since 3.16)
        /// </summary>
        public virtual bool StateRefresh
        {
            get => GetAttribute("STATEREFRESH") != "NO";
            set => SetAttribute("STATEREFRESH", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the child appears automatically once the mouse has
        /// rested over the bar for a second. Default: false.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// The dialog layout is <b>not</b> recalculated in this mode - the child is
        /// drawn on top of the dialog and hidden again when the mouse leaves, so
        /// nothing else moves. It works only when the child is a native container such
        /// as Frame, BackgroundBox, ScrollBox or Tabs, and it cannot be combined with
        /// Animation.
        /// </remarks>
        public virtual bool AutoShow
        {
            get => GetAttribute("AUTOSHOW") == "YES";
            set => SetAttribute("AUTOSHOW", value ? "YES" : "NO");
        }

        #endregion

        #region ANIMATION

        static readonly (string, ExpanderAnimation)[] _animations = new[]
        {
            ("NO", ExpanderAnimation.None),
            ("SLIDE", ExpanderAnimation.Slide),
            ("CURTAIN", ExpanderAnimation.Curtain)
        };

        /// <summary>
        /// Gets or sets the animation used while opening and closing.
        /// Default: None.
        /// (non inheritable) (since 3.14)
        /// </summary>
        /// <remarks>
        /// Works only with BarPosition.Top, and not at all with AutoShow. The child
        /// must also be a native container such as Tabs, Frame, BackgroundBox or
        /// ScrollBox, or the result will look wrong.
        /// </remarks>
        public virtual ExpanderAnimation Animation
        {
            get => Utils.MapAttrib(GetAttribute("ANIMATION"), _animations);
            set => SetAttribute("ANIMATION", Utils.MapEnum(value, _animations));
        }

        /// <summary>
        /// Gets or sets how many frames the animation uses. Default: 10.
        /// (non inheritable) (since 3.14)
        /// </summary>
        public int NumFrames
        {
            get => GetInt("NUMFRAMES", 10);
            set => SetAttribute("NUMFRAMES", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the time between animation frames, in milliseconds.
        /// Default: 30.
        /// (non inheritable) (since 3.14)
        /// </summary>
        /// <remarks>
        /// If the dialog's layout takes longer than this to compute, a frame is
        /// dropped - but the total animation time stays NumFrames times FrameTime
        /// either way.
        /// </remarks>
        public int FrameTime
        {
            get => GetInt("FRAMETIME", 30);
            set => SetAttribute("FRAMETIME", value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region TITLE

        private Image _titleImage;
        private string _titleImageName;

        /// <summary>
        /// Gets or sets the title shown in the bar handler beside the expand button.
        /// Shown only when BarPosition is Top. Setting it resets TitleImage.
        /// (non inheritable)
        /// </summary>
        public string Title
        {
            get => GetAttribute("TITLE") ?? "";
            set => SetAttribute("TITLE", value);
        }

        /// <summary>
        /// Gets or sets an image used in place of the title text. Shown only when
        /// BarPosition is Top. Setting it resets Title - the two are mutually
        /// exclusive.
        /// (non inheritable) (since 3.14)
        /// </summary>
        public virtual Image TitleImage
        {
            get => _titleImage;
            set => SetImageHandle("TITLEIMAGE", value, ref _titleImage, ref _titleImageName);
        }

        /// <summary>
        /// Gets or sets the title image by name rather than by object.
        /// (non inheritable) (since 3.14)
        /// </summary>
        public virtual string TitleImageName
        {
            get => _titleImageName;
            set => SetImageName("TITLEIMAGE", value, ref _titleImage, ref _titleImageName);
        }

        /// <summary>Title image name used when the expander is open.</summary>
        public string TitleImageOpenName
        {
            get => GetAttribute("TITLEIMAGEOPEN");
            set => SetAttribute("TITLEIMAGEOPEN", value);
        }

        /// <summary>
        /// Title image name used when the mouse is over the title and the expander is
        /// closed.
        /// </summary>
        public string TitleImageHighlightName
        {
            get => GetAttribute("TITLEIMAGEHIGHLIGHT");
            set => SetAttribute("TITLEIMAGEHIGHLIGHT", value);
        }

        /// <summary>
        /// Title image name used when the mouse is over the title and the expander is
        /// open.
        /// </summary>
        public string TitleImageOpenHighlightName
        {
            get => GetAttribute("TITLEIMAGEOPENHIGHLIGHT");
            set => SetAttribute("TITLEIMAGEOPENHIGHLIGHT", value);
        }

        /// <summary>
        /// Gets or sets whether clicking the title also expands and collapses, rather
        /// than only the arrow. Default: false.
        /// (non inheritable) (since 3.14)
        /// </summary>
        /// <remarks>HighColor applies only when this is true.</remarks>
        public virtual bool TitleExpand
        {
            get => GetAttribute("TITLEEXPAND") == "YES";
            set => SetAttribute("TITLEEXPAND", value ? "YES" : "NO");
        }

        #endregion

        #region ARROW IMAGES

        /// <summary>
        /// Gets or sets the image name replacing the arrow when the expander is
        /// closed. Works only when BarPosition is Top.
        /// (non inheritable) (since 3.11)
        /// </summary>
        public string ImageName
        {
            get => GetAttribute("IMAGE");
            set => SetAttribute("IMAGE", value);
        }

        /// <summary>Image name used when the expander is open.</summary>
        public string ImageOpenName
        {
            get => GetAttribute("IMAGEOPEN");
            set => SetAttribute("IMAGEOPEN", value);
        }

        /// <summary>
        /// Image name used when the mouse is over the bar and the expander is closed.
        /// </summary>
        public string ImageHighlightName
        {
            get => GetAttribute("IMAGEHIGHLIGHT");
            set => SetAttribute("IMAGEHIGHLIGHT", value);
        }

        /// <summary>
        /// Image name used when the mouse is over the bar and the expander is open.
        /// </summary>
        public string ImageOpenHighlightName
        {
            get => GetAttribute("IMAGEOPENHIGHLIGHT");
            set => SetAttribute("IMAGEOPENHIGHLIGHT", value);
        }

        #endregion

        #region EXTRA BUTTONS

        /// <summary>
        /// Gets how many extra image buttons the bar has. Creation only - pass it to
        /// the constructor.
        /// (non inheritable) (since 3.11)
        /// </summary>
        public int ExtraButtons => GetInt("EXTRABUTTONS", 0);

        /// <summary>
        /// Sets the image for an extra button, by name.
        /// </summary>
        /// <param name="id">
        /// Which button, from 1 to 3. Button 1 is the rightmost, counting leftwards.
        /// </param>
        /// <param name="imageName">The image name, or null to clear it.</param>
        /// <exception cref="ArgumentOutOfRangeException">id is outside 1 to 3.</exception>
        public void SetExtraButtonImageName(int id, string imageName) =>
            SetAttributeId("IMAGEEXTRA", CheckExtraButtonId(id), imageName);

        /// <summary>Sets the image shown while an extra button is pressed, by name.</summary>
        public void SetExtraButtonPressImageName(int id, string imageName) =>
            SetAttributeId("IMAGEEXTRAPRESS", CheckExtraButtonId(id), imageName);

        /// <summary>
        /// Sets the image shown while the mouse is over an extra button, by name.
        /// </summary>
        public void SetExtraButtonHighlightImageName(int id, string imageName) =>
            SetAttributeId("IMAGEEXTRAHIGHLIGHT", CheckExtraButtonId(id), imageName);

        /// <summary>
        /// Sets the image for an extra button from an Image object.
        /// </summary>
        /// <remarks>
        /// No managed reference is kept, so the caller must keep the Image alive for
        /// as long as the button uses it.
        /// </remarks>
        public void SetExtraButtonImage(int id, Image image)
        {
            CheckAlive();
            NativeIup.SetAttributeHandleId(Handle, "IMAGEEXTRA", CheckExtraButtonId(id),
                image == null ? IntPtr.Zero : image.Handle);
        }

        private static int CheckExtraButtonId(int id)
        {
            if (id < 1 || id > MaxExtraButtons)
                throw new ArgumentOutOfRangeException(nameof(id),
                    $"Extra button ids run from 1 to {MaxExtraButtons}, 1 being the rightmost.");

            return id;
        }

        #endregion

        #region BAR APPEARANCE

        /// <summary>
        /// Gets where the bar handler sits. Creation only - pass it to the
        /// constructor.
        /// </summary>
        public BarPosition BarPosition => ParseBarPosition(GetAttribute("BARPOSITION"));

        /// <summary>
        /// Gets or sets the size of the bar handler, in pixels. When not set it is the
        /// height or width that fits the bar's contents, depending on BarPosition.
        /// (non inheritable)
        /// </summary>
        public int BarSize
        {
            get => GetInt("BARSIZE", 0);
            set => SetAttribute("BARSIZE", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the bar handler's background colour. Returns Color.Empty when
        /// not set, in which case the background of the native parent is used.
        /// (non inheritable) (since 3.9)
        /// </summary>
        public Color BackColor
        {
            get => Utils.ParseColor(GetAttribute("BACKCOLOR"));
            set => SetAttribute("BACKCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the title text colour. Default: the global attribute
        /// DlgFgColor.
        /// (non inheritable) (since 3.9)
        /// </summary>
        public Color ForeColor
        {
            get => Utils.ParseColor(GetAttribute("FORECOLOR"));
            set => SetAttribute("FORECOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the title text colour while the expander is open. Falls back
        /// to ForeColor when not set.
        /// (non inheritable) (since 3.14)
        /// </summary>
        public Color OpenColor
        {
            get => Utils.ParseColor(GetAttribute("OPENCOLOR"));
            set => SetAttribute("OPENCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the title text colour while highlighted. Works only when
        /// TitleExpand is true, and falls back to ForeColor when not set.
        /// (non inheritable) (since 3.14)
        /// </summary>
        public Color HighColor
        {
            get => Utils.ParseColor(GetAttribute("HIGHCOLOR"));
            set => SetAttribute("HIGHCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets whether a frame line is drawn around the bar area.
        /// Default: false.
        /// (non inheritable) (since 3.23)
        /// </summary>
        public virtual bool Frame
        {
            get => GetAttribute("FRAME") == "YES";
            set => SetAttribute("FRAME", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the frame line colour. Default: the global attribute
        /// DlgFgColor.
        /// (non inheritable) (since 3.23)
        /// </summary>
        public Color FrameColor
        {
            get => Utils.ParseColor(GetAttribute("FRAMECOLOR"));
            set => SetAttribute("FRAMECOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the frame line width. Default: 1.
        /// (non inheritable) (since 3.23)
        /// </summary>
        public int FrameWidth
        {
            get => GetInt("FRAMEWIDTH", 1);
            set => SetAttribute("FRAMEWIDTH", value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region HELPERS

        private static string BarPositionToString(BarPosition p) => p switch
        {
            BarPosition.Bottom => "BOTTOM",
            BarPosition.Left => "LEFT",
            BarPosition.Right => "RIGHT",
            _ => "TOP"
        };

        private static BarPosition ParseBarPosition(string v) => v switch
        {
            "BOTTOM" => BarPosition.Bottom,
            "LEFT" => BarPosition.Left,
            "RIGHT" => BarPosition.Right,
            _ => BarPosition.Top
        };

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
            _child = null;
            _titleImage = null;
            _titleImageName = null;
            base.OnDestroying();
        }

        #region CALLBACKS

        private Callback _action;
        private IFn _actionInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called after the state has been interactively
        /// changed. Use OpenCloseCB to veto a change before it happens.
        /// (since 3.9)
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
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Expander Action callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private ExpanderStateCallback _openCloseCB;
        private IFni _openCloseCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called before the state is interactively changed.
        /// Set the callback data's Result to Ignore to veto the change.
        /// (since 3.11)
        /// </summary>
        public ExpanderStateCallback OpenCloseCB
        {
            get => _openCloseCB;
            set
            {
                _openCloseCB = value;
                _openCloseCBInternal = OpenCloseCBInternal;
                SetCallback("OPENCLOSE_CB", Utils.CastCallback<Icallback>(_openCloseCBInternal));
            }
        }
        private int OpenCloseCBInternal(nint ih, int state)
        {
            try
            {
                var cb = new ExpanderStateData(this, state);
                _openCloseCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Expander OpenCloseCB callback: {ex}");
                // Do not apply a state change the handler failed to vet.
                return (int)CallbackResult.Ignore;
            }
        }

        private ExtraButtonCallback _extraButtonCB;
        private IFnii _extraButtonCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when one of the extra bar buttons is pressed
        /// or released. Only meaningful when the constructor was given a non-zero
        /// extraButtons count.
        /// (since 3.11)
        /// </summary>
        public ExtraButtonCallback ExtraButtonCB
        {
            get => _extraButtonCB;
            set
            {
                _extraButtonCB = value;
                _extraButtonCBInternal = ExtraButtonCBInternal;
                SetCallback("EXTRABUTTON_CB", Utils.CastCallback<Icallback>(_extraButtonCBInternal));
            }
        }
        private int ExtraButtonCBInternal(nint ih, int button, int pressed)
        {
            try
            {
                var cb = new ExtraButtonData(this, button, pressed);
                _extraButtonCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Expander ExtraButtonCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}