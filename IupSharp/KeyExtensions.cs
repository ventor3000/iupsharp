using System;
using System.Text;

namespace IupSharp
{
    /// <summary>
    /// Helpers for decomposing <see cref="Key"/> values, mirroring the iup_Xkey*
    /// macros in iupkey.h.
    ///
    /// <para><b>Encoding.</b> Modifiers occupy the top four bits (0xF0000000) and the
    /// base key the low 28 bits (0x0FFFFFFF). Since IUP 3.9 the modifiers are
    /// independent flags, so any modifier combines with any key and modifiers may be
    /// mixed freely.</para>
    ///
    /// <para><b>Base key values.</b> Printable ASCII uses the character code, so
    /// Key.A is 65. Everything else uses the same values as X11 and GDK. In the
    /// Latin-1 range (0xA0-0xFF) those keysyms coincide with Unicode code points by
    /// design, which is why 'å' arrives as 229 (0xE5). Above
    /// Latin-1 they do not coincide: X11 encodes other characters as
    /// 0x01000000 | codepoint, and that range extends to 0x0110FFFF, which is still
    /// comfortably inside the 28-bit base mask.</para>
    ///
    /// <para><b>K_ANY is a key channel, not a text channel.</b> Composed input from an
    /// IME (Chinese, Japanese, Korean) is delivered to the control as text, not as
    /// key events, so it will not appear here. Read composed text from the relevant
    /// control's ACTION or VALUECHANGED callback instead.</para>
    /// </summary>
    public static class KeyExtensions
    {
        private const int ShiftBit = 0x10000000;
        private const int CtrlBit = 0x20000000;
        private const int AltBit = 0x40000000;
        private const int SysBit = unchecked((int)0x80000000);

        /// <summary>Mask covering all four modifier bits.</summary>
        public const int ModifierMask = unchecked((int)0xF0000000);

        /// <summary>Mask covering the base key, with all modifiers removed.</summary>
        public const int BaseMask = 0x0FFFFFFF;

        /// <summary>Flag marking an X11 keysym that directly encodes a Unicode code point.</summary>
        private const int UnicodeKeysymFlag = 0x01000000;

        #region DECOMPOSITION

        /// <summary>Gets the base key with all modifiers removed (iup_XkeyBase).</summary>
        public static Key BaseKey(this Key key) => (Key)((int)key & BaseMask);

        /// <summary>Gets only the modifier bits, or Key.None if there are none.</summary>
        public static Key Modifiers(this Key key) => (Key)((int)key & ModifierMask);

        /// <summary>True if the Shift modifier is set (iup_isShiftXkey).</summary>
        public static bool HasShift(this Key key) => ((int)key & ShiftBit) != 0;

        /// <summary>True if the Control modifier is set (iup_isCtrlXkey).</summary>
        public static bool HasCtrl(this Key key) => ((int)key & CtrlBit) != 0;

        /// <summary>True if the Alt modifier is set (iup_isAltXkey).</summary>
        public static bool HasAlt(this Key key) => ((int)key & AltBit) != 0;

        /// <summary>True if the Sys (Windows or Apple) modifier is set (iup_isSysXkey).</summary>
        public static bool HasSys(this Key key) => ((int)key & SysBit) != 0;

        /// <summary>True if any modifier is set.</summary>
        public static bool HasModifiers(this Key key) => ((int)key & ModifierMask) != 0;

        /// <summary>
        /// Adds one or more modifiers to a base key. Equivalent to the | operator;
        /// provided for readability when the modifier is chosen at run time. Any
        /// non-modifier bits in <paramref name="modifiers"/> are ignored.
        /// </summary>
        public static Key With(this Key baseKey, Key modifiers) =>
            (Key)((int)baseKey | ((int)modifiers & ModifierMask));

        #endregion

        #region CLASSIFICATION

        /// <summary>
        /// True if the base key is an IUP extended code rather than a character
        /// (iup_isXkey: base is 128 or above). Note that Latin-1 letters are extended
        /// by this definition but are still characters; use <see cref="IsChar"/> to
        /// test for that.
        /// </summary>
        public static bool IsExtended(this Key key) => (int)key.BaseKey() >= 128;

