using System;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates a 32 bit-per-pixel RGBA image, to be shown on a label, button,
    /// toggle, or as a cursor.
    /// </summary>
    /// <remarks>
    /// Images created with the IupImage* constructors can be reused in different
    /// elements. They should be destroyed (via Destroy()/Dispose()) when no longer
    /// necessary, but only once the controls that use them have been destroyed
    /// first - an image cannot be destroyed while still in use.
    /// </remarks>
    public class ImageRGBA : IupObject
    {
        /// <summary>
        /// Creates a new ImageRGBA from a width, height, and a pixel buffer.
        /// </summary>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="pixels">
        /// Pixel data, top to bottom and left to right, 4 bytes per pixel in
        /// R, G, B, A order. Must contain exactly width * height * 4 bytes.
        /// The array is duplicated internally by IUP, so it can be discarded
        /// or reused after the call.
        /// </param>
        public ImageRGBA(int width, int height, byte[] pixels)
            : base(IupNative.IupImageRGBA(width, height, ValidatePixels(width, height, pixels)))
        {
        }

        private static byte[] ValidatePixels(int width, int height, byte[] pixels)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));

            long expected = (long)width * height * 4;
            if (pixels.Length != expected)
                throw new ArgumentException(
                    $"Expected {expected} bytes for a {width}x{height} RGBA image (4 bytes/pixel), but got {pixels.Length}.",
                    nameof(pixels));

            return pixels;
        }

        /// <summary>
        /// Gets or sets the automatic scale factor applied to the image. Can be
        /// "DPI" or a real scale factor. If not set, the global IMAGEAUTOSCALE
        /// attribute is used. The minimum resulting size when automatically
        /// resized is 24 pixels height.
        /// </summary>
        public string AutoScale
        {
            get => GetAttribute("AUTOSCALE");
            set => SetAttribute("AUTOSCALE", value);
        }

        /// <summary>
        /// Gets or sets the color used for transparency compositing. If not
        /// defined, the BGCOLOR of the control that contains the image is
        /// used instead.
        /// </summary>
        public uint BgColor
        {
            get => Utils.ParseColor(GetAttribute("BGCOLOR"));
            set => SetAttribute("BGCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets the number of bits per pixel in the image. Always 32 for
        /// ImageRGBA. (read-only)
        /// </summary>
        public int BPP => int.Parse(GetAttribute("BPP"), CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets the number of channels in the image. Always 4 for ImageRGBA.
        /// (read-only)
        /// </summary>
        public int Channels => int.Parse(GetAttribute("CHANNELS"), CultureInfo.InvariantCulture);

        /// <summary>
        /// Clears the internal native image cache, so WID can be dynamically
        /// changed. (write-only)
        /// </summary>
        public void ClearCache() => SetAttribute("CLEARCACHE", "YES");

        /// <summary>
        /// Gets or sets the resolution expected for display, used when
        /// AutoScale is "DPI". If not defined, the global IMAGESDPI attribute
        /// is used.
        /// </summary>
        public string Dpi
        {
            get => GetAttribute("DPI");
            set => SetAttribute("DPI", value);
        }

        /// <summary>
        /// Gets the image height in pixels. (read-only)
        /// </summary>
        public int Height => int.Parse(GetAttribute("HEIGHT"), CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets or sets the hotspot: the position inside a cursor image
        /// indicating the mouse-click spot, as (x, y) coordinates in pixels.
        /// Default: (0, 0).
        /// </summary>
        public (int, int) Hotspot
        {
            get
            {
                string value = GetAttribute("HOTSPOT");
                if (string.IsNullOrEmpty(value)) return (0, 0);

                string[] parts = value.Split(':');
                if (parts.Length != 2) return (0, 0);
                if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)) return (0, 0);
                if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)) return (0, 0);

                return (x, y);
            }
            set => SetAttribute("HOTSPOT",
                value.Item1.ToString(CultureInfo.InvariantCulture) + ":" +
                value.Item2.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets the size the image had before it was scaled, as (width, height)
        /// in pixels. (read-only)
        /// </summary>
        public (int, int) OriginalScale => Utils.ParseSize(GetAttribute("ORIGINALSCALE"));

        /// <summary>
        /// Gets the image size in pixels, as (width, height). (read-only)
        /// </summary>
        public (int, int) RasterSize => Utils.ParseSize(GetAttribute("RASTERSIZE"));

        /// <summary>
        /// Reallocates the image to a new size, given as (width, height) in
        /// pixels. The WIDTH and HEIGHT attributes are updated, but the image
        /// contents are undefined (trashed) after the reshape. (write-only)
        /// </summary>
        public void Reshape(int width, int height) =>
            SetAttribute("RESHAPE", Utils.FormatSize((width, height)));

        /// <summary>
        /// Resizes the image to a new size, given as (width, height) in
        /// pixels, using bilinear interpolation. The WIDTH and HEIGHT
        /// attributes are updated and existing content is preserved and
        /// resampled. (write-only)
        /// </summary>
        public void Resize(int width, int height) =>
            SetAttribute("RESIZE", Utils.FormatSize((width, height)));

        /// <summary>
        /// Gets whether the image has been resized. (read-only)
        /// </summary>
        public bool Scaled => GetAttribute("SCALED") == "YES";

        /// <summary>
        /// Gets the internal pixel data pointer. (read-only)
        /// </summary>
        /// <remarks>
        /// This is a raw pointer into IUP's native memory, valid only while
        /// the image is alive and only meaningful to interop code; IupSharp
        /// does not manage or dereference it.
        /// </remarks>
        public IntPtr Wid => GetAttributePtr("WID");

        /// <summary>
        /// Gets the image width in pixels. (read-only)
        /// </summary>
        public int Width => int.Parse(GetAttribute("WIDTH"), CultureInfo.InvariantCulture);

        /// <summary>
        /// Saves the image to a file in a textual format understood by IUP
        /// (e.g. LED or C source), under the given resource name.
        /// </summary>
        /// <param name="filename">Path of the file to write.</param>
        /// <param name="format">Format to save as, e.g. "LED" or "C".</param>
        /// <param name="name">Resource name to give the image in the saved file.</param>
        /// <returns>True if the image was saved successfully.</returns>
        public bool SaveAsText(string filename, string format, string name)
        {
            CheckAlive();
            return IupNative.SaveImageAsText(Handle, filename, format, name) != 0;
        }
    }
}