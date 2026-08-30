using System;
using System.Runtime.InteropServices;

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

    /// <summary>
    /// Data for the Text Action callback, raised before the value actually changes.
    /// </summary>
    public class TextActionData : CallbackData
    {
        /// <summary>
        /// The typed character, or '\0' when the change did not come from a single
        /// keystroke - for example a paste or an undo.
        /// </summary>
        public readonly char Character;

        /// <summary>The text the control would have after the change.</summary>
        public readonly string NewValue;

        /// <summary>
        /// Set this to substitute a different character for the one typed. Only
        /// meaningful when Character is not '\0'. Leave it as '\0' to accept the
        /// original, or set Result to Ignore to reject the change entirely.
        /// </summary>
        public char Replacement;

        public TextActionData(Control sender, int c, string newValue) : base(sender)
        {
            this.Character = c == 0 ? '\0' : (char)c;
            this.NewValue = newValue;
            this.Replacement = '\0';
        }
    }
    public delegate void TextActionCallback(TextActionData d);


    /// <summary>Data for the CaretCB callback.</summary>
    public class CaretCBData : CallbackData
    {
        /// <summary>Line number, starting at 1. Always 1 for a single line control.</summary>
        public readonly int Line;

        /// <summary>Column number, starting at 1.</summary>
        public readonly int Column;

        /// <summary>
        /// Zero based character position. For a single line control this is always
        /// Column - 1.
        /// </summary>
        public readonly int Position;

        public CaretCBData(Control sender, int lin, int col, int pos) : base(sender)
        {
            this.Line = lin;
            this.Column = col;
            this.Position = pos;
        }
    }
    public delegate void CaretCBCallback(CaretCBData d);


    /// <summary>Data for the SpinCB callback.</summary>
    public class SpinCBData : CallbackData
    {
        /// <summary>The spin value, after it was incremented or decremented.</summary>
        public readonly int Value;

        public SpinCBData(Control sender, int pos) : base(sender)
        {
            this.Value = pos;
        }
    }
    public delegate void SpinCBCallback(SpinCBData d);

    /// <summary>
    /// Data for the DropDownCB and DropShowCB callbacks. For DropDownCB the state is
    /// the one about to be applied; for DropShowCB it is the current one.
    /// </summary>
    public class DropStateData : CallbackData
    {
        /// <summary>True when the drop child is being shown, false when hidden.</summary>
        public readonly bool Shown;

        public DropStateData(Control sender, int state) : base(sender)
        {
            this.Shown = state != 0;
        }
    }
    public delegate void DropStateCallback(DropStateData d);

    // =====================================================================
    // Add to CallbackData.cs
    // =====================================================================

    /// <summary>
    /// Data for the List Action callback, raised when an item's selection state
    /// changes.
    /// </summary>
    public class ListActionData : CallbackData
    {
        /// <summary>Text of the item whose state changed.</summary>
        public readonly string Text;

        /// <summary>One-based position of the item whose state changed.</summary>
        public readonly int Position;

        /// <summary>True if the item was selected, false if it was deselected.</summary>
        public readonly bool Selected;

        public ListActionData(Control sender, string text, int item, int state) : base(sender)
        {
            this.Text = text;
            this.Position = item;
            this.Selected = state != 0;
        }
    }
    public delegate void ListActionCallback(ListActionData d);


    /// <summary>
    /// Data for the MultiSelectCB callback, raised once a multiple selection
    /// interaction is over.
    /// </summary>
    public class MultiSelectData : CallbackData
    {
        /// <summary>
        /// The raw IUP string: '+' for a newly selected item, '-' for a newly
        /// deselected one, and 'x' for an item whose state did not change. One
        /// character per item, in order.
        /// </summary>
        public readonly string Value;

        /// <summary>One-based positions of the items just selected.</summary>
        public readonly int[] Selected;

        /// <summary>One-based positions of the items just deselected.</summary>
        public readonly int[] Deselected;

        /// <summary>One-based positions of every currently selected item.</summary>
        public readonly int[] AllSelected;

        public MultiSelectData(Control sender, string value) : base(sender)
        {
            this.Value = value;

            if (string.IsNullOrEmpty(value))
            {
                Selected = Array.Empty<int>();
                Deselected = Array.Empty<int>();
                AllSelected = Array.Empty<int>();
                return;
            }

            var sel = new System.Collections.Generic.List<int>();
            var desel = new System.Collections.Generic.List<int>();
            var all = new System.Collections.Generic.List<int>();

            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '+':
                        sel.Add(i + 1);
                        all.Add(i + 1);
                        break;
                    case '-':
                        desel.Add(i + 1);
                        break;
                        // 'x' means unchanged; it does not say whether the item is
                        // selected, so it cannot contribute to AllSelected.
                }
            }

            Selected = sel.ToArray();
            Deselected = desel.ToArray();
            AllSelected = all.ToArray();
        }
    }
    public delegate void MultiSelectCallback(MultiSelectData d);


    /// <summary>Data for the DblClickCB callback.</summary>
    public class DblClickData : CallbackData
    {
        /// <summary>One-based position of the double clicked item.</summary>
        public readonly int Position;

        /// <summary>Text of the double clicked item.</summary>
        public readonly string Text;

        public DblClickData(Control sender, int item, string text) : base(sender)
        {
            this.Position = item;
            this.Text = text;
        }
    }
    public delegate void DblClickCallback(DblClickData d);


    /// <summary>
    /// Data for the DragDropCB callback, raised when an internal drag and drop of
    /// items completes.
    /// </summary>
    public class DragDropData : CallbackData
    {
        /// <summary>One-based position where the drag started.</summary>
        public readonly int DragPosition;

        /// <summary>
        /// One-based position where the drop happened, or -1 for a drop in blank
        /// space.
        /// </summary>
        public readonly int DropPosition;

        /// <summary>True if Shift was held.</summary>
        public readonly bool Shift;

        /// <summary>True if Ctrl was held, which copies rather than moves.</summary>
        public readonly bool Control;

        public DragDropData(Control sender, int dragId, int dropId, int isShift, int isControl)
            : base(sender)
        {
            this.DragPosition = dragId;
            this.DropPosition = dropId;
            this.Shift = isShift != 0;
            this.Control = isControl != 0;
        }
    }
    public delegate void DragDropCallback(DragDropData d);

    /// <summary>Data for the Link Action callback.</summary>
    public class LinkActionData : CallbackData
    {
        /// <summary>
        /// The destination address, as IUP passed it. Normally the same as the link's
        /// Url property.
        /// </summary>
        public readonly string Url;

        public LinkActionData(Control sender, string url) : base(sender)
        {
            this.Url = url;
        }
    }
    public delegate void LinkActionCallback(LinkActionData d);
}
