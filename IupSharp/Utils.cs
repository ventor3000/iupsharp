using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IupSharp
{
    internal static class Utils
    {
        public static string FormatPadding((int, int) p) => FormatInt(p.Item1) + "x" + FormatInt(p.Item2);

        public static string FormatSize((int, int) s) => FormatPadding(s);

        public static string FormatInt(int i) => i.ToString(CultureInfo.InvariantCulture);

        internal static T CastCallback<T>(Delegate value) where T : class
        {
            if (value == null)
                return default!;

            nint ptr = Marshal.GetFunctionPointerForDelegate(value);
            return Unsafe.As<T>(Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)));
        }

        internal static T MapAttrib<T>(string value, params(string,T)[] opts) where T:Enum
        {
            foreach(var opt in opts)
            {
                if (opt.Item1 == value)
                    return opt.Item2;
            }

            return opts[0].Item2;
        }

        internal static string MapEnum<T>(T value, params (string,T)[] opts) where T : Enum
        {
            foreach (var opt in opts)
            {
                if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(opt.Item2, value))
                    return opt.Item1;
            }

            return opts[0].Item1;
        }

        public static string FormatAlignment(Alignment align)
        {
            string left, right;

            if (align.HasFlag(Alignment.Left))
                left = "ALEFT";
            else if (align.HasFlag(Alignment.Center))
                left = "ACENTER";
            else if (align.HasFlag(Alignment.Right))
                left = "ARIGHT";
            else
                left = "";

            if (align.HasFlag(Alignment.Top))
                right = "ATOP";
            else if (align.HasFlag(Alignment.Middle))
                right = "ACENTER";
            else if (align.HasFlag(Alignment.Bottom))
                right = "ABOTTOM";
            else
                right = "";

            return left + ":" + right;
        }

        public static Alignment ParseAlignment(string str)
        {
            if (str == null)
                return Alignment.MiddleCenter;

            var parts = str.Split(':');
            Alignment align = 0;
            if (parts.Length > 0)
            {
                if (parts[0] == "ALEFT")
                    align |= Alignment.Left;
                else if (parts[0] == "ACENTER")
                    align |= Alignment.Center;
                else if (parts[0] == "ARIGHT")
                    align |= Alignment.Right;
            }
            if (parts.Length > 1)
            {
                if (parts[1] == "ATOP")
                    align |= Alignment.Top;
                else if (parts[1] == "ACENTER")
                    align |= Alignment.Middle;
                else if (parts[1] == "ABOTTOM")
                    align |= Alignment.Bottom;
            }
            return align;
        }

        /// <summary>
        /// Formats a color the way IUP expects it: "R G B", or "R G B A" when the
        /// color is not fully opaque. Color.Empty maps to null, which clears the
        /// attribute so the system default applies again.
        /// </summary>
        public static string FormatColor(Color color)
        {
            if (color.IsEmpty)
                return null;

            string s = FormatInt(color.R) + " " + FormatInt(color.G) + " " + FormatInt(color.B);

            if (color.A != 255)
                s += " " + FormatInt(color.A);

            return s;
        }

        /// <summary>
        /// Parses an IUP color attribute. Accepts the native "R G B" and "R G B A"
        /// forms as well as "#RRGGBB" and "#RRGGBBAA". Returns Color.Empty when the
        /// attribute has no value, meaning the system default applies.
        /// </summary>
        /// <exception cref="FormatException">The value is not a recognized color.</exception>
        public static Color ParseColor(string color)
        {
            if (!TryParseColor(color, out Color result) && !string.IsNullOrWhiteSpace(color))
                throw new FormatException($"Invalid color: \"{color}\"");

            return result;
        }

        /// <summary>
        /// Parses an IUP color attribute without throwing. Returns false for a
        /// malformed value; an absent value yields Color.Empty and true, since
        /// "not set" is a valid state rather than an error.
        /// </summary>
        public static bool TryParseColor(string color, out Color result)
        {
            result = Color.Empty;

            if (string.IsNullOrWhiteSpace(color))
                return true;

            color = color.Trim();

            byte r, g, b, a = 255;

            if (color.StartsWith('#'))
            {
                string hex = color.Substring(1);
                if (hex.Length != 6 && hex.Length != 8)
                    return false;

                if (!byte.TryParse(hex.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r) ||
                    !byte.TryParse(hex.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g) ||
                    !byte.TryParse(hex.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
                    return false;

                if (hex.Length == 8 &&
                    !byte.TryParse(hex.AsSpan(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a))
                    return false;
            }
            else
            {
                string[] parts = color.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3 && parts.Length != 4)
                    return false;

                if (!byte.TryParse(parts[0], CultureInfo.InvariantCulture, out r) ||
                    !byte.TryParse(parts[1], CultureInfo.InvariantCulture, out g) ||
                    !byte.TryParse(parts[2], CultureInfo.InvariantCulture, out b))
                    return false;

                if (parts.Length == 4 && !byte.TryParse(parts[3], CultureInfo.InvariantCulture, out a))
                    return false;
            }

            result = Color.FromArgb(a, r, g, b);
            return true;
        }
        /// <summary>
        /// Parses an IUP "WxH" size string (as returned by attributes like
        /// RASTERSIZE or ORIGINALSCALE) into a (width, height) tuple. Returns
        /// (0, 0) if the value is null, empty, or not in the expected format.
        /// </summary>
        public static (int, int) ParseSize(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return (0, 0);

            string[] parts = s.Split('x');
            if (parts.Length != 2)
                return (0, 0);

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int w))
                return (0, 0);
            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int h))
                return (0, 0);

            return (w, h);
        }

    }
}
