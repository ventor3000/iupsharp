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
        static List lst;
        static ProgressBar pbar;
        static Valuator valuator;
        static AnimatedLabel animatedlabel;
        static Radio radio;
        static Toggle tog1, tog2, tog3, tog4;
        static Tabs tabs;
        static Zbox zbox;

        static void Main(string[] args)
        {
            Iup.Open();
            ImageLib.Open();
            // NativeIupControls.IupControlsOpen();

            Button btn;


            var dropcontent =
                new VBox(
                    new Button("Alfa"),
                    new Button("Beta"),
                    new Button("Gamma"));

            ImageLib.StockSize = 96;

            dlg = new Dialog(
                new VBox(
                    new Button("Toggle") { Action=BtnAction,Expand = Expand.Yes },
                    zbox=new Zbox(
                        new VBox(
                            new Button("A"),
                            new Button("B"),
                            new Button("C") { Expand = Expand.Yes }
                        ),
                        new VBox(
                            new Button("D"),
                            new Button("E"),
                            new Button("F")
                            )
                        )
                    )
                );
            
            dlg.Popup();

            

            Iup.Close();
        }

        private static void BtnAction(CallbackData d)
        {
            zbox.SelectedIndex = ((zbox.SelectedIndex + 1) % zbox.Count);
        }

        private static void LinkAction(LinkActionData d)
        {
            Iup.Message(d.Url);
            d.Result = CallbackResult.Continue;
        }

        private static void ValChanged(CallbackData d)
        {
            dlg.Title = valuator.Value.ToString();

            pbar.Value = pbar.Min + (pbar.Max - pbar.Min) * valuator.Value;
        }

        private static void PelleClick(CallbackData cd)
        {
            Param a = Param.Int("Mitt heltal");
            Param b = Param.Int("Mitt andra heltal", 3, -1, 5);
            Param c = Param.Bool("Vill du testa?");
            Param d = Param.Angle("Ange vinkel",90);
            Param e = Param.Font("Ange typsnitt");
            DoubleParam f = Param.Real("Ange flyttal",3.5);
            ColorParam par = Param.Color("ANge färg", Color.Pink);

            

            if (Iup.GetParams("Ange parametrar",ParamCb, a, b, c, d,e,f,par))
                Iup.Message("Bekräftat");


            Iup.Message(f.Value.ToString());
            
        }

        private static void ParamCb(ParamCallbackData d)
        {
            if(d.Parameter is DoubleParam)
                d.Accept = false;
            
        }

        private static void NewItemClick(CallbackData d)
        {
            Iup.Message("Hello world");
        }

        static int testCount = 0;
        private static void OnTimer(CallbackData d)
        {
            /*
            
            var val = pbar.Value;
            val++;
            if (val > pbar.Max)
                val = pbar.Min;

            pbar.Value = val;*/
            
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

            tog2.Checked = true;

            int i = tabs.TabItems.Count;

            tabs.TabItems[1].Title = "Test!";

        }

        private static void ToggleAction(ToggleActionData d)
        {
            
        }
    }
}
