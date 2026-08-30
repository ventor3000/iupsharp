// ======================================================================
// TEMPORARY NAMES - see the note at the bottom of this file.
//
// The base class ships as ImageRGBA and the concrete 32-bit class as
// ImageRGBA32, so that every existing reference in Button, Label,
// DropButton and the rest still compiles. Use Visual Studio's Rename
// twice, in this order:
//
//   1. ImageRGBA   -> Image        (updates all existing call sites)
//   2. ImageRGBA32 -> ImageRGBA
//
// After that the file matches its intended shape and this banner and the
// closing note can be deleted.
// ======================================================================

using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Base class for the three IUP image formats: <see cref="Image8"/> (8 bits per
    /// pixel, paletted), <see cref="ImageRGB"/> (24 bits) and
    /// <see cref="Image"/> (32 bits with alpha).
    /// </summary>
    /// <remarks>
    /// <para>An image is not a control - it has no size, position or focus of its
    /// own, only the attributes below. It is attached to a control through that
    /// control's image properties, and one image can be shared by several
    /// controls.</para>
    ///
    /// <para><b>Ownership.</b> Setting an image on a control does not transfer
    /// ownership, so destroying the control leaves the image alive. Images should be
    /// destroyed once the controls using them have been destroyed, or left to
    /// Iup.Close, which is the usual choice for icons that live as long as the
    /// application.</para>
    ///
    /// <para>Typical sizes: application icons 32x32, toolbar bitmaps 24x24 or
    /// smaller, menu bitmaps and small icons 16x16 or smaller.</para>
    /// </remarks>
    public abstract class Image : IupObject
    {
        protected Image(nint handle) : base(handle)
        {
        }

        /// <summary>
        /// Validates the dimensions and buffer length for a given number of bytes per
        /// pixel, and returns the buffer so it can be passed straight to the native
        /// constructor.
        /// </summary>
        protected static byte[] ValidatePixels(int width, int height, byte[] pixels,
                                               int bytesPerPixel, string formatName)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));

            long expected = (long)width * height * bytesPerPixel;
            if (pixels.Length != expected)
                throw new ArgumentException(
                    $"Expected {expected} bytes for a {width}x{height} {formatName} image " +
                    $"({bytesPerPixel} byte{(bytesPerPixel == 1 ? "" : "s")}/pixel), but got {pixels.Length}.",
                    nameof(pixels));

            return pixels;
        }

        #region SIZE

        /// <summary>Gets the image width in pixels. (read only)</summary>
        public int Width => GetInt("WIDTH", 0);

        /// <summary>Gets the image height in pixels. (read only)</summary>
        public int Height => GetInt("HEIGHT", 0);

        /// <summary>
        /// Gets the image size in pixels, as (width, height).
        /// (read only) (since 3.0)
        /// </summary>
        public (int, int) RasterSize => Utils.ParseSize(GetAttribute("RASTERSIZE"));

        /// <summary>
        /// Gets the size the image had before it was scaled, as (width, height).
        /// (read only) (since 3.25)
        /// </summary>
        public (int, int) OriginalScale => Utils.ParseSize(GetAttribute("ORIGINALSCALE"));

        /// <summary>
        /// Gets whether the image has been resized.
        /// (read only) (since 3.25)
        /// </summary>
        public bool Scaled => GetAttribute("SCALED") == "YES";

        /// <summary>
        /// Resizes the image, resampling the existing contents. Uses bilinear
        /// interpolation for RGB and RGBA, and nearest neighbour for 8 bit images.
        /// (write only) (since 3.24)
        /// </summary>
        public void Resize(int width, int height) =>
            SetAttribute("RESIZE", Utils.FormatSize((width, height)));

        /// <summary>
        /// Reallocates the image for a new size. The contents are NOT preserved and
        /// will be undefined afterwards - fill them through Pixels before use.
        /// (write only) (since 3.24)
        /// </summary>
        public void Reshape(int width, int height) =>
            SetAttribute("RESHAPE", Utils.FormatSize((width, height)));

        #endregion

        #region FORMAT

        /// <summary>
        /// Gets the number of bits per pixel: 8 for Image8, 24 for ImageRGB and 32 for
        /// ImageRGBA.
        /// (read only) (since 3.0)
        /// </summary>
        public int Bpp => GetInt("BPP", 0);

        /// <summary>
        /// Gets the number of channels: 1 for Image8, 3 for ImageRGB and 4 for
        /// ImageRGBA.
        /// (read only) (since 3.0)
        /// </summary>
        public int Channels => GetInt("CHANNELS", 0);

        /// <summary>
        /// Gets the number of bytes each pixel occupies in the buffer returned by Wid,
        /// which is the same as Channels.
        /// </summary>
        public int BytesPerPixel => Channels;

        #endregion

        #region APPEARANCE

        /// <summary>
        /// Gets or sets the colour used for transparency. When not set, the BgColor of
        /// the control containing the image is used instead.
        /// </summary>
        public Color BgColor
        {
            get => Utils.ParseColor(GetAttribute("BGCOLOR"));
            set => SetAttribute("BGCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets automatic scaling: either "DPI" or a numeric scale factor.
        /// When not set the global IMAGEAUTOSCALE attribute applies. The minimum
        /// resulting height when scaled automatically is 24 pixels.
        /// (since 3.16)
        /// </summary>
        public string AutoScale
        {
            get => GetAttribute("AUTOSCALE");
            set => SetAttribute("AUTOSCALE", value);
        }

        /// <summary>
        /// Gets or sets the resolution expected for display, used when AutoScale is
        /// "DPI". When not set the global IMAGESDPI attribute applies.
        /// (since 3.23)
        /// </summary>
        public string Dpi
        {
            get => GetAttribute("DPI");
            set => SetAttribute("DPI", value);
        }

        /// <summary>
        /// Gets or sets the hotspot for a cursor image: the (x, y) position of the
        /// click point inside the image, in pixels. Default: (0, 0).
        /// </summary>
        public (int, int) Hotspot
        {
            get
            {
                string value = GetAttribute("HOTSPOT");
                if (string.IsNullOrEmpty(value))
                    return (0, 0);

                string[] parts = value.Split(':');
                if (parts.Length != 2)
                    return (0, 0);

                int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x);
                int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y);
                return (x, y);
            }
            set => SetAttribute("HOTSPOT",
                value.Item1.ToString(CultureInfo.InvariantCulture) + ":" +
                value.Item2.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region PIXEL DATA

        /// <summary>
        /// Gets the internal pixel data pointer. Valid only while the image is alive.
        /// (read only) (since 3.0)
        /// </summary>
        /// <remarks>
        /// Prefer Pixels, which wraps this in a bounds-checked span. After writing
        /// through either, call ClearCache or the change will not become visible.
        /// </remarks>
        public IntPtr Wid => GetAttributePtr("WID");

     

        /// <summary>
        /// Discards IUP's cached native bitmap so edits made through Pixels or Wid
        /// become visible.
        /// (write only) (since 3.24)
        /// </summary>
        public void ClearCache() => SetAttribute("CLEARCACHE", "YES");

        #endregion

        #region SAVING

        /// <summary>
        /// Saves the image to a file in a textual format understood by IUP, such as
        /// LED or C source, under the given resource name.
        /// </summary>
        /// <param name="filename">Path of the file to write.</param>
        /// <param name="format">Format to save as, for example "LED" or "C".</param>
        /// <param name="name">Resource name given to the image in the saved file.</param>
        /// <returns>True if the image was saved successfully.</returns>
        public bool SaveAsText(string filename, string format, string name)
        {
            CheckAlive();
            return IupNative.SaveImageAsText(Handle, filename, format, name) != 0;
        }

        #endregion

        #region HELPERS

        private protected int GetInt(string name, int fallback)
        {
            string v = GetAttribute(name);
            return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)
                ? i
                : fallback;
        }

        #endregion
    }


    /// <summary>
    /// Creates an 8 bit-per-pixel paletted image, where each byte is an index into a
    /// colour table of up to 256 entries.
    /// </summary>
    /// <remarks>
    /// <para>This is the only IUP image format that supports the "BGCOLOR" palette
    /// entry, which makes those pixels take on the background colour of whatever
    /// control the image sits in. That is how IUP's own icons blend into a toolbar or
    /// dialog whose background is not known in advance - something the RGB and RGBA
    /// formats cannot express.</para>
    ///
    /// <para>The first 16 palette entries have sensible defaults, so a small icon
    /// often needs no colours set at all. Beyond 16, <b>every</b> index up to the
    /// largest one used must be defined, even those the image never references.</para>
    ///
    /// <para>A "BGCOLOR" entry must be at an index below 16.</para>
    ///
    /// <example>
    /// <code>
    /// var icon = new Image8(3, 3, new byte[]
    /// {
    ///     1, 0, 1,
    ///     0, 2, 0,
    ///     1, 0, 1
    /// });
    ///
    /// icon.SetTransparent(0);                  // index 0 shows the parent background
    /// icon.SetColor(1, Color.Black);
    /// icon.SetColor(2, Color.Red);
    /// </code>
    /// </example>
    /// </remarks>
    public class Image8 : Image
    {
        /// <summary>The palette value that makes an index transparent.</summary>
        private const string TransparentValue = "BGCOLOR";

        /// <summary>The highest palette index at which "BGCOLOR" may be placed.</summary>
        private const int MaxTransparentIndex = 15;

        /// <summary>
        /// Creates a new 8 bit paletted image.
        /// </summary>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="pixels">
        /// One byte per pixel, each a palette index, ordered top to bottom and left to
        /// right. Must be exactly width * height bytes. IUP copies the array, so it can
        /// be reused afterwards.
        /// </param>
        public Image8(int width, int height, byte[] pixels)
            : base(IupNative.IupImage(width, height,
                ValidatePixels(width, height, pixels, 1, "8 bit")))
        {
        }

        /// <summary>
        /// Creates a new 8 bit paletted image with an uninitialised buffer, to be
        /// filled through Pixels.
        /// </summary>
        public Image8(int width, int height)
            : this(width, height, new byte[checked(width * height)])
        {
        }

        /// <summary>
        /// Sets the colour of a palette index. Indices run from 0 to 255.
        /// </summary>
        /// <remarks>
        /// The first 16 indices have defaults, so they need setting only to override
        /// them. For images using more than 16 colours, every index from 16 up to the
        /// largest one used must be defined, even if some are never referenced.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">The index is outside 0 to 255.</exception>
        public void SetColor(int index, Color color)
        {
            CheckIndex(index);
            SetAttribute(index.ToString(CultureInfo.InvariantCulture), Utils.FormatColor(color));
        }

        /// <summary>
        /// Gets the colour of a palette index, or Color.Empty when the index has not
        /// been set or is transparent. Use IsTransparent to tell the two apart.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The index is outside 0 to 255.</exception>
        public Color GetColor(int index)
        {
            CheckIndex(index);

            string v = GetAttribute(index.ToString(CultureInfo.InvariantCulture));
            if (string.IsNullOrEmpty(v) || v == TransparentValue)
                return Color.Empty;

            return Utils.ParseColor(v);
        }

        /// <summary>
        /// Makes a palette index transparent, so pixels using it take the background
        /// colour of the control containing the image.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The index is outside 0 to 15. IUP requires a transparent entry to be below
        /// index 16.
        /// </exception>
        public void SetTransparent(int index)
        {
            if (index < 0 || index > MaxTransparentIndex)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"A transparent palette entry must be at an index from 0 to {MaxTransparentIndex}.");

            SetAttribute(index.ToString(CultureInfo.InvariantCulture), TransparentValue);
        }

        /// <summary>
        /// Gets whether a palette index is transparent.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The index is outside 0 to 255.</exception>
        public bool IsTransparent(int index)
        {
            CheckIndex(index);
            return GetAttribute(index.ToString(CultureInfo.InvariantCulture)) == TransparentValue;
        }

        private static void CheckIndex(int index)
        {
            if (index < 0 || index > 255)
                throw new ArgumentOutOfRangeException(nameof(index),
                    "Palette indices run from 0 to 255.");
        }
    }


    /// <summary>
    /// Creates a 24 bit-per-pixel RGB image, with no transparency.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Image"/> when transparency is needed, or
    /// <see cref="Image8"/> for a small icon that should blend into its parent's
    /// background. This format's only advantage over RGBA is a quarter less memory.
    /// (since 3.0)
    /// </remarks>
    public class ImageRGB : Image
    {
        /// <summary>
        /// Creates a new 24 bit RGB image.
        /// </summary>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="pixels">
        /// Three bytes per pixel in R, G, B order, ordered top to bottom and left to
        /// right. Must be exactly width * height * 3 bytes. IUP copies the array, so it
        /// can be reused afterwards.
        /// </param>
        public ImageRGB(int width, int height, byte[] pixels)
            : base(IupNative.IupImageRGB(width, height,
                ValidatePixels(width, height, pixels, 3, "RGB")))
        {
        }

        /// <summary>
        /// Creates a new 24 bit RGB image with an uninitialised buffer, to be filled
        /// through Pixels.
        /// </summary>
        public ImageRGB(int width, int height)
            : this(width, height, new byte[checked(width * height * 3)])
        {
        }
    }


    /// <summary>
    /// Creates a 32 bit-per-pixel RGBA image, with an alpha channel.
    /// </summary>
    /// <remarks>
    /// <para>On Motif the alpha channel is always composited against the control's
    /// BgColor before the image is set. On Windows and GTK the system composites it,
    /// except that Windows needs it composited in advance for Item and Submenu
    /// always, and for Toggle when Visual Styles are off. Where compositing happens in
    /// advance and the control background is not uniform, the transparent areas may
    /// not match exactly.</para>
    /// (since 3.0)
    /// </remarks>
    public class ImageRGBA : Image
    {
        /// <summary>
        /// Creates a new 32 bit RGBA image.
        /// </summary>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="pixels">
        /// Four bytes per pixel in R, G, B, A order, ordered top to bottom and left to
        /// right. Must be exactly width * height * 4 bytes. IUP copies the array, so it
        /// can be reused afterwards.
        /// </param>
        public ImageRGBA(int width, int height, byte[] pixels)
            : base(IupNative.IupImageRGBA(width, height,
                ValidatePixels(width, height, pixels, 4, "RGBA")))
        {
        }

        /// <summary>
        /// Creates a new 32 bit RGBA image with an uninitialised buffer, to be filled
        /// through Pixels. Note that a zeroed buffer is fully transparent black.
        /// </summary>
        public ImageRGBA(int width, int height)
            : this(width, height, new byte[checked(width * height * 4)])
        {
        }
    }
}

// ======================================================================
// After the two renames the hierarchy reads:
//
//   Image            abstract base: size, format, palette-free attributes
//    +- Image8       8 bpp paletted, supports the "BGCOLOR" palette entry
//    +- ImageRGB     24 bpp, no transparency
//    +- ImageRGBA    32 bpp with alpha
//
// Note the base is abstract, so any "new ImageRGBA(w, h, pixels)" written
// before this change resolves to the concrete class only AFTER step 2.
// Between the two renames such a call will not compile.
// ======================================================================