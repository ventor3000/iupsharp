using IupSharp;
using System.Drawing;

namespace IupSharpTest
{
    internal class Program
    {

        static Canvas can;
        static void Main(string[] args)
        {
            Iup.Open();

            Button btn;
            

            using Dialog dlg = new Dialog(
                new VBox(
                    btn = new Button("My button") { Expand = Expand.Horizontal, Action = ButtonAction, BgColor = Color.CornflowerBlue, FgColor = Color.Red },
                    new Toggle("My toggle") { Action = ToggleAction },
                    can=new Canvas() { RasterSize=(200,200), BgColor = Color.Black,Action=RedrawCanvas },
                    new Label("The end")
                )
            )
            { CloseCB = ClosaCall,KAny=DialogKey,Shrink=false};

            var col = btn.FgColor;

            btn.FgColor = Color.Empty;
            

            dlg.Popup();

            

            Iup.Close();
        }

        private static void RedrawCanvas(CanvasActionData d)
        {
            int debug = 0;
            string ss = can.DrawDriver;
        }

        private static void DialogKey(KeyCBData d)
        {
            if(d.Key!=(Key.Shift|Key.LeftShift))
                Iup.Message(d.Key.Describe());
        }

        private static void ClosaCall(CallbackData d)
        {
            
            Iup.Message("Hello");
            d.Result = IupNative.IUP_CONTINUE;

        }

        private static void ButtonAction(CallbackData d)
        {
            Iup.MessageError("test");
        }

        private static void ToggleAction(ToggleActionData d)
        {
            
        }
    }
}
