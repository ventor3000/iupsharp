using System;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates a label that displays an image changed periodically, producing an
    /// animation. (since 3.17)
    /// </summary>
    /// <remarks>
    /// <para>This derives from Label because IUP does: an animated label inherits
    /// every Label attribute and callback, and unlike Link it uses none of them
    /// internally, so the whole Label surface is safe to use here.</para>
    ///
    /// <para>The Label Image property is driven by the animation timer, so setting it
    /// yourself while the animation runs has no lasting effect.</para>
    ///
    /// <para><b>The animation starts stopped.</b> Call Start after assigning
    /// one.</para>
    ///
    /// <example>
    /// Indefinite progress using the stock animation, which needs no images of your
    /// own:
    /// <code>
    /// ImageLib.Open();
    ///
    /// var busy = new AnimatedLabel();
    /// busy.AnimationName = ImageLib.CircleProgressAnimation;
    /// busy.Start();
    /// </code>
    /// With frames built by the application:
    /// <code>
    /// var anim = new Animation(frame0, frame1, frame2) { FrameTime = 100 };
    /// var label = new AnimatedLabel(anim);
    /// label.Start();
    /// </code>
    /// </example>
    /// </remarks>
    public class AnimatedLabel : Label
    {
        /// <summary>
        /// Creates a new animated label.
        /// </summary>
        /// <param name="animation">
        /// The frames to show. It can be null, in which case an animation can be
        /// assigned later through Animation or AnimationName.
        /// </param>
        public AnimatedLabel(Animation animation = null)
            : base(IupNative.IupAnimatedLabel(
                animation == null ? IntPtr.Zero : animation.Handle))
        {
            _animation = animation;
        }

        #region ANIMATION

        private Animation _animation;
        private string _animationName;

        /// <summary>
        /// Gets or sets the animation shown by this label.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// <para>Assigning this picks up the animation's FrameTime, if it has one, and
        /// applies it to this label - so set FrameTime here rather than on the
        /// animation if you want to override it.</para>
        ///
        /// <para>This and <see cref="AnimationName"/> set the same IUP attribute, so
        /// assigning either clears the other. Reading this returns null when the
        /// animation was set by name.</para>
        ///
        /// <para>The label does not own the animation, so it is not destroyed with the
        /// label.</para>
        /// </remarks>
        public virtual Animation Animation
        {
            get => _animation;
            set
            {
                CheckAlive();

                _animation = value;
                _animationName = null;

                IupNative.SetAttributeHandle(Handle, "ANIMATION_HANDLE",
                    value == null ? IntPtr.Zero : value.Handle);
            }
        }

        /// <summary>
        /// Gets or sets the animation by name rather than by object. This is how the
        /// stock animation from ImageLib is used.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// <para>The name must refer to an element registered with IupSetHandle that
        /// holds the frames, or to a stock animation such as
        /// <c>ImageLib.CircleProgressAnimation</c> once ImageLib.Open has been
        /// called.</para>
        ///
        /// <para>IUP resolves the name lazily and shows nothing if it does not match,
        /// so a typo produces a blank label rather than an error.</para>
        ///
        /// <para>This and <see cref="Animation"/> set the same IUP attribute, so
        /// assigning either clears the other. Reading this returns null when the
        /// animation was set as an object.</para>
        /// </remarks>
        public virtual string AnimationName
        {
            get => _animationName;
            set
            {
                CheckAlive();

                _animationName = value;
                _animation = null;

                SetAttribute("ANIMATION", value);
            }
        }

        /// <summary>
        /// Gets the number of frames in the current animation, as IUP reports it.
        /// (read only) (non inheritable)
        /// </summary>
        public int FrameCount
        {
            get
            {
                string v = GetAttribute("FRAMECOUNT");
                return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)
                    ? i
                    : 0;
            }
        }

        #endregion

        #region PLAYBACK

        /// <summary>
        /// Starts the animation. It is stopped when the label is created, so this must
        /// be called for anything to move.
        /// </summary>
        public void Start() => SetAttribute("START", "YES");

        /// <summary>Stops the animation, leaving the current frame displayed.</summary>
        public void Stop() => SetAttribute("STOP", "YES");

        /// <summary>
        /// Gets whether the animation is currently running.
        /// (read only) (non inheritable)
        /// </summary>
        public bool Running => GetAttribute("RUNNING") == "YES";

        /// <summary>
        /// Gets or sets the time between frames, in milliseconds. Setting an Animation
        /// that carries its own FrameTime overwrites this, so assign it afterwards to
        /// override.
        /// (non inheritable)
        /// </summary>
        public int FrameTime
        {
            get
            {
                string v = GetAttribute("FRAMETIME");
                return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)
                    ? i
                    : 0;
            }
            set => SetAttribute("FRAMETIME", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets whether the animation stops automatically while the label is
        /// hidden, so a hidden spinner does not keep the timer running.
        /// Default: true.
        /// (non inheritable) (since 3.18)
        /// </summary>
        public virtual bool StopWhenHidden
        {
            get => GetAttribute("STOPWHENHIDDEN") != "NO";
            set => SetAttribute("STOPWHENHIDDEN", value ? "YES" : "NO");
        }

        #endregion

        protected override void OnDestroying()
        {
            _animation = null;
            _animationName = null;
            base.OnDestroying();
        }
    }
}