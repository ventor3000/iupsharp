using System;

namespace IupSharp
{
    /// <summary>
    /// Creates a pair of stacked arrow buttons for incrementing and decrementing a
    /// value.
    /// </summary>
    /// <remarks>
    /// <para>Unlike the Spin properties on a Text control, this does NOT increment
    /// anything by itself and is not placed inside the text area. It only reports
    /// clicks through SpinCB, leaving the application to decide what they mean - which
    /// is what makes it usable next to any element rather than only a text field.</para>
    ///
    /// <para>For a numeric entry field, prefer Text with its Spin property set: it
    /// handles the value, the range and the wrapping for you. Reach for this when the
    /// thing being incremented is not a Text, or when the increment logic is the
    /// application's own.</para>
    ///
    /// <para>Internally this is a VBox holding two Buttons, which is why it derives
    /// from VBoxHBox. Do not add children to it - use Spinbox to place a spin beside
    /// another control.</para>
    ///
    /// <example>
    /// <code>
    /// var spin = new Spin();
    /// spin.SpinCB = d =&gt; count += d.Increment;   // +1, -1, or a multiple
    /// </code>
    /// </example>
    /// </remarks>
    public class Spin : VBoxHBox
    {
        /// <summary>
        /// Creates a new pair of spin buttons.
        /// </summary>
        public Spin() : base(IupNative.IupSpin())
        {
        }

        #region CALLBACKS

        private SpinButtonCallback _spinCB;
        private IFni _spinCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called each time the user clicks one of the
        /// buttons. The callback data carries the increment: +1 for up and -1 for
        /// down, multiplied by 2 while Shift is held, 10 for Ctrl, and 100 for both.
        /// </summary>
        public SpinButtonCallback SpinCB
        {
            get => _spinCB;
            set
            {
                _spinCB = value;
                _spinCBInternal = SpinCBInternal;
                SetCallback("SPIN_CB", Utils.CastCallback<Icallback>(_spinCBInternal));
            }
        }
        private int SpinCBInternal(nint ih, int inc)
        {
            try
            {
                var cb = new SpinButtonData(this, inc);
                _spinCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Spin SpinCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }


    /// <summary>
    /// A horizontal container holding one control with a Spin beside it.
    /// </summary>
    /// <remarks>
    /// <para>The spin itself is created by IUP, so it has no wrapper object. Set
    /// SpinCB on the Spinbox: IUP dispatches SPIN_CB to this element.</para>
    ///
    /// <para>The child is not incremented automatically - see Spin for why. For a
    /// numeric field, Text with its Spin property set is usually the better
    /// choice.</para>
    ///
    /// <example>
    /// <code>
    /// var field = new Text();
    /// var box = new Spinbox(field);
    /// box.SpinCB = d =&gt;
    /// {
    ///     int v = int.TryParse(field.Value, out int n) ? n : 0;
    ///     field.Value = (v + d.Increment).ToString();
    /// };
    /// </code>
    /// </example>
    /// </remarks>
    public class Spinbox : VBoxHBox
    {
        private Control _child;

        /// <summary>
        /// Creates a new spinbox around the given control.
        /// </summary>
        /// <param name="child">
        /// The control to place beside the spin buttons. It can be null, in which case
        /// the box is created empty and a child can be added later with Append.
        /// </param>
        public Spinbox(Control child)
            : base(IupNative.IupSpinbox(child == null ? IntPtr.Zero : child.Handle))
        {
            _child = child;
        }

        /// <summary>
        /// Creates a new empty spinbox. A child can be added later with Append.
        /// </summary>
        public Spinbox() : this(null)
        {
        }

        /// <summary>
        /// Gets the control placed beside the spin buttons, or null if there is none.
        /// </summary>
        public Control Child => _child;

        /// <summary>
        /// Adds the control that sits beside the spin buttons. A spinbox holds one
        /// control besides its own spin; wrap several in a VBox or HBox if needed.
        /// </summary>
        /// <exception cref="InvalidOperationException">The spinbox already has a child.</exception>
        public override void Append(Control child)
        {
            if (_child != null)
                throw new InvalidOperationException(
                    "A Spinbox holds one control. Wrap several in a VBox or HBox.");

            base.Append(child);
            _child = child;
        }

        protected override void OnDestroying()
        {
            _child = null;
            base.OnDestroying();
        }

        #region CALLBACKS

        private SpinButtonCallback _spinCB;
        private IFni _spinCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action called each time the user clicks one of the spin
        /// buttons. The callback data carries the increment: +1 for up and -1 for down,
        /// multiplied by 2 while Shift is held, 10 for Ctrl, and 100 for both.
        /// </summary>
        public SpinButtonCallback SpinCB
        {
            get => _spinCB;
            set
            {
                _spinCB = value;
                _spinCBInternal = SpinCBInternal;
                SetCallback("SPIN_CB", Utils.CastCallback<Icallback>(_spinCBInternal));
            }
        }
        private int SpinCBInternal(nint ih, int inc)
        {
            try
            {
                var cb = new SpinButtonData(this, inc);
                _spinCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Spinbox SpinCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}