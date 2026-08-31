using System;
using System.Drawing;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// The layout of a <see cref="Dial"/>. Unlike most controls a dial has three
    /// orientations, so it does not use the shared Orientation enum.
    /// </summary>
    public enum DialOrientation
    {
        /// <summary>
        /// A horizontal band. Increments when dragged right, decrements when dragged
        /// left. This is the default.
        /// </summary>
        Horizontal,

        /// <summary>
        /// A vertical band. Increments when dragged up, decrements when dragged down.
        /// </summary>
        Vertical,

        /// <summary>
        /// A circular knob. Increments counter clockwise, decrements clockwise. This
        /// is the only orientation that measures absolute angles.
        /// </summary>
        Circular
    }

    /// <summary>
    /// The unit a <see cref="Dial"/> uses when reporting angles to its callbacks.
    /// </summary>
    /// <remarks>
    /// This affects the callbacks only. The Value property is always in radians
    /// regardless.
    /// </remarks>
    public enum AngleUnit
    {
        /// <summary>Angles are reported in radians. This is the default.</summary>
        Radians,

        /// <summary>Angles are reported in degrees.</summary>
        Degrees
    }

    /// <summary>
    /// Creates a dial for regulating an angular value.
    /// </summary>
    /// <remarks>
    /// <para>Migrated from the IupControls library into the main library in IUP 3.24,
    /// where it was rewritten to use IupDraw and no longer depends on CD - so it needs
    /// no extra native library.</para>
    ///
    /// <para><b>Horizontal and vertical dials measure relative angles</b>: the value
    /// resets to zero each time the user starts a new interaction, so they report how
    /// far the dial has been turned rather than where it points. Only a circular dial
    /// measures an absolute angle, with the origin at three o'clock.</para>
    ///
    /// <para>Although IupDial inherits from IupCanvas natively, this wrapper derives
    /// from Control. The canvas drawing surface is used internally to paint the dial,
    /// so exposing it would offer members that either do nothing or break the
    /// control.</para>
    ///
    /// <para>Orientation is creation-only, so it is a constructor argument.</para>
    ///
    /// <example>
    /// <code>
    /// var dial = new Dial(DialOrientation.Circular) { Unit = AngleUnit.Degrees };
    /// dial.ValueChangedCB = d =&gt;
    ///     Console.WriteLine($"{((Dial)d.Sender).ValueDegrees:F1} degrees");
    /// </code>
    /// </example>
    /// </remarks>
    public class Dial : Control
    {
        /// <summary>
        /// Creates a new dial.
        /// </summary>
        /// <param name="orientation">
        /// The dial layout. Creation only. Default: Horizontal.
        /// </param>
        public Dial(DialOrientation orientation = DialOrientation.Horizontal)
            : base(NativeIup.Dial(OrientationToString(orientation)))
        {
        }

        #region VALUE

        /// <summary>
        /// Gets or sets the dial's angular value, <b>always in radians</b> whatever
        /// Unit is set to.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// For a horizontal or vertical dial this is a relative angle and is reset to
        /// zero when the user starts a new interaction. For a circular dial it is an
        /// absolute angle measured from three o'clock.
        /// </remarks>
        public double Value
        {
            get => GetDouble("VALUE", 0.0);
            set => SetDouble("VALUE", value);
        }

        /// <summary>
        /// Gets or sets the dial's angular value in degrees, converting to and from
        /// the radians that <see cref="Value"/> uses.
        /// </summary>
        public double ValueDegrees
        {
            get => Value * (180.0 / Math.PI);
            set => Value = value * (Math.PI / 180.0);
        }

        /// <summary>
        /// Gets or sets the unit used when angles are reported to the callbacks.
        /// Default: Radians.
        /// </summary>
        /// <remarks>
        /// This has no effect on <see cref="Value"/>, which is always in radians. It
        /// only changes the number passed to MouseMoveCB, ButtonPressCB and
        /// ButtonReleaseCB.
        /// </remarks>
        public virtual AngleUnit Unit
        {
            get => GetAttribute("UNIT") == "DEGREES" ? AngleUnit.Degrees : AngleUnit.Radians;
            set => SetAttribute("UNIT", value == AngleUnit.Degrees ? "DEGREES" : "RADIANS");
        }

        #endregion

        #region APPEARANCE

        /// <summary>
        /// Gets the dial layout. Creation only - pass it to the constructor.
        /// (non inheritable)
        /// </summary>
        public DialOrientation Orientation => ParseOrientation(GetAttribute("ORIENTATION"));

        /// <summary>
        /// Gets or sets the number of lines per pixel drawn in the dial's handle,
        /// which controls how finely the ticks are spaced. Default: 0.2.
        /// </summary>
        public double Density
        {
            get => GetDouble("DENSITY", 0.2);
            set => SetDouble("DENSITY", value);
        }

        /// <summary>
        /// Gets or sets whether a one pixel flat border is used instead of the default
        /// three pixel sunken border. Default: false.
        /// (since 3.24)
        /// </summary>
        public virtual bool Flat
        {
            get => GetAttribute("FLAT") == "YES";
            set => SetAttribute("FLAT", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets the border colour used when Flat is true.
        /// Default: (160,160,160).
        /// (since 3.24)
        /// </summary>
        public Color FlatColor
        {
            get => Utils.ParseColor(GetAttribute("FLATCOLOR"));
            set => SetAttribute("FLATCOLOR", Utils.FormatColor(value));
        }

        /// <summary>
        /// Gets or sets the foreground colour. Default: (64,64,64). It is only visible
        /// in a circular dial, and only since 3.24.
        /// </summary>
        public override Color FgColor { get => base.FgColor; set => base.FgColor = value; }

        /// <summary>
        /// Removes the initial size so the layout may use smaller values. The default
        /// is 16x80, 80x16 or 40x35 depending on the orientation, and otherwise acts
        /// as a minimum.
        /// </summary>
        public void ClearDefaultSize() => SetAttribute("SIZE", null);

        #endregion

        #region HELPERS

        private static string OrientationToString(DialOrientation o) => o switch
        {
            DialOrientation.Vertical => "VERTICAL",
            DialOrientation.Circular => "CIRCULAR",
            _ => "HORIZONTAL"
        };

        private static DialOrientation ParseOrientation(string v) => v switch
        {
            "VERTICAL" => DialOrientation.Vertical,
            "CIRCULAR" => DialOrientation.Circular,
            _ => DialOrientation.Horizontal
        };

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

        #region CALLBACKS

        // NOTE ON CALLBACK PRECEDENCE
        //
        // If ValueChangedCB is set, IUP calls it INSTEAD OF MouseMoveCB,
        // ButtonPressCB and ButtonReleaseCB - those three are only invoked when
        // ValueChangedCB is not defined. So use either ValueChangedCB on its own, or
        // the other three on their own; mixing them means the three never fire.

        private Callback _valueChangedCB;
        private IFn _valueChangedCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called after the value was interactively changed by
        /// the user. This is the callback to use unless you need to distinguish press
        /// from drag from release.
        /// (since 3.0)
        /// </summary>
        /// <remarks>
        /// <b>Setting this suppresses ButtonPressCB, MouseMoveCB and
        /// ButtonReleaseCB</b>, which IUP only calls when this one is undefined.
        /// </remarks>
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
                    $"[IupSharp] unhandled exception in Dial ValueChangedCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private AngleCallback _buttonPressCB;
        private IFnd _buttonPressCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when the user presses the left mouse button
        /// over the dial. The angle is always zero except on a circular dial.
        /// </summary>
        /// <remarks>
        /// Not called at all when ValueChangedCB is set. Pressing and releasing an
        /// arrow key calls this and then ButtonReleaseCB, in that order.
        /// </remarks>
        public AngleCallback ButtonPressCB
        {
            get => _buttonPressCB;
            set
            {
                _buttonPressCB = value;
                _buttonPressCBInternal = ButtonPressCBInternal;
                SetCallback("BUTTON_PRESS_CB", Utils.CastCallback<Icallback>(_buttonPressCBInternal));
            }
        }
        private int ButtonPressCBInternal(nint ih, double angle)
        {
            try
            {
                var cb = new AngleData(this, angle);
                _buttonPressCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Dial ButtonPressCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private AngleCallback _mouseMoveCB;
        private IFnd _mouseMoveCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called each time the user moves the dial with the
        /// mouse button held down. The angle is how far the dial has turned since it
        /// was initialised.
        /// </summary>
        /// <remarks>
        /// Not called at all when ValueChangedCB is set. Holding an arrow key down
        /// also calls this on every repeat, and rotating the wheel calls only this.
        /// Each step is PI/10 (18 degrees), reduced to PI/100 (1.8 degrees) while
        /// Shift is held.
        /// </remarks>
        public AngleCallback MouseMoveCB
        {
            get => _mouseMoveCB;
            set
            {
                _mouseMoveCB = value;
                _mouseMoveCBInternal = MouseMoveCBInternal;
                SetCallback("MOUSEMOVE_CB", Utils.CastCallback<Icallback>(_mouseMoveCBInternal));
            }
        }
        private int MouseMoveCBInternal(nint ih, double angle)
        {
            try
            {
                var cb = new AngleData(this, angle);
                _mouseMoveCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Dial MouseMoveCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private AngleCallback _buttonReleaseCB;
        private IFnd _buttonReleaseCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called when the user releases the left mouse button
        /// after pressing it over the dial. Useful for committing a value once the
        /// interaction is finished.
        /// </summary>
        /// <remarks>Not called at all when ValueChangedCB is set.</remarks>
        public AngleCallback ButtonReleaseCB
        {
            get => _buttonReleaseCB;
            set
            {
                _buttonReleaseCB = value;
                _buttonReleaseCBInternal = ButtonReleaseCBInternal;
                SetCallback("BUTTON_RELEASE_CB", Utils.CastCallback<Icallback>(_buttonReleaseCBInternal));
            }
        }
        private int ButtonReleaseCBInternal(nint ih, double angle)
        {
            try
            {
                var cb = new AngleData(this, angle);
                _buttonReleaseCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Dial ButtonReleaseCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}