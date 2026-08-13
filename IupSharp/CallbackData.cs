using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IupSharp
{
    public class CallbackData
    {
        public readonly IupObject Sender;
        public int Result = IupNative.IUP_DEFAULT;

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
}
