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
            { CloseCB = ClosaCall,KAny=DialogKey,Shrink=false,DestroyOnClose=true};

            var col = btn.FgColor;

            btn.FgColor = Color.Empty;
            

            dlg.Popup();

            

            Iup.Close();
        }

        private static void RedrawCanvas(CanvasActionData d)
        {
            using (Draw draw = new Draw(can))
            {

                var tt = can.DrawDriver;

                string ss = draw.Driver;

                draw.Color = Color.Red;
                draw.LineWidth = 5; 
                draw.Line(100, 100, 200, 200);
                draw.Color = Color.Yellow;
                draw.Font = "Times, -50";
                draw.Text("Hello!", 150, 200);
            }

        }

        private static void DialogKey(KeyCBData d)
        {
            if(d.Key!=(Key.Shift|Key.LeftShift))
                Iup.Message(d.Key.Describe());
        }

        private static void ClosaCall(CallbackData d)
        {
            
            

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
