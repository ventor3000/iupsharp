using System;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates a month calendar interface element, where the user can select a date.
    /// </summary>
    /// <remarks>
    /// Available in GTK and Windows only. NOT available in Motif.
    /// </remarks>
    public class Calendar : Control
    {
        /// <summary>
        /// IUP's date format for the VALUE and TODAY attributes ("year/month/day",
        /// unpadded, e.g. "2026/8/19").
        /// </summary>
        private const string DateFormat = "yyyy/M/d";

        /// <summary>
        /// Creates a new Calendar.
        /// </summary>
        public Calendar() : base(IupNative.IupCalendar())
        {
        }

        /// <summary>
        /// Parses a date string returned by IUP in the "year/month/day" format.
        /// Logs and returns null if the value is missing or not in the expected format,
        /// rather than silently substituting a fallback date.
        /// </summary>
        private static DateTime? ParseIupDate(string value, string attributeName)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (!DateTime.TryParseExact(
                value,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] Calendar.{attributeName}: could not parse IUP date value \"{value}\".");
                return null;
            }

            return date;
        }

        /// <summary>
        /// Gets or sets the current date. Can be set to DateTime.Now to select today's
        /// date (equivalent to setting the native VALUE attribute to "TODAY").
        /// Default value is today's date.
        /// </summary>
        public DateTime Value
        {
            get => ParseIupDate(GetAttribute("VALUE"), nameof(Value)) ?? DateTime.Now;
            set
            {
                string formatted = value.ToString(DateFormat, CultureInfo.InvariantCulture);
                SetAttribute("VALUE", formatted);
            }
        }

        /// <summary>
        /// Gets today's date, as reported by IUP in the same format as VALUE.
        /// (read-only)
        /// </summary>
        public DateTime Today => ParseIupDate(GetAttribute("TODAY"), nameof(Today)) ?? DateTime.Now;

        /// <summary>
        /// Gets or sets whether the number of the week along the year is shown.
        /// Default: No.
        /// </summary>
        public bool WeekNumbers
        {
            get => GetAttribute("WEEKNUMBERS") == "YES";
            set => SetAttribute("WEEKNUMBERS", value ? "YES" : "NO");
        }


        private Callback _valueChangedCB; // users callback function
        private IFn _valueChangedCBInternal; // need reference to keep alive in GC

        /// <summary>
        /// Gets or sets the callback called after the value was interactively
        /// changed by the user.
        /// </summary>
        public Callback ValueChangedCB
        {
            get => _valueChangedCB;
            set
            {
                _valueChangedCB = value;
                _valueChangedCBInternal = ValueChangedInternal;
                SetCallback("VALUECHANGED_CB", Utils.CastCallback<Icallback>(_valueChangedCBInternal));
            }
        }

        private int ValueChangedInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _valueChangedCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ValueChangedCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }
    }
}