using IupSharp;
using System.Drawing;

namespace IupSharpTest
{
    internal class Program
    {

        static Canvas can;
        static Dialog dlg;
        static UITimer tt;

        static void Main(string[] args)
        {
            Iup.Open();

            Button btn;
            

            dlg = new Dialog(
                new VBox(
                    btn = new Button("My button") { Expand = Expand.Horizontal, Action = ButtonAction, BgColor = Color.CornflowerBlue, FgColor = Color.Red },
                    new Toggle("My toggle") { Action = ToggleAction },
                    can=new Canvas() { RasterSize=(200,200), BgColor = Color.Black,Action=RedrawCanvas },
                    new Text("Hello") { Expand=Expand.Horizontal},
                    new Label("The end")
                )
                { Gap=8,Margin=(8,8)}
            )
            { CloseCB = ClosaCall,KAny=DialogKey,Shrink=false,DestroyOnClose=true};

            var col = btn.FgColor;

            btn.FgColor = Color.Empty;


            tt = new UITimer(500, OnTimer, true);
            
            dlg.Popup();

            

            Iup.Close();
        }


        static int testCount = 0;
        private static void OnTimer(CallbackData d)
        {
            testCount++;
            dlg.Title=testCount.ToString()+" "+tt.Wid.ToString();
            
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
            
        }

        private static void ClosaCall(CallbackData d)
        {

            int debug = 0;

        }

        private static void ButtonAction(CallbackData d)
        {
            tt.Running = !tt.Running;

            
        }

        private static void ToggleAction(ToggleActionData d)
        {
            
        }
    }
}
