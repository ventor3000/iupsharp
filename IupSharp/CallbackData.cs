using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IupSharp
{
    public class CallbackData
    {
        public readonly IupObject Sender;
        public CallbackResult Result = CallbackResult.Default;

        public CallbackData(IupObject sender)
        {
            this.Sender = sender;
        }
    }
    public delegate void Callback(CallbackData d);

    public enum MouseButton
    {
        Left=1,
        Middle=2,
        Right=3,
        Button4=4,
        Button5=5
    }

    public class ButtonCBData:CallbackData
    {

        public readonly MouseButton Button;
        public readonly bool Pressed;
        public readonly int X;
        public readonly int Y;
        public readonly string Status;

        public ButtonCBData(Control sender,int button, int pressed, int x, int y, string status):base(sender)
        {
            this.Button = (MouseButton)(button-'0');
            this.Pressed = pressed==0 ? false:true;
            this.X = x;
            this.Y = y;
            this.Status = status;
        }
    }
    public delegate void ButtonCBCallback(ButtonCBData d);
    

    public class ToggleActionData : CallbackData
    {
        public bool Checked;
        
        public ToggleActionData(Control sender, int state):base(sender)
        {
            this.Checked = state == 0 ? false : true;
        }
    }
    public delegate void ToggleActionCallback(ToggleActionData d);


    public class MotionCBData : CallbackData
    {
        public readonly int X;
        public readonly int Y;
        public readonly string Status;
        public MotionCBData(Control sender, int x, int y, string status) : base(sender)
        {
            this.X = x;
            this.Y = y;
            this.Status = status;
        }
    }
    public delegate void MotionCBCallback(MotionCBData d);

    public class DropFilesCBData : CallbackData
    {
        public readonly string FileName;
        public readonly int X;
        public readonly int Y;
        public readonly int Index;
        
        public DropFilesCBData(Control sender, string filename,int num,int x, int y) : base(sender)
        {
            this.FileName = filename;
            this.X = x;
            this.Y = y;
            this.Index = num;
        }
    }
    public delegate void DropFilesCBCallback(DropFilesCBData d);


    /// <summary>Data for the ShowCB callback.</summary>
    public class ShowCBData : CallbackData
    {
        /// <summary>What happened to the dialog.</summary>
        public readonly ShowState State;

        public ShowCBData(Control sender, int state) : base(sender)
        {
            this.State = (ShowState)state;
        }
    }
    public delegate void ShowCBCallback(ShowCBData d);


    /// <summary>Data for the ResizeCB callback. Sizes are in pixels and refer to the client area.</summary>
    public class ResizeCBData : CallbackData
    {
        public readonly int Width;
        public readonly int Height;

        public ResizeCBData(Control sender, int width, int height) : base(sender)
        {
            this.Width = width;
            this.Height = height;
        }
    }
    public delegate void ResizeCBCallback(ResizeCBData d);


    /// <summary>Data for the MoveCB callback. Coordinates are the new screen position.</summary>
    public class MoveCBData : CallbackData
    {
        public readonly int X;
        public readonly int Y;

        public MoveCBData(Control sender, int x, int y) : base(sender)
        {
            this.X = x;
            this.Y = y;
        }
    }
    public delegate void MoveCBCallback(MoveCBData d);


    /// <summary>
    /// Data for the TrayClickCB callback. Note that the button numbering here is 1, 2, 3
    /// and is NOT the same as the character codes used by ButtonCB.
    /// </summary>
    public class TrayClickCBData : CallbackData
    {
        /// <summary>1 = left, 2 = middle, 3 = right. GTK does not report the middle button.</summary>
        public readonly MouseButton Button;
        /// <summary>True while the button is down. Always true in GTK.</summary>
        public readonly bool Pressed;
        /// <summary>True on a double click. Simulated in GTK.</summary>
        public readonly bool DoubleClick;

        public TrayClickCBData(Control sender, int but, int pressed, int dclick) : base(sender)
        {
            this.Button = (MouseButton)but;
            this.Pressed = pressed != 0;
            this.DoubleClick = dclick != 0;
        }
    }
    public delegate void TrayClickCBCallback(TrayClickCBData d);


    /// <summary>
    /// Data for the KAny callback. The key code combines the base key with modifier
    /// flags; use the Iup key helpers or compare against the K_* constants.
    /// </summary>
    public class KeyCBData : CallbackData
    {
        /// <summary>The key code, including modifier bits.</summary>
        public readonly Key Key;

        public KeyCBData(Control sender, Key key) : base(sender)
        {
            this.Key = key;
        }
    }
    public delegate void KeyCBCallback(KeyCBData d);


    /// <summary>
    /// Data for the canvas Action callback, generated when the canvas needs to be
    /// redrawn.
    /// </summary>
    public class CanvasActionData : CallbackData
    {
        /// <summary>
        /// Thumb position in the horizontal scrollbar. This is the old float-format
        /// parameter; read the canvas PosX property for the full double precision
        /// value.
        /// </summary>
        public readonly float PosX;

        /// <summary>
        /// Thumb position in the vertical scrollbar. This is the old float-format
        /// parameter; read the canvas PosY property for the full double precision
        /// value.
        /// </summary>
        public readonly float PosY;

        public CanvasActionData(Control sender, float posx, float posy) : base(sender)
        {
            this.PosX = posx;
            this.PosY = posy;
        }
    }
    public delegate void CanvasActionCallback(CanvasActionData d);


    /// <summary>Data for the ScrollCB callback.</summary>
    public class ScrollCBData : CallbackData
    {
        /// <summary>The operation performed on the scrollbar.</summary>
        public readonly ScrollOperation Operation;

        /// <summary>Thumb position in the horizontal scrollbar, as float.</summary>
        public readonly float PosX;

        /// <summary>Thumb position in the vertical scrollbar, as float.</summary>
        public readonly float PosY;

        /// <summary>True if the operation was on the vertical scrollbar.</summary>
        public bool IsVertical => Operation <= ScrollOperation.DragV;

        public ScrollCBData(Control sender, int op, float posx, float posy) : base(sender)
        {
            this.Operation = (ScrollOperation)op;
            this.PosX = posx;
            this.PosY = posy;
        }
    }
    public delegate void ScrollCBCallback(ScrollCBData d);


    /// <summary>Data for the canvas FocusCB callback.</summary>
    public class FocusCBData : CallbackData
    {
        /// <summary>True if the canvas is gaining the focus, false if losing it.</summary>
        public readonly bool HasFocus;

        public FocusCBData(Control sender, int focus) : base(sender)
        {
            this.HasFocus = focus != 0;
        }
    }
    public delegate void FocusCBCallback(FocusCBData d);


    /// <summary>Data for the KeyPressCB callback.</summary>
    public class KeyPressCBData : CallbackData
    {
        /// <summary>The key, including modifier flags.</summary>
        public readonly Key Key;

        /// <summary>True if the key was pressed, false if released.</summary>
        public readonly bool Pressed;

        public KeyPressCBData(Control sender, int key, int press) : base(sender)
        {
            this.Key = (Key)key;
            this.Pressed = press != 0;
        }
    }
    public delegate void KeyPressCBCallback(KeyPressCBData d);


    /// <summary>Data for the WheelCB callback.</summary>
    public class WheelCBData : CallbackData
    {
        /// <summary>
        /// Wheel rotation. Positive is away from the user, negative is toward. Usually
        /// 1 or -1 per notch, but a high-resolution wheel may report fractions.
        /// </summary>
        public readonly float Delta;

        /// <summary>Mouse x position in pixels, relative to the top-left of the canvas.</summary>
        public readonly int X;

        /// <summary>Mouse y position in pixels, relative to the top-left of the canvas.</summary>
        public readonly int Y;

        /// <summary>The modifier and button status string.</summary>
        public readonly string Status;

        public WheelCBData(Control sender, float delta, int x, int y, string status) : base(sender)
        {
            this.Delta = delta;
            this.X = x;
            this.Y = y;
            this.Status = status;
        }
    }
    public delegate void WheelCBCallback(WheelCBData d);


    /// <summary>Data for the TouchCB callback.</summary>
    public class TouchCBData : CallbackData
    {
        /// <summary>Identifies the touch point.</summary>
        public readonly int Id;

        /// <summary>Position in pixels, relative to the top-left of the canvas.</summary>
        public readonly int X;

        /// <summary>Position in pixels, relative to the top-left of the canvas.</summary>
        public readonly int Y;

        /// <summary>The touch point state.</summary>
        public readonly TouchState State;

        /// <summary>True if this is the primary touch point.</summary>
        public readonly bool IsPrimary;

        /// <summary>The raw state string as reported by IUP.</summary>
        public readonly string RawState;

        public TouchCBData(Control sender, int id, int x, int y, string state) : base(sender)
        {
            this.Id = id;
            this.X = x;
            this.Y = y;
            this.RawState = state;

            // IUP appends "-PRIMARY" to the state of the primary point.
            this.IsPrimary = state != null && state.EndsWith("-PRIMARY", StringComparison.Ordinal);

            string s = state;
            if (IsPrimary)
                s = state.Substring(0, state.Length - "-PRIMARY".Length);

            this.State = s switch
            {
                "DOWN" => TouchState.Down,
                "MOVE" => TouchState.Move,
                "UP" => TouchState.Up,
                _ => TouchState.Unknown
            };
        }
    }
    public delegate void TouchCBCallback(TouchCBData d);


    /// <summary>A single point reported by the MultiTouchCB callback.</summary>
    public readonly struct TouchPoint
    {
        /// <summary>Identifies the touch point.</summary>
        public readonly int Id;
        /// <summary>Position in pixels, relative to the top-left of the canvas.</summary>
        public readonly int X;
        /// <summary>Position in pixels, relative to the top-left of the canvas.</summary>
        public readonly int Y;
        /// <summary>The touch point state.</summary>
        public readonly TouchState State;

        public TouchPoint(int id, int x, int y, TouchState state)
        {
            Id = id;
            X = x;
            Y = y;
            State = state;
        }
    }

    /// <summary>Data for the MultiTouchCB callback.</summary>
    public class MultiTouchCBData : CallbackData
    {
        /// <summary>
        /// The touch points. Copied out of the native arrays during construction, so
        /// this stays valid after the callback returns.
        /// </summary>
        public readonly TouchPoint[] Points;

        public MultiTouchCBData(Control sender, int count, IntPtr pid, IntPtr px, IntPtr py, IntPtr pstate)
            : base(sender)
        {
            if (count <= 0 || pid == IntPtr.Zero || px == IntPtr.Zero ||
                py == IntPtr.Zero || pstate == IntPtr.Zero)
            {
                Points = Array.Empty<TouchPoint>();
                return;
            }

            // The native arrays are owned by IUP and only valid for the duration of
            // the call, so copy them before the callback returns.
            int[] ids = new int[count];
            int[] xs = new int[count];
            int[] ys = new int[count];
            int[] states = new int[count];

            Marshal.Copy(pid, ids, 0, count);
            Marshal.Copy(px, xs, 0, count);
            Marshal.Copy(py, ys, 0, count);
            Marshal.Copy(pstate, states, 0, count);

            Points = new TouchPoint[count];
            for (int i = 0; i < count; i++)
            {
                // States arrive as the character codes 'D', 'U' and 'M'.
                TouchState st = states[i] switch
                {
                    'D' => TouchState.Down,
                    'U' => TouchState.Up,
                    'M' => TouchState.Move,
                    _ => TouchState.Unknown
                };
                Points[i] = new TouchPoint(ids[i], xs[i], ys[i], st);
            }
        }
    }
    public delegate void MultiTouchCBCallback(MultiTouchCBData d);


    /// <summary>Data for the WomCB audio device callback.</summary>
    public class WomCBData : CallbackData
    {
        /// <summary>
        /// The device state: 0 when the device is opened, 1 when a buffer finishes
        /// playing, 2 when the device is closed.
        /// </summary>
        public readonly int State;

        public WomCBData(Control sender, int state) : base(sender)
        {
            this.State = state;
        }
    }
    public delegate void WomCBCallback(WomCBData d);

    /// <summary>
    /// Data for the PostMessageCB callback, carrying the values passed to
    /// PostMessage.
    /// </summary>
    public class PostMessageData : CallbackData
    {
        /// <summary>The name passed to PostMessage. May be null.</summary>
        public readonly string Text;

        /// <summary>The integer passed to PostMessage.</summary>
        public readonly int Value;

        /// <summary>The double passed to PostMessage.</summary>
        public readonly double Number;

        /// <summary>
        /// The pointer passed to PostMessage, or IntPtr.Zero. The sender owns whatever
        /// it points at; IupSharp does not manage its lifetime.
        /// </summary>
        public readonly IntPtr Pointer;

        public PostMessageData(IupObject sender, string text, int value, double number, IntPtr pointer)
            : base(sender)
        {
            this.Text = text;
            this.Value = value;
            this.Number = number;
            this.Pointer = pointer;
        }
    }

    public delegate void PostMessageCallback(PostMessageData d);
}
