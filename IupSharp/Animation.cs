using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// A sequence of images shown one after another by an
    /// <see cref="AnimatedLabel"/>.
    /// </summary>
    /// <remarks>
    /// <para>IUP has no dedicated animation element. An animation is an IupUser
    /// element holding several images as children, which is what this class wraps -
    /// so an Animation is a container of Images, not an Image itself.</para>
    ///
    /// <para>Every frame should be the same size. IUP does not enforce it, and a
    /// mismatched frame simply changes the label's size mid-animation.</para>
    ///
    /// <para><b>Ownership.</b> Frames become children of the animation, so destroying
    /// the animation destroys them. The animation itself is not owned by the label
    /// using it, so destroy it explicitly or leave it to Iup.Close.</para>
    ///
    /// <example>
    /// <code>
    /// var spinner = new Animation(frame0, frame1, frame2) { FrameTime = 83 };
    /// var label = new AnimatedLabel(spinner);
    /// label.Start();
    /// </code>
    /// For indefinite progress the stock animation avoids all of this:
    /// <code>
    /// var label = new AnimatedLabel();
    /// label.AnimationName = ImageLib.CircleProgressAnimation;
    /// label.Start();
    /// </code>
    /// </example>
    /// </remarks>
    public class Animation : MappableObject, IEnumerable<Image>
    {
        private readonly System.Collections.Generic.List<Image> _frames = new();

        /// <summary>
        /// Creates a new animation from the given frames, in order. It can also be
        /// created empty and filled later with Append.
        /// </summary>
        public Animation(params Image[] frames) : base(IupNative.IupUser())
        {
            if (frames == null)
                return;

            foreach (Image frame in frames)
                Append(frame);
        }

        #region FRAMES

        /// <summary>Gets the number of frames.</summary>
        public int Count => _frames.Count;

        /// <summary>Gets the frame at the given zero based position.</summary>
        public Image this[int index] => _frames[index];

        /// <summary>
        /// Gets the number of frames as IUP reports it, by counting the children of
        /// the underlying element. Should always equal <see cref="Count"/>; a
        /// difference means something has manipulated the element behind this
        /// wrapper's back.
        /// </summary>
        public int NativeFrameCount
        {
            get
            {
                CheckAlive();
                return IupNative.IupGetChildCount(Handle);
            }
        }

        /// <summary>
        /// Adds a frame at the end. All frames should share the same size.
        /// </summary>
        /// <exception cref="ArgumentNullException">The frame is null.</exception>
        /// <exception cref="IupException">IUP rejected the frame.</exception>
        public virtual void Append(Image frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            CheckAlive();

            if (IupNative.IupAppend(Handle, frame.Handle) == IntPtr.Zero)
                throw new IupException("Failed to append the frame to the animation.");

            _frames.Add(frame);
        }

        /// <summary>Adds several frames at the end, in order.</summary>
        public void AppendRange(IEnumerable<Image> frames)
        {
            if (frames == null)
                return;

            foreach (Image frame in frames)
                Append(frame);
        }

        public IEnumerator<Image> GetEnumerator() => _frames.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _frames.GetEnumerator();

        #endregion

        #region ATTRIBUTES

        /// <summary>
        /// Gets or sets the time between frames, in milliseconds. An AnimatedLabel
        /// picks this up when the animation is assigned to it, though the label's own
        /// FrameTime can be changed afterwards to override it.
        /// </summary>
        /// <remarks>
        /// Returns 0 when not set, in which case the label uses its own default. The
        /// stock IUP_CircleProgressAnimation uses 83 ms, about one second per turn
        /// across its twelve frames.
        /// </remarks>
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

        #endregion

        protected override void OnDestroying()
        {
            // IUP destroys the frames with the animation, so drop the references
            // without destroying them again.
            _frames.Clear();
            base.OnDestroying();
        }
    }
}