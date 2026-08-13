using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IupSharp
{
    internal static class Utils
    {
        public static string FormatPadding((int, int) p) => FormatInt(p.Item1) + "x" + FormatInt(p.Item2);

        /// <summary>
        /// Formats a width/height or x/y pair as IUP expects it: "WxH".
        /// Identical to FormatPadding; kept separate only for readable call sites.
        /// You may prefer to delete FormatPadding and use this everywhere.
        /// </summary>
        public static string FormatSize((int, int) s) =>
            Utils.FormatInt(s.Item1) + "x" + Utils.FormatInt(s.Item2);

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


        public static string FormatColor(uint color)
        {
            byte a = (byte)((color & 0xff000000) >> 24);
            byte r = (byte)((color & 0xff0000) >> 16);
            byte g = (byte)((color & 0xff00) >> 8);
            byte b = (byte)(color & 0xff);

            if (a != 255)
                return
                    r.ToString(CultureInfo.InvariantCulture) + " "
                    + g.ToString(CultureInfo.InvariantCulture) + " "
                    + b.ToString(CultureInfo.InvariantCulture) + " "
                    + a.ToString(CultureInfo.InvariantCulture);
            else
                return r.ToString(CultureInfo.InvariantCulture) + " "
                    + g.ToString(CultureInfo.InvariantCulture) + " "
                    + b.ToString(CultureInfo.InvariantCulture);

        }
        public static uint ParseColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return 0; // TODO: what color should we return in this case?

            color = color.Trim();

            byte r, g, b, a = 255;

            if (color.StartsWith('#'))
            {
                string hex = color.Substring(1);
                if (hex.Length != 6 && hex.Length != 8)
                    throw new FormatException($"Invalid hex color: \"{color}\"");

                r = byte.Parse(hex.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                g = byte.Parse(hex.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                b = byte.Parse(hex.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                if (hex.Length == 8)
                    a = byte.Parse(hex.AsSpan(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            else
            {
                string[] parts = color.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3 && parts.Length != 4)
                    throw new FormatException($"Invalid color: \"{color}\"");

                r = byte.Parse(parts[0], CultureInfo.InvariantCulture);
                g = byte.Parse(parts[1], CultureInfo.InvariantCulture);
                b = byte.Parse(parts[2], CultureInfo.InvariantCulture);
                if (parts.Length == 4)
                    a = byte.Parse(parts[3], CultureInfo.InvariantCulture);
            }

            return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
        }

    }
}
