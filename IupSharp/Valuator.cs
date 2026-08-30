using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Where the tick marks sit relative to the valuator trail.
    /// [Windows only]
    /// </summary>
    public enum TicksPosition
    {
        /// <summary>
        /// Above a horizontal valuator, or left of a vertical one. This is the default,
        /// and the only position Motif supports.
        /// </summary>
        Normal,

        /// <summary>Below a horizontal valuator, or right of a vertical one.</summary>
        Reverse,

        /// <summary>On both sides.</summary>
        Both
    }

    /// <summary>
    /// Creates a valuator, which selects a value from a limited interval. Native
    /// systems call it a scale or a trackbar. (since 3.0)
    /// </summary>
    /// <remarks>
    /// <para>Orientation, CanFocus and TicksPosition are creation-only, so they are
    /// constructor arguments rather than settable properties.</para>
    ///
    /// <para>A vertical valuator runs bottom to top and a horizontal one left to
    /// right, which is why Inverted defaults differently for each. Visually the arrow
    /// keys always move in the same direction; semantically they follow Inverted.</para>
    ///
    /// <para>Step and PageStep are fractions of the range rather than absolute
    /// increments, so both must lie strictly between 0 and 1. StepSize and
    /// PageStepSize give the resulting absolute amounts.</para>
    /// </remarks>
    public class Valuator : Control
    {
        /// <summary>
        /// Creates a new valuator.
        /// </summary>
        /// <param name="orientation">
        /// The direction. Horizontal runs left to right, Vertical runs bottom to top.
        /// Creation only. Default: Horizontal.
        /// </param>
        /// <param name="ticks">
        /// The number of tick marks along the trail, or 0 for none. The minimum
        /// non-zero value is 2. Whether ticks are present cannot be changed after the
        /// element is mapped, and GTK does not support them at all.
        /// [Windows and Motif only]
        /// </param>
        /// <param name="ticksPosition">
        /// Where the ticks sit. Creation only, and ignored in Motif, where the position
        /// is always Normal.
        /// [Windows only]
        /// </param>
        /// <param name="canFocus">
        /// False to exclude the valuator from focus traversal. Creation only. In
        /// Windows it still takes the focus when clicked.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">ticks is 1, or negative.</exception>
        public Valuator(Orientation orientation = Orientation.Horizontal,
                        int ticks = 0,
                        TicksPosition ticksPosition = TicksPosition.Normal,
                        bool canFocus = true)
            : base(NativeIup.Val(orientation == Orientation.Vertical ? "VERTICAL" : "HORIZONTAL"))
        {
            if (ticks == 1 || ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(ticks),
                    "Ticks must be 0 for none, or 2 or more.");

            // Creation-only attributes must be set before the element is mapped.
            if (ticks > 0)
            {
                SetAttribute("SHOWTICKS", ticks.ToString(CultureInfo.InvariantCulture));

                if (ticksPosition != TicksPosition.Normal)
                    SetAttribute("TICKSPOS",
                        ticksPosition == TicksPosition.Both ? "BOTH" : "REVERSE");
            }

            if (!canFocus)
                SetAttribute("CANFOCUS", "NO");
        }

        /// <summary>
        /// Creates a new valuator with the given range and value.
        /// </summary>
        public Valuator(double min, double max, double value = 0.0,
                        Orientation orientation = Orientation.Horizontal)
            : this(orientation)
        {
            SetRange(min, max, value);
        }

        #region VALUE

        /// <summary>
        /// Gets or sets the valuator position, a number between Min and Max.
        /// Default: 0. Assigning this is what refreshes the display.
        /// (non inheritable)
        /// </summary>
        public double Value
        {
            get => GetDouble("VALUE", 0.0);
            set => SetDouble("VALUE", value);
        }

        /// <summary>
        /// Gets or sets the minimum value. Default: 0.
        /// </summary>
        /// <remarks>
        /// Changing this does not redraw the control. Assign Value afterwards, or use
        /// SetRange, which does it for you.
        /// </remarks>
        public double Min
        {
            get => GetDouble("MIN", 0.0);
            set => SetDouble("MIN", value);
        }

        /// <summary>
        /// Gets or sets the maximum value. Default: 1.
        /// </summary>
        /// <remarks>
        /// Changing this does not redraw the control. Assign Value afterwards, or use
        /// SetRange, which does it for you.
        /// </remarks>
        public double Max
        {
            get => GetDouble("MAX", 1.0);
            set => SetDouble("MAX", value);
        }

        /// <summary>
        /// Sets the range and, optionally, the value, then refreshes the display.
        /// Prefer this over assigning Min and Max separately, since those alone leave
        /// the control showing its previous position.
        /// </summary>
        /// <exception cref="ArgumentException">min is not less than max.</exception>
        public void SetRange(double min, double max, double? value = null)
        {
            if (min >= max)
                throw new ArgumentException("min must be less than max.", nameof(min));

            double v = value ?? Value;

            Min = min;
            Max = max;
            Value = v;   // assigning VALUE is what refreshes the control
        }

        /// <summary>
        /// Gets or sets the position as a fraction between 0 and 1 of the range from
        /// Min to Max. Values outside that range are clamped.
        /// </summary>
        public double Fraction
        {
            get
            {
                double min = Min, max = Max;
                if (max <= min)
                    return 0.0;

                double f = (Value - min) / (max - min);
                return f < 0.0 ? 0.0 : (f > 1.0 ? 1.0 : f);
            }
            set
            {
                double f = value < 0.0 ? 0.0 : (value > 1.0 ? 1.0 : value);
                double min = Min, max = Max;
                Value = min + f * (max - min);
            }
        }

        #endregion

        #region INCREMENTS

        /// <summary>
        /// Gets or sets the increment for the arrow keys and the mouse wheel, as a
        /// fraction of the range. The actual increment is Step * (Max - Min).
        /// Default: 0.01. Must lie strictly between 0 and 1.
        /// </summary>
        public double Step
        {
            get => GetDouble("STEP", 0.01);
            set
            {
                if (value <= 0.0 || value >= 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "Step is a fraction of the range, so it must be between 0 and 1 exclusive.");

                SetDouble("STEP", value);
            }
        }

        /// <summary>
        /// Gets or sets the increment for the PgUp and PgDn keys, as a fraction of the
        /// range. The actual increment is PageStep * (Max - Min). Default: 0.1. Must
        /// lie strictly between 0 and 1.
        /// </summary>
        public double PageStep
        {
            get => GetDouble("PAGESTEP", 0.1);
            set
            {
                if (value <= 0.0 || value >= 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "PageStep is a fraction of the range, so it must be between 0 and 1 exclusive.");

                SetDouble("PAGESTEP", value);
            }
        }

        /// <summary>
        /// Gets or sets the arrow key increment as an absolute amount, converting to
        /// and from the Step fraction. Reading it returns Step * (Max - Min).
        /// </summary>
        public double StepSize
        {
            get => Step * (Max - Min);
            set
            {
                double range = Max - Min;
                if (range <= 0.0)
                    throw new InvalidOperationException("Set a valid range before setting StepSize.");

                Step = value / range;
            }
        }

        /// <summary>
        /// Gets or sets the page increment as an absolute amount, converting to and
        /// from the PageStep fraction. Reading it returns PageStep * (Max - Min).
        /// </summary>
        public double PageStepSize
        {
            get => PageStep * (Max - Min);
            set
            {
                double range = Max - Min;
                if (range <= 0.0)
                    throw new InvalidOperationException("Set a valid range before setting PageStepSize.");

                PageStep = value / range;
            }
        }

        #endregion

        #region APPEARANCE

        /// <summary>
        /// Gets the direction. Creation only - pass it to the constructor.
        /// (non inheritable)
        /// </summary>
        public Orientation Orientation =>
            GetAttribute("ORIENTATION") == "VERTICAL" ? Orientation.Vertical : Orientation.Horizontal;

        /// <summary>
        /// Gets or sets whether the minimum and maximum are swapped on screen. When
        /// true the maximum is at the top or left; when false it is at the bottom or
        /// right. Defaults to true for a vertical valuator and false for a horizontal
        /// one, matching the natural reading direction of each.
        /// </summary>
        public virtual bool Inverted
        {
            get => GetAttribute("INVERTED") == "YES";
            set => SetAttribute("INVERTED", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets the number of tick marks along the trail, or 0 when there are none.
        /// Whether ticks exist at all is fixed at creation; GTK never shows them.
        /// [Windows and Motif only]
        /// </summary>
        public int Ticks => GetInt("SHOWTICKS", 0);

        /// <summary>
        /// Gets where the tick marks sit. Creation only, and always Normal in Motif.
        /// [Windows only]
        /// </summary>
        public TicksPosition TicksPosition
        {
            get
            {
                string v = GetAttribute("TICKSPOS");
                return v switch
                {
                    "BOTH" => TicksPosition.Both,
                    "REVERSE" => TicksPosition.Reverse,
                    _ => TicksPosition.Normal
                };
            }
        }

        /// <summary>
        /// Gets whether focus traversal is enabled. Creation only - pass it to the
        /// constructor. In Windows the control still takes the focus when clicked.
        /// (non inheritable)
        /// </summary>
        public bool CanFocus => GetAttribute("CANFOCUS") != "NO";

        /// <summary>
        /// Gets or sets whether the focus callback is forwarded to the next native
        /// parent with FocusCB defined. Default: false.
        /// (non inheritable) (since 3.23)
        /// </summary>
        public virtual bool PropagateFocus
        {
            get => GetAttribute("PROPAGATEFOCUS") == "YES";
            set => SetAttribute("PROPAGATEFOCUS", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the background colour. Transparent on every system except
        /// Motif, where it uses the background of the native parent.
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; }

        /// <summary>
        /// Removes the initial raster size so the layout may use smaller values. The
        /// natural size is otherwise 100 pixels along the major axis, plus room for the
        /// handler and any ticks on the minor axis.
        /// </summary>
        public void ClearDefaultSize() => SetAttribute("RASTERSIZE", null);

        #endregion

        #region HELPERS

        private double GetDouble(string name, double fallback)
        {
            string v = GetAttribute(name);
            return double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)
                ? d
                : fallback;
        }

        private void SetDouble(string name, double value) =>
            SetAttribute(name, value.ToString("R", CultureInfo.InvariantCulture));

        private int GetInt(string name, int fallback)
        {
            string v = GetAttribute(name);
            return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)
                ? i
                : fallback;
        }

        #endregion

        #region CALLBACKS

        private Callback _valueChangedCB;
        private IFn _valueChangedCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called after the value was interactively changed by
        /// the user. This is the callback to use; the older MOUSEMOVE_CB,
        /// BUTTON_PRESS_CB and BUTTON_RELEASE_CB are only invoked when this one is not
        /// defined.
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
                    $"[IupSharp] unhandled exception in Valuator ValueChangedCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}