        /// <summary>
        /// Faithful translation of the iup_isprint macro: true only for a printable
        /// ASCII code with no modifiers applied. Most callers want
        /// <see cref="IsChar"/> instead, which also accepts modified keys and
        /// characters outside ASCII.
        /// </summary>
        public static bool IsPrintable(this Key key) => (int)key > 31 && (int)key < 127;

        /// <summary>
        /// True if the base key maps to a character: printable ASCII, the Latin-1
        /// range, or an X11 Unicode keysym. Modifiers are ignored, so Ctrl+A counts
        /// as a character.
        /// </summary>
        public static bool IsChar(this Key key) => key.ToCodePoint() >= 0;

        #endregion

        #region CHARACTER CONVERSION

        public static int ToCodePoint(this Key key, bool applyShift = true)
        {
            int b = (int)key.BaseKey();
            int cp = -1;

            if (b > 31 && b < 127) cp = b;                       // ASCII
            else if (b >= 0xA0 && b <= 0xFF) cp = b;             // Latin-1
            else if ((b & UnicodeKeysymFlag) != 0)               // X11 Unicode keysym
            {
                int u = b & 0x00FFFFFF;
                if (IsValidCodePoint(u)) cp = u;
            }

            if (cp < 0) return -1;

            // IUP resolves Shift into the character for ASCII, but for other characters it
            // reports the unshifted key with the Shift flag set. Normalise so both agree.
            if (applyShift && key.HasShift() && cp <= 0xFFFF)
            {
                char upper = char.ToUpperInvariant((char)cp);
                if (upper != (char)cp) cp = upper;
            }

            return cp;
        }

        /// <summary>
        /// Gets the character or characters for a character key, or an empty string if
        /// the key does not map to a character. Returns a surrogate pair for code
        /// points above U+FFFF, so prefer this over <see cref="ToChar"/>.
        /// </summary>
        public static string ToText(this Key key)
        {
            int cp = key.ToCodePoint();
            if (cp < 0)
                return string.Empty;

            return char.ConvertFromUtf32(cp);
        }

        /// <summary>
        /// Gets the character for a character key, or '\0' if the key does not map to
        /// a character, or if its code point is above U+FFFF and therefore needs a
        /// surrogate pair. Deliberately returns '\0' rather than silently truncating.
        /// Use <see cref="ToText"/> to handle the full Unicode range.
        /// </summary>
        public static char ToChar(this Key key)
        {
            int cp = key.ToCodePoint();
            if (cp < 0 || cp > 0xFFFF)
                return '\0';

            return (char)cp;
        }

        private static bool IsValidCodePoint(int cp) =>
            cp >= 0 && cp <= 0x10FFFF && (cp < 0xD800 || cp > 0xDFFF);

        #endregion

        #region DISPLAY

        public static string Describe(this Key key)
        {
            Key baseKey = key.BaseKey();
            int cp = key.ToCodePoint();        // full key, so Shift is applied
            bool isChar = cp >= 0;

            // The base key after shift resolution: 0xE5 -> 0xC5 for Shift+å.
            Key effective = isChar && cp <= 0xFFFF ? (Key)cp : baseKey;
            bool shiftConsumed = isChar && key.HasShift() && effective != baseKey;

            var sb = new StringBuilder();
            if (key.HasCtrl()) sb.Append("Ctrl+");
            if (key.HasAlt()) sb.Append("Alt+");
            if (key.HasShift() && !shiftConsumed) sb.Append("Shift+");
            if (key.HasSys()) sb.Append("Sys+");

            if (Enum.IsDefined(typeof(Key), effective))
                sb.Append(effective.ToString());
            else if (isChar)
                sb.Append(char.ConvertFromUtf32(cp));
            else
                sb.Append("0x").Append(((int)baseKey).ToString("X"));

            return sb.ToString();
        }
        #endregion
    }
}