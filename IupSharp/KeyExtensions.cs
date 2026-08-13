using System;

namespace IupSharp
{
    /// <summary>Helpers for decomposing and validating <see cref="Key"/> values.</summary>
    public static class KeyExtensions
    {
        private const int ModifierSpan = 256;

        /// <summary>Gets the base key with any modifier removed.</summary>
        public static Key BaseKey(this Key key) => (Key)((int)key % ModifierSpan);

        /// <summary>Gets the modifier, or Key.None if there is none.</summary>
        public static Key Modifier(this Key key)
        {
            int m = ((int)key / ModifierSpan) * ModifierSpan;
            return (Key)m;
        }

        /// <summary>True if the key can be used directly as a printable character.</summary>
        public static bool IsPrintable(this Key key) => (int)key > 31 && (int)key < 127;

        /// <summary>True if the key is an IUP extended code rather than a character.</summary>
        public static bool IsExtended(this Key key)
        {
            int b = (int)key % ModifierSpan;
            return b > 128;
        }

        /// <summary>
        /// Gets the character for a printable key, or '\0' if the key is not printable.
        /// </summary>
        public static char ToChar(this Key key) => key.IsPrintable() ? (char)key : '\0';

        /// <summary>
        /// True if the value is a code IUP can actually represent. Returns false for
        /// combinations of two or more modifiers, which silently alias a different key.
        /// </summary>
        public static bool IsValid(this Key key) => (int)key >= 0 && (int)key < 1280;

        /// <summary>
        /// Combines a base key with a single modifier, throwing if the base key already
        /// carries one. Prefer this over | when the modifier is chosen at run time.
        /// </summary>
        /// <exception cref="ArgumentException">The base key already has a modifier, or the modifier is not one of Shift, Ctrl, Alt or Sys.</exception>
        public static Key With(this Key baseKey, Key modifier)
        {
            if ((int)baseKey >= ModifierSpan)
                throw new ArgumentException(
                    $"{baseKey} already carries a modifier; IUP can encode only one.", nameof(baseKey));

            if (modifier != Key.Shift && modifier != Key.Ctrl &&
                modifier != Key.Alt && modifier != Key.Sys && modifier != Key.None)
                throw new ArgumentException(
                    $"{modifier} is not a modifier.", nameof(modifier));

            return (Key)((int)baseKey + (int)modifier);
        }
    }
}
