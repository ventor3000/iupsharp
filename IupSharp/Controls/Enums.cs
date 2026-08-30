using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IupSharp.Controls
{
    /// <summary>
    /// Data for a callback that reports an angle, used by the Dial press, move and
    /// release callbacks.
    /// </summary>
    public class AngleData : CallbackData
    {
        /// <summary>
        /// The angle, in whatever unit the dial's Unit property selects - radians by
        /// default. Note this differs from the dial's Value property, which is always
        /// in radians.
        /// </summary>
        public readonly double Angle;

        public AngleData(Control sender, double angle) : base(sender)
        {
            this.Angle = angle;
        }
    }
    public delegate void AngleCallback(AngleData d);
}
