using IupSharp;
using System.Drawing;

namespace IupSharpTest
{
    internal class Program
    {

        static Canvas can;
        static Dialog dlg;
        static UITimer tt;
        static DropButton dropbtn;
        static Submenu editMenu;

        static void Main(string[] args)
        {
            Iup.Open();

            Button btn;


            var dropcontent = 
                new VBox(
                    new Button("Alfa"),
                    new Button("Beta"),
                    new Button("Gamma"));


            dlg = new Dialog(
                new VBox(
                    btn = new Button("My button") { Expand = Expand.Horizontal, Action = ButtonAction, BgColor = Color.CornflowerBlue, FgColor = Color.Red },
                    new Toggle("My toggle") { Action = ToggleAction },
                    can=new Canvas() { RasterSize=(200,200), BgColor = Color.Black,Action=RedrawCanvas },
                    new Text("Hello\nworld",true) { Expand=Expand.Yes,BgColor=Color.Blue },
                    new Label("The end"),
                    dropbtn=new DropButton("My dropdown", dropcontent) { CanFocus=false}
                )
                { Gap=8,Margin=(8,8)}
            )
            { CloseCB = ClosaCall,KAny=DialogKey,Shrink=false,DestroyOnClose=true};


            dlg.Menu = new Menu(
                new Submenu("File", new Menu(
                        new Item("Kalle"),
                        new Item("Olle"),
                        new Item("Pelle",PelleClick)
                    )),
                editMenu=new Submenu("Edit", new Menu(
                        new Item("Kalle 2"),
                        new Item("Olle 2"),
                        new Item("Pelle 2")
                    )),
                new Submenu("Help"));

            var col = btn.FgColor;
            //dropbtn.DropChild = dropdlg;

            btn.FgColor = Color.Empty;


            tt = new UITimer(500, OnTimer, true);
            
            dlg.Popup();

            

            Iup.Close();
        }

        private static void PelleClick(CallbackData d)
        {
            Item i = new Item("New item",NewItemClick);
            editMenu.Menu.Append(i);
            
            i.Map();
        }

        private static void NewItemClick(CallbackData d)
        {
            Iup.Message("Hello world");
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
