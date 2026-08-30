using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates a progress bar, showing a value that can be updated to indicate
    /// progress. (since 3.0)
    /// </summary>
    /// <remarks>
    /// <para>Similar to IupGauge but built from native controls, and with no support
    /// for text inside the bar.</para>
    ///
    /// <para>Orientation is creation-only, so it is a constructor argument rather than
    /// a settable property.</para>
    ///
    /// <para>The control has no callbacks of its own - only the common MapCB, UnmapCB
    /// and DestroyCB, all inherited.</para>
    /// </remarks>
    public class ProgressBar : Control
    {
        /// <summary>
        /// Creates a new progress bar.
        /// </summary>
        /// <param name="orientation">
        /// The bar direction. Horizontal fills left to right, Vertical fills bottom to
        /// top. Creation only. Default: Horizontal.
        /// </param>
        /// <param name="dashed">
        /// True for a dashed rather than continuous bar. Creation only on Windows, and
        /// ignored there since Vista when using Visual Styles.
        /// [Windows and GTK only]
        /// </param>
        /// <param name="marquee">
        /// True to show an undefined state, animating continuously rather than showing
        /// a position. See the Marquee property for starting and stopping it later.
        /// </param>
        public ProgressBar(Orientation orientation = Orientation.Horizontal,
                           bool dashed = false, bool marquee = false)
            : base(IupNative.IupProgressBar())
        {
            // Creation-only attributes must be set before the element is mapped.
            if (orientation == Orientation.Vertical)
                SetAttribute("ORIENTATION", "VERTICAL");

            if (dashed)
                SetAttribute("DASHED", "YES");

            if (marquee)
                SetAttribute("MARQUEE", "YES");
        }

        /// <summary>
        /// Creates a new horizontal progress bar with the given range and value.
        /// </summary>
        public ProgressBar(double min, double max, double value = 0.0)
            : this()
        {
            SetRange(min, max, value);
        }

        #region VALUE

        /// <summary>
        /// Gets or sets the current position, which should lie between Min and Max.
        /// Assigning this is what refreshes the display.
        /// (non inheritable)
        /// </summary>
        public double Value
        {
            get => GetDouble("VALUE", 0.0);
            set => SetDouble("VALUE", value);
        }

        /// <summary>
        /// Gets or sets the minimum value. Default: 0.
        /// (non inheritable)
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
        /// (non inheritable)
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
        /// <param name="min">The new minimum.</param>
        /// <param name="max">The new maximum.</param>
        /// <param name="value">
        /// The new position. When omitted the current value is re-applied, which is
        /// what triggers the redraw.
        /// </param>
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

        #region APPEARANCE

        /// <summary>
        /// Gets the bar direction. Creation only - pass it to the constructor.
        /// </summary>
        public Orientation Orientation =>
            GetAttribute("ORIENTATION") == "VERTICAL" ? Orientation.Vertical : Orientation.Horizontal;

        /// <summary>
        /// Gets or sets whether the bar shows an undefined state, animating
        /// continuously instead of showing a position.
        /// </summary>
        /// <remarks>
        /// The mode itself is fixed at creation; afterwards this only starts and stops
        /// the animation. Setting it to true on a bar not created with marquee has no
        /// effect. In Windows it works only with Visual Styles enabled.
        /// </remarks>
        public virtual bool Marquee
        {
            get => GetAttribute("MARQUEE") == "YES";
            set => SetAttribute("MARQUEE", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether the bar uses a dashed rather than continuous pattern.
        /// Creation only on Windows, where it is also ignored since Vista when using
        /// Visual Styles.
        /// [Windows and GTK only]
        /// </summary>
        public virtual bool Dashed
        {
            get => GetAttribute("DASHED") == "YES";
            set => SetAttribute("DASHED", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the background colour. Default: the global attribute
        /// DlgBgColor.
        /// [Windows Classic and Motif only]
        /// </summary>
        public override Color BgColor { get => base.BgColor; set => base.BgColor = value; }

        /// <summary>
        /// Gets or sets the bar colour. Default: the global attribute DlgFgColor.
        /// [Windows Classic and Motif only]
        /// </summary>
        public override Color FgColor { get => base.FgColor; set => base.FgColor = value; }

        /// <summary>
        /// Removes the initial 200x30 raster size so the layout may use smaller
        /// values. The default size otherwise acts as a minimum.
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

        #endregion
    }
}