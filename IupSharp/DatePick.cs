using System;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// The order in which the day, month and year appear in a DatePick.
    /// </summary>
    public enum DateOrder
    {
        /// <summary>Day, month, year. This is IUP's default.</summary>
        DayMonthYear,

        /// <summary>Month, day, year, as commonly written in the United States.</summary>
        MonthDayYear,

        /// <summary>Year, month, day, as in ISO 8601 and in IUP's own VALUE format.</summary>
        YearMonthDay,

        /// <summary>Year, day, month.</summary>
        YearDayMonth,

        /// <summary>Day, year, month.</summary>
        DayYearMonth,

        /// <summary>Month, year, day.</summary>
        MonthYearDay
    }

    /// <summary>
    /// Creates a date editing element, which can drop down a calendar for choosing a
    /// date. (since 3.17)
    /// </summary>
    /// <remarks>
    /// <para>Native on Windows, a custom element built from IUP elements on GTK. On
    /// Motif it exists but cannot display the calendar.</para>
    ///
    /// <para><b>Order, Separator, ZeroPreceded and MonthShortNames must be set before
    /// the element is mapped on Windows</b>, and the last three must be set before
    /// Order, since Order rewrites the underlying FORMAT attribute from them. The
    /// constructor takes all four so the ordering is handled for you; assigning them
    /// afterwards is allowed but may not take effect.</para>
    ///
    /// <para>The display order does not affect Value, which is always read and
    /// written as a DateTime regardless of how the control presents it.</para>
    /// </remarks>
    public class DatePick : Control
    {
        /// <summary>
        /// IUP's date format for the VALUE and TODAY attributes ("year/month/day",
        /// unpadded, e.g. "2026/8/19").
        /// </summary>
        private const string DateFormat = "yyyy/M/d";

        /// <summary>
        /// Creates a new date picker showing today's date.
        /// </summary>
        /// <param name="order">
        /// The order of day, month and year in the display. Default: DayMonthYear.
        /// </param>
        /// <param name="separator">
        /// The character between the parts. Default: "/". Must be set before Order on
        /// Windows, which the constructor does.
        /// </param>
        /// <param name="zeroPreceded">
        /// True to pad single digit days and months with a leading zero. Must be set
        /// before Order on Windows.
        /// </param>
        /// <param name="monthShortNames">
        /// True to show a three letter month abbreviation instead of a number, in the
        /// language of the system. Must be set before Order on Windows.
        /// [Windows only]
        /// </param>
        /// <param name="weekNumbers">
        /// True to show the week number down the side of the dropdown calendar.
        /// </param>
        public DatePick(DateOrder order = DateOrder.DayMonthYear,
                        string separator = null,
                        bool zeroPreceded = false,
                        bool monthShortNames = false,
                        bool weekNumbers = false)
            : base(NativeIup.IupDatePick())
        {
            // SEPARATOR, ZEROPRECED and MONTHSHORTNAMES must all be set before ORDER,
            // because ORDER is what rebuilds the Windows FORMAT string from them.
            if (separator != null)
                SetAttribute("SEPARATOR", separator);

            if (zeroPreceded)
                SetAttribute("ZEROPRECED", "YES");

            if (monthShortNames)
                SetAttribute("MONTHSHORTNAMES", "YES");

            if (order != DateOrder.DayMonthYear)
                SetAttribute("ORDER", OrderToString(order));

            if (weekNumbers)
                SetAttribute("CALENDARWEEKNUMBERS", "YES");
        }

        /// <summary>
        /// Creates a new date picker showing the given date.
        /// </summary>
        public DatePick(DateTime value) : this()
        {
            Value = value;
        }

        #region VALUE

        /// <summary>
        /// Gets or sets the selected date. Defaults to today. The time part of an
        /// assigned DateTime is ignored, since IUP stores only the date.
        /// </summary>
        public DateTime Value
        {
            get => ParseIupDate(GetAttribute("VALUE"), nameof(Value)) ?? DateTime.Today;
            set => SetAttribute("VALUE", value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Sets the value to today's date, using IUP's own idea of today rather than
        /// the machine clock read from managed code.
        /// </summary>
        public void SelectToday() => SetAttribute("VALUE", "TODAY");

        /// <summary>
        /// Gets today's date as IUP reports it.
        /// (read only)
        /// </summary>
        public DateTime Today =>
            ParseIupDate(GetAttribute("TODAY"), nameof(Today)) ?? DateTime.Today;

        #endregion

        #region APPEARANCE

        /// <summary>
        /// Gets or sets the order of day, month and year in the display. This does not
        /// affect Value, which is always a DateTime.
        /// </summary>
        /// <remarks>
        /// On Windows this rewrites the FORMAT attribute from the current Separator,
        /// ZeroPreceded and MonthShortNames, so those must already be set. Prefer the
        /// constructor, which orders them correctly.
        /// </remarks>
        public virtual DateOrder Order
        {
            get => ParseOrder(GetAttribute("ORDER"));
            set => SetAttribute("ORDER", OrderToString(value));
        }

        /// <summary>
        /// Gets or sets the character shown between the day, month and year.
        /// Default: "/". Must be set before Order on Windows.
        /// </summary>
        public virtual string Separator
        {
            get => GetAttribute("SEPARATOR") ?? "/";
            set => SetAttribute("SEPARATOR", value);
        }

        /// <summary>
        /// Gets or sets whether single digit days and months are padded with a leading
        /// zero. Default: false. Must be set before Order on Windows.
        /// </summary>
        public virtual bool ZeroPreceded
        {
            get => GetAttribute("ZEROPRECED") == "YES";
            set => SetAttribute("ZEROPRECED", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the month is shown as a three letter abbreviation in
        /// the language of the system, rather than a number. Default: false. Must be
        /// set before Order.
        /// [Windows only]
        /// </summary>
        public virtual bool MonthShortNames
        {
            get => GetAttribute("MONTHSHORTNAMES") == "YES";
            set => SetAttribute("MONTHSHORTNAMES", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the dropdown calendar shows the week number down the
        /// side. Default: false.
        /// </summary>
        public virtual bool CalendarWeekNumbers
        {
            get => GetAttribute("CALENDARWEEKNUMBERS") == "YES";
            set => SetAttribute("CALENDARWEEKNUMBERS", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the raw Windows date format string, for cases the Order,
        /// Separator, ZeroPreceded and MonthShortNames properties cannot express.
        /// Literal text must be enclosed in single quotes. Default: "d'/'M'/'yyyy".
        /// </summary>
        /// <remarks>
        /// Setting Order overwrites this. The recognised elements are d, dd, ddd,
        /// dddd, M, MM, MMM, MMMM, yy and yyyy.
        /// [Windows only]
        /// </remarks>
        public virtual string Format
        {
            get => GetAttribute("FORMAT");
            set => SetAttribute("FORMAT", value);
        }

        /// <summary>
        /// Opens or closes the dropdown calendar. Ignored before the element is
        /// mapped, and on Windows only closing works.
        /// (write only) (since 3.28)
        /// </summary>
        public bool ShowDropDown
        {
            set => SetAttribute("SHOWDROPDOWN", value ? "YES" : "NO");
        }

        #endregion

        #region HELPERS

        /// <summary>
        /// Parses a date string returned by IUP in the "year/month/day" format. Logs
        /// and returns null if the value is missing or not in the expected format,
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
                    $"[IupSharp] DatePick.{attributeName}: could not parse IUP date value \"{value}\".");
                return null;
            }

            return date;
        }

        private static string OrderToString(DateOrder order) => order switch
        {
            DateOrder.MonthDayYear => "MDY",
            DateOrder.YearMonthDay => "YMD",
            DateOrder.YearDayMonth => "YDM",
            DateOrder.DayYearMonth => "DYM",
            DateOrder.MonthYearDay => "MYD",
            _ => "DMY"
        };

        private static DateOrder ParseOrder(string value) => value switch
        {
            "MDY" => DateOrder.MonthDayYear,
            "YMD" => DateOrder.YearMonthDay,
            "YDM" => DateOrder.YearDayMonth,
            "DYM" => DateOrder.DayYearMonth,
            "MYD" => DateOrder.MonthYearDay,
            _ => DateOrder.DayMonthYear
        };

        #endregion

        #region CALLBACKS

        private Callback _valueChangedCB;
        private IFn _valueChangedCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called after the date was interactively changed by
        /// the user. On Windows the date does not change while the user browses other
        /// months; the callback fires only once a day is actually chosen.
        /// </summary>
        public Callback ValueChangedCB
        {
            get => _valueChangedCB;
            set
            {
                _valueChangedCB = value;
                _valueChangedCBInternal = ValueChangedCBInternal;
                SetCallback("VALUECHANGED_CB", Utils.CastCallback<Icallback>(_valueChangedCBInternal));
            }
        }
        private int ValueChangedCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _valueChangedCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in DatePick ValueChangedCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}