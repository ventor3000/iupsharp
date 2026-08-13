using System;
using System.Globalization;

namespace IupSharp
{
    public class Calendar : Control
    {
        public Calendar() : base(IupNative.IupCalendar())
        {
        }

        public DateTime Value
        {
            get
            {
                string value = GetAttribute("VALUE");
                if (!DateTime.TryParseExact(
                    value,
                    "yyyy/M/d",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime date))
                    return DateTime.Now;

                return date;
            }
            set
            {
                string formatted = value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                SetAttribute("VALUE", formatted);
            }
        }

        public bool WeekNumbers
        {
            get
            {
                return GetAttribute("WEEKNUMBERS") == "YES" ? true : false;
            }
            set
            {
                SetAttribute("WEEKNUMBERS", value ? "YES" : "NO");
            }
        }


        private Callback _valueChangedCB; // users callback function
        private IFn _valueChangedCBInternal; // need reference to keep alive in GC
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
