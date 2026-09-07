using System;
using System.Drawing;

namespace IupSharp
{
    /// <summary>
    /// Creates a native container that draws a border around its single child, with
    /// an optional title.
    /// </summary>
    /// <remarks>
    /// <para><b>Title is effectively creation-only.</b> IUP will not add a title to a
    /// frame that was created without one - to change it later it must have been at
    /// least an empty string at creation. The constructor therefore takes the title,
    /// and passing an empty string reserves the ability to set one afterwards.</para>
    ///
    /// <para>A frame holds one child. Put several controls in a VBox or HBox and pass
    /// that.</para>
    ///
    /// <example>
    /// <code>
    /// var box = new Frame("Paper size", new VBox(a4Toggle, letterToggle));
    ///
    /// // Untitled, but the title can still be set later:
    /// var later = new Frame("", content);
    /// later.Title = "Now it has one";
    ///
    /// // Untitled and permanently so, drawn as a sunken area:
    /// var sunken = new Frame(content) { Sunken = true };
    /// </code>
    /// </example>
    /// </remarks>
    public class Frame : ContainerControl
    {
        private Control _child;

        /// <summary>
        /// Creates a new frame with a title.
        /// </summary>
        /// <param name="title">
        /// The text shown at the top of the frame. Pass an empty string for an
        /// untitled frame whose title can still be set later; pass null, or use the
        /// other constructor, for a frame that will never have one.
        /// </param>
        /// <param name="child">
        /// The element to surround. It can be null, in which case a child can be added
        /// later with Append.
        /// </param>
        public Frame(string title, Control child)
            : base(NativeIup.IupFrame(child == null ? IntPtr.Zero : child.Handle))
        {
            _child = child;

            // Must be set at creation: IUP will not add a title to a frame that never
            // had one, though an empty string is enough to reserve the option.
            if (title != null)
                SetAttribute("TITLE", title);
        }

        /// <summary>
        /// Creates a new frame with no title. A title cannot be added later - use the
        /// other constructor with an empty string if you may want one.
        /// </summary>
        /// <param name="child">
        /// The element to surround. It can be null, in which case a child can be added
        /// later with Append.
        /// </param>
        public Frame(Control child = null) : this(null, child)
        {
        }

        /// <summary>
        /// Gets the single child of the frame, or null if it has none.
        /// </summary>
        public Control Child => _child;

        /// <summary>
        /// Sets the single child of the frame. A frame accepts only one child; wrap
        /// several controls in a VBox or HBox.
        /// </summary>
        /// <exception cref="InvalidOperationException">The frame already has a child.</exception>
        public override void Append(Control child)
        {
            if (_child != null)
                throw new InvalidOperationException(
                    "A Frame accepts only one child. Wrap the controls in a VBox or HBox.");

            base.Append(child);
            _child = child;
        }

        #region ATTRIBUTES

        /// <summary>
        /// Gets or sets the text shown at the top of the frame.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// Setting this has no effect unless the frame was created with a title, even
        /// an empty one. That is an IUP restriction, not a wrapper one.
        /// </remarks>
        public string Title
        {
            get => GetAttribute("TITLE") ?? "";
            set => SetAttribute("TITLE", value);
        }

        /// <summary>
        /// Gets or sets whether the border is drawn as a sunken area rather than a
        /// plain line. Only applies to a frame with no title. Default: false.
        /// </summary>
        public virtual bool Sunken
        {
            get => GetAttribute("SUNKEN") == "YES";
            set => SetAttribute("SUNKEN", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets a position offset for the child, in pixels. It does not affect
        /// the natural size, and allows positioning controls outside the client area.
        /// Default: (0,0).
        /// (since 3.14)
        /// </summary>
        public (int, int) ChildOffset
        {
            get { CheckAlive(); NativeIup.GetIntInt(Handle, "CHILDOFFSET", out int x, out int y); return (x, y); }
            set => SetAttribute("CHILDOFFSET", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets the background colour. Normally ignored and transparent, using
        /// the background of the native parent - except on a frame with no title where
        /// this is set before mapping, which gives the frame a coloured background
        /// that can then be changed freely.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; }

        /// <summary>
        /// Gets or sets the title text colour. Not available on Windows when using
        /// Visual Styles.
        /// </summary>
        public override Color FgColor { get => base.FgColor; set => base.FgColor = value; }

        #endregion

        protected override void OnDestroying()
        {
            _child = null;
            base.OnDestroying();
        }
    }



}