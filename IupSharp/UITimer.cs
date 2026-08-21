using System;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Creates a timer that periodically invokes a callback when the time is up.
    /// </summary>
    /// <remarks>
    /// <para>A timer is not an interface element: it has no visual representation and
    /// cannot be added to a container. It is mapped only while it is running.</para>
    ///
    /// <para>The callback is dispatched from the main loop, so it will not fire unless
    /// a loop is running. It is also not a real-time guarantee - a long-running
    /// callback delays the next tick.</para>
    ///
    /// <para>Timers must be destroyed explicitly, either by calling Destroy or by
    /// letting Iup.Close clean them up at shutdown. Dropping the last reference is not
    /// enough: IupSharp keeps every live element reachable until its native element is
    /// destroyed.</para>
    ///
    /// <example>
    /// <code>
    /// var timer = new Timer(1000);
    /// timer.Action = d =&gt; Debug.WriteLine($"tick at {((Timer)d.Sender).ElapsedTime} ms");
    /// timer.Start();
    /// </code>
    /// </example>
    /// </remarks>
    public class UITimer : MappableObject
    {
        /// <summary>
        /// Creates a new timer, stopped, with the default interval.
        /// </summary>
        public UITimer() : base(IupNative.IupTimer())
        {
        }

        /// <summary>
        /// Creates a new timer, stopped, with the given interval in milliseconds.
        /// </summary>
        /// <param name="milliseconds">
        /// The interval. On Windows the minimum accepted by the system is 10 ms.
        /// </param>
        public UITimer(int milliseconds) : this()
        {
            Time = milliseconds;
        }

        /// <summary>
        /// Creates a new timer with the given interval and callback, optionally started
        /// immediately.
        /// </summary>
        /// <param name="milliseconds">The interval in milliseconds.</param>
        /// <param name="action">The callback invoked each time the interval elapses.</param>
        /// <param name="start">Whether to start the timer right away.</param>
        public UITimer(int milliseconds, Callback action, bool start = true) : this(milliseconds)
        {
            Action = action;

            if (start)
                Start();
        }

        #region ATTRIBUTES

        /// <summary>
        /// Gets or sets the interval between callbacks, in milliseconds. On Windows the
        /// system minimum is 10 ms; smaller values are clamped by the platform rather
        /// than by IUP.
        /// </summary>
        /// <remarks>
        /// Changing this while the timer is running does not reliably take effect on
        /// every platform. To be safe, stop the timer, set the interval, then start it
        /// again - which is what SetTime does.
        /// </remarks>
        public int Time
        {
            get
            {
                string v = GetAttribute("TIME");
                return int.TryParse(v, CultureInfo.InvariantCulture, out int t) ? t : 0;
            }
            set => SetAttribute("TIME", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets whether the timer is running. Setting it is equivalent to
        /// calling Start or Stop.
        /// </summary>
        /// <remarks>
        /// With multiple threads, start the timer from the main thread.
        /// </remarks>
        public bool Running
        {
            get => GetAttribute("RUN") == "YES";
            set => SetAttribute("RUN", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets the time in milliseconds since the timer was started.
        /// </summary>
        /// <remarks>
        /// Only meaningful inside the Action callback; the value is undefined
        /// elsewhere. (since 3.15)
        /// </remarks>
        public int ElapsedTime
        {
            get
            {
                string v = GetAttribute("ELAPSEDTIME");
                return int.TryParse(v, CultureInfo.InvariantCulture, out int t) ? t : 0;
            }
        }

        /// <summary>
        /// Gets the native serial number of the timer, or -1 when it is not running.
        /// A timer is mapped only while running.
        /// </summary>
        public int Wid
        {
            get
            {
                string v = GetAttribute("WID");
                return int.TryParse(v, CultureInfo.InvariantCulture, out int w) ? w : -1;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Starts the timer. Starting an already running timer has no effect.
        /// </summary>
        public void Start() => Running = true;

        /// <summary>
        /// Stops the timer. The Action callback will no longer be invoked.
        /// </summary>
        public void Stop() => Running = false;

        /// <summary>
        /// Restarts the timer, so the next tick is a full interval away. Useful for
        /// debounce-style behaviour where an event should reset the countdown.
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }

        /// <summary>
        /// Sets the interval and restarts the timer if it was running, which is the
        /// portable way to change the interval of a live timer.
        /// </summary>
        /// <param name="milliseconds">The new interval in milliseconds.</param>
        public void SetTime(int milliseconds)
        {
            bool wasRunning = Running;

            if (wasRunning)
                Stop();

            Time = milliseconds;

            if (wasRunning)
                Start();
        }

        /// <summary>
        /// Stops the timer and clears its callback, then destroys it. Convenience for
        /// the common teardown sequence.
        /// </summary>
        public void StopAndDestroy()
        {
            if (Handle == IntPtr.Zero)
                return;

            Stop();
            Action = null;
            Destroy();
        }

        #endregion

        #region CALLBACKS

        private Callback _action;
        private IFn _actionInternal; // need reference to keep alive in GC

        /// <summary>
        /// Gets or sets the action invoked each time the interval elapses. Read
        /// ElapsedTime inside the callback to find how long the timer has been running.
        /// Set the callback data's Result to Close to end the main loop.
        /// </summary>
        /// <remarks>
        /// To stop the callback being invoked, stop the timer - clearing this property
        /// alone does not stop it.
        /// </remarks>
        public Callback Action
        {
            get => _action;
            set
            {
                _action = value;
                _actionInternal = ActionInternal;
                SetCallback("ACTION_CB", Utils.CastCallback<Icallback>(_actionInternal));
            }
        }

        private int ActionInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _action?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in Timer Action callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion

        protected override void OnDestroying()
        {
            _action = null;
            base.OnDestroying();
        }
    }
}