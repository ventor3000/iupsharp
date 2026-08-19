using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Line style used by the Rectangle, Arc and Polygon primitives.
    /// </summary>
    public enum DrawStyle
    {
        /// <summary>Fill the shape.</summary>
        Fill,
        /// <summary>Stroke the outline with a continuous line. This is the default.</summary>
        Stroke,
        /// <summary>Stroke the outline with a dashed line.</summary>
        StrokeDash,
        /// <summary>Stroke the outline with a dotted line.</summary>
        StrokeDot,
        /// <summary>Stroke the outline with a dash-dot line. (since 3.25)</summary>
        StrokeDashDot,
        /// <summary>Stroke the outline with a dash-dot-dot line. (since 3.25)</summary>
        StrokeDashDotDot
    }

    /// <summary>Horizontal alignment used when drawing multi-line text.</summary>
    public enum DrawTextAlignment
    {
        /// <summary>Align to the left. This is the default.</summary>
        Left,
        /// <summary>Centre horizontally.</summary>
        Center,
        /// <summary>Align to the right.</summary>
        Right
    }

    /// <summary>
    /// A drawing session on an <see cref="IupSharp.Canvas"/> or IupBackgroundBox,
    /// wrapping the IupDraw API (since IUP 3.19).
    ///
    /// <para><b>This may only be used inside the canvas Action callback.</b> IupDraw
    /// has no meaning outside it, and calling into it from elsewhere is undefined. To
    /// force a repaint from application code, call Update or Redraw on the control and
    /// let the system invoke Action.</para>
    ///
    /// <para>All drivers are double buffered: drawing happens off-screen and only
    /// appears when the session is disposed, which calls IupDrawEnd. Use it with a
    /// <c>using</c> statement so that always happens, even if the drawing code throws.</para>
    ///
    /// <example>
    /// <code>
    /// canvas.Action = d =>
    /// {
    ///     using var g = new Draw((Control)d.Sender);
    ///     var (w, h) = g.Size;
    ///
    ///     g.Color = Color.White;
    ///     g.Style = DrawStyle.Fill;
    ///     g.Rectangle(0, 0, w - 1, h - 1);
    ///
    ///     g.Color = Color.Black;
    ///     g.Style = DrawStyle.Stroke;
    ///     g.Text("Hello", 10, 10);
    /// };
    /// </code>
    /// </example>
    ///
    /// <para>IupDraw is deliberately a small set of primitives meant for building
    /// custom controls, not a full 2D graphics library. For anything richer, use a
    /// toolkit such as CD.</para>
    /// </summary>
    public sealed class Draw : IDisposable
    {
        private readonly Control _control;
        private bool _ended;

        /// <summary>
        /// Begins a drawing session on the given control, which must be a Canvas or a
        /// BackgroundBox. Call only from inside the control's Action callback.
        /// </summary>
        /// <exception cref="ArgumentNullException">The control is null.</exception>
        /// <exception cref="ObjectDisposedException">The control has been destroyed.</exception>
        public Draw(Control control)
        {
            _control = control ?? throw new ArgumentNullException(nameof(control));

            if (_control.Handle == IntPtr.Zero)
                throw new ObjectDisposedException(control.GetType().Name);

            IupNative.IupDrawBegin(_control.Handle);
        }

        /// <summary>The control being drawn on.</summary>
        public Control Control => _control;

        /// <summary>
        /// Ends the drawing session and puts the result on screen. Calling this more
        /// than once is harmless.
        /// </summary>
        public void Dispose()
        {
            if (_ended)
                return;

            _ended = true;

            if (_control.Handle != IntPtr.Zero)
                IupNative.IupDrawEnd(_control.Handle);
        }

        private void CheckOpen()
        {
            if (_ended)
                throw new InvalidOperationException("The drawing session has already ended.");

            if (_control.Handle == IntPtr.Zero)
                throw new ObjectDisposedException(_control.GetType().Name);
        }

        #region STATE

        /// <summary>
        /// Gets or sets the color used by the primitives. Default: black. The alpha
        /// component is honoured only by drivers that support it (Direct2D, GDI+ and
        /// Cairo, since 3.25); elsewhere it is ignored.
        /// </summary>
        public Color Color
        {
            get => Utils.ParseColor(_control.GetAttribute("DRAWCOLOR"));
            set => _control.SetAttribute("DRAWCOLOR", Utils.FormatColor(value));
        }

        static readonly (string, DrawStyle)[] _styles = new[]
        {
            ("STROKE", DrawStyle.Stroke),
            ("FILL", DrawStyle.Fill),
            ("STROKE_DASH", DrawStyle.StrokeDash),
            ("STROKE_DOT", DrawStyle.StrokeDot),
            ("STROKE_DASH_DOT", DrawStyle.StrokeDashDot),
            ("STROKE_DASH_DOT_DOT", DrawStyle.StrokeDashDotDot)
        };

        /// <summary>
        /// Gets or sets whether Rectangle, Arc and Polygon are filled or stroked, and
        /// the line style when stroked. Default: Stroke. Setting Fill before Line has
        /// the same effect as Stroke.
        /// </summary>
        public DrawStyle Style
        {
            get => Utils.MapAttrib(_control.GetAttribute("DRAWSTYLE"), _styles);
            set => _control.SetAttribute("DRAWSTYLE", Utils.MapEnum(value, _styles));
        }

        /// <summary>
        /// Gets or sets the line width in pixels. Default: 1.
        /// (since 3.24)
        /// </summary>
        public int LineWidth
        {
            get
            {
                string v = _control.GetAttribute("DRAWLINEWIDTH");
                return int.TryParse(v, CultureInfo.InvariantCulture, out int w) ? w : 1;
            }
            set => _control.SetAttribute("DRAWLINEWIDTH", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the font used by Text and TextSize, in the usual IUP format
        /// such as "Helvetica, Bold -15". When not set, the control's Font is used.
        /// (since 3.22)
        /// </summary>
        public string Font
        {
            get => _control.GetAttribute("DRAWFONT");
            set => _control.SetAttribute("DRAWFONT", value);
        }

        /// <summary>
        /// Gets the name of the draw driver in use: GDI, D2D, GDI+, X11, GDK or CAIRO.
        /// Only meaningful during an open drawing session.
        /// (since 3.25)
        /// </summary>
        public string Driver => _control.GetAttribute("DRAWDRIVER");

        #endregion

        #region TEXT OPTIONS

        static readonly (string, DrawTextAlignment)[] _textAligns = new[]
        {
            ("ALEFT", DrawTextAlignment.Left),
            ("ACENTER", DrawTextAlignment.Center),
            ("ARIGHT", DrawTextAlignment.Right)
        };

        /// <summary>
        /// Gets or sets the horizontal alignment used for multi-line text.
        /// Default: Left. (since 3.22)
        /// </summary>
        public DrawTextAlignment TextAlignment
        {
            get => Utils.MapAttrib(_control.GetAttribute("DRAWTEXTALIGNMENT"), _textAligns);
            set => _control.SetAttribute("DRAWTEXTALIGNMENT", Utils.MapEnum(value, _textAligns));
        }

        /// <summary>
        /// Gets or sets whether a single-line text longer than its box is broken into
        /// multiple lines. Default: false. Ignored when the width and height passed to
        /// Text are -1 or 0. Not supported on X11.
        /// (since 3.25)
        /// </summary>
        public bool TextWrap
        {
            get => _control.GetAttribute("DRAWTEXTWRAP") == "YES";
            set => _control.SetAttribute("DRAWTEXTWRAP", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether an ellipsis replaces the invisible part of a text that
        /// is longer than its box. Default: false. Ignored when TextWrap is true, and
        /// not supported on X11.
        /// (since 3.25)
        /// </summary>
        public bool TextEllipsis
        {
            get => _control.GetAttribute("DRAWTEXTELLIPSIS") == "YES";
            set => _control.SetAttribute("DRAWTEXTELLIPSIS", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether text is clipped to its rectangle. Default: false. Text
        /// is not clipped automatically. Depending on the driver this may interfere
        /// with the region set by SetClipRect.
        /// (since 3.25)
        /// </summary>
        public bool TextClip
        {
            get => _control.GetAttribute("DRAWTEXTCLIP") == "YES";
            set => _control.SetAttribute("DRAWTEXTCLIP", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the text angle in degrees, counter-clockwise. Default: 0.
        /// Not supported on X11.
        /// (since 3.25)
        /// </summary>
        public double TextOrientation
        {
            get
            {
                string v = _control.GetAttribute("DRAWTEXTORIENTATION");
                return double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double d) ? d : 0.0;
            }
            set => _control.SetAttribute("DRAWTEXTORIENTATION", value.ToString("R", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets whether the layout of oriented text is centred inside the given
        /// rectangle. Default: false.
        /// (since 3.25)
        /// </summary>
        public bool TextLayoutCenter
        {
            get => _control.GetAttribute("DRAWTEXTLAYOUTCENTER") == "YES";
            set => _control.SetAttribute("DRAWTEXTLAYOUTCENTER", value ? "YES" : "NO");
        }

        #endregion

        #region IMAGE OPTIONS

        /// <summary>
        /// Gets or sets whether images are drawn with a disabled appearance.
        /// </summary>
        public bool MakeInactive
        {
            get => _control.GetAttribute("DRAWMAKEINACTIVE") == "YES";
            set => _control.SetAttribute("DRAWMAKEINACTIVE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the background color used for the inactive state, and when
        /// transparency has to be flattened.
        /// </summary>
        public Color BgColor
        {
            get => Utils.ParseColor(_control.GetAttribute("DRAWBGCOLOR"));
            set => _control.SetAttribute("DRAWBGCOLOR", Utils.FormatColor(value));
        }

        #endregion

        #region CLIPPING

        /// <summary>Sets a rectangular clipping region.</summary>
        public void SetClipRect(int x1, int y1, int x2, int y2)
        {
            CheckOpen();
            IupNative.IupDrawSetClipRect(_control.Handle, x1, y1, x2, y2);
        }

        /// <summary>Removes the clipping region.</summary>
        public void ResetClip()
        {
            CheckOpen();
            IupNative.IupDrawResetClip(_control.Handle);
        }

        /// <summary>
        /// Gets the clipping region previously set by SetClipRect, as
        /// (x1, y1, x2, y2). All values are zero if clipping has been reset.
        /// (since 3.25)
        /// </summary>
        public (int, int, int, int) GetClipRect()
        {
            CheckOpen();
            IupNative.IupDrawGetClipRect(_control.Handle, out int x1, out int y1, out int x2, out int y2);
            return (x1, y1, x2, y2);
        }

        #endregion

        #region PRIMITIVES

        /// <summary>Fills the whole area with the native parent's background color.</summary>
        public void ParentBackground()
        {
            CheckOpen();
            IupNative.IupDrawParentBackground(_control.Handle);
        }

        /// <summary>Draws a line, including both end points.</summary>
        public void Line(int x1, int y1, int x2, int y2)
        {
            CheckOpen();
            IupNative.IupDrawLine(_control.Handle, x1, y1, x2, y2);
        }

        /// <summary>Draws a rectangle, including the start and end points.</summary>
        public void Rectangle(int x1, int y1, int x2, int y2)
        {
            CheckOpen();
            IupNative.IupDrawRectangle(_control.Handle, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draws an arc inside the given rectangle, between two angles in degrees.
        /// Angles are counter-clockwise relative to the 3 o'clock position. When the
        /// style is Fill this draws a pie shape with its vertex at the centre of the
        /// rectangle.
        /// </summary>
        public void Arc(int x1, int y1, int x2, int y2, double angle1, double angle2)
        {
            CheckOpen();
            IupNative.IupDrawArc(_control.Handle, x1, y1, x2, y2, angle1, angle2);
        }

        /// <summary>
        /// Draws a polygon from a flat coordinate array in the sequence
        /// x1, y1, x2, y2, and so on.
        /// </summary>
        /// <exception cref="ArgumentException">The array length is odd, or it is empty.</exception>
        public void Polygon(int[] points)
        {
            CheckOpen();

            if (points == null)
                throw new ArgumentNullException(nameof(points));
            if (points.Length < 2)
                throw new ArgumentException("A polygon needs at least one point.", nameof(points));
            if (points.Length % 2 != 0)
                throw new ArgumentException(
                    "The array must hold x,y pairs, so its length must be even.", nameof(points));

            IupNative.IupDrawPolygon(_control.Handle, points, points.Length / 2);
        }

        /// <summary>
        /// Draws a polygon from a sequence of points.
        /// </summary>
        public void Polygon(IEnumerable<Point> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            var list = new List<int>();
            foreach (Point p in points)
            {
                list.Add(p.X);
                list.Add(p.Y);
            }

            Polygon(list.ToArray());
        }

        /// <summary>
        /// Draws text at the given position, using Font if set and otherwise the
        /// control's Font. Coordinates are relative to the top-left corner of the text.
        /// The '\n' character starts a new line.
        /// </summary>
        /// <param name="text">The text. Multiple lines are allowed.</param>
        /// <param name="x">Left edge of the text box.</param>
        /// <param name="y">Top edge of the text box.</param>
        /// <param name="w">
        /// Width of the text box, or -1 to use the text's own size. TextWrap and
        /// TextEllipsis have no effect when this is -1 or 0.
        /// </param>
        /// <param name="h">Height of the text box, or -1 to use the text's own size.</param>
        public void Text(string text, int x, int y, int w = -1, int h = -1)
        {
            CheckOpen();
            // len is -1 so IUP calls strlen itself; the marshaller has already
            // produced a null-terminated UTF-8 buffer.
            IupNative.IupDrawText(_control.Handle, text ?? "", -1, x, y, w, h);
        }

        /// <summary>
        /// Draws a named image. Coordinates are relative to the top-left corner of the
        /// image. The name must have been registered with IupSetHandle.
        /// </summary>
        /// <param name="name">The registered image name.</param>
        /// <param name="x">Left edge.</param>
        /// <param name="y">Top edge.</param>
        /// <param name="w">Width, or -1 to use the image's own width with no zoom.</param>
        /// <param name="h">Height, or -1 to use the image's own height with no zoom.</param>
        /// <remarks>Image zoom is not supported on X11 or GDK.</remarks>
        public void Image(string name, int x, int y, int w = -1, int h = -1)
        {
            CheckOpen();
            IupNative.IupDrawImage(_control.Handle, name, x, y, w, h);
        }

        /// <summary>Draws a selection rectangle.</summary>
        public void SelectRect(int x1, int y1, int x2, int y2)
        {
            CheckOpen();
            IupNative.IupDrawSelectRect(_control.Handle, x1, y1, x2, y2);
        }

        /// <summary>Draws a focus rectangle.</summary>
        public void FocusRect(int x1, int y1, int x2, int y2)
        {
            CheckOpen();
            IupNative.IupDrawFocusRect(_control.Handle, x1, y1, x2, y2);
        }

        #endregion

        #region INFORMATION

        /// <summary>Gets the drawing area size in pixels, as (width, height).</summary>
        public (int Width, int Height) Size
        {
            get
            {
                CheckOpen();
                IupNative.IupDrawGetSize(_control.Handle, out int w, out int h);
                return (w, h);
            }
        }

        /// <summary>
        /// Measures the given text using Font if set, and otherwise the control's Font.
        /// </summary>
        public (int Width, int Height) TextSize(string text)
        {
            CheckOpen();
            IupNative.IupDrawGetTextSize(_control.Handle, text ?? "", -1, out int w, out int h);
            return (w, h);
        }

        /// <summary>
        /// Gets a named image's size and bits per pixel, which can be 8, 24 or 32.
        /// This is a static query and does not require an open drawing session.
        /// </summary>
        public static (int Width, int Height, int Bpp) ImageInfo(string name)
        {
            IupNative.IupDrawGetImageInfo(name, out int w, out int h, out int bpp);
            return (w, h, bpp);
        }

        #endregion
    }
}