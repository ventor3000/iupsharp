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

        static void Main(string[] args)
        {
            Iup.Open();
            ImageLib.Open();

            Button btn;


            var dropcontent = 
                new VBox(
                    new Button("Alfa"),
                    new Button("Beta"),
                    new Button("Gamma"));

            ImageLib.StockSize = 96;

            dlg = new Dialog(
                new VBox(
                    btn = new Button("My button") { ImageName=ImageLib.Print,ImPressName=ImageLib.FileNew, Expand = Expand.Horizontal, Action = ButtonAction, BgColor = Color.CornflowerBlue, FgColor = Color.Red },
                    new Toggle("My toggle") { Action = ToggleAction },
                    can = new Canvas() { RasterSize = (200, 200), BgColor = Color.Black, Action = RedrawCanvas },
                    new Text("Hello\nworld", true) { Expand = Expand.Yes, BgColor = Color.Blue },
                    new Label("The end"),
                    dropbtn = new DropButton("My dropdown", dropcontent) { CanFocus = false },
                    lst = new List(ListStyle.Multiple, "Kalle", "Olle", "Pelle", "Niklas") { SelectedPosition = 25 },
                    pbar = new ProgressBar(Orientation.Vertical, false, false) { Min = 0, Max = 100, Value = 45 },
                    new Link("www.google.com", "Press here to go to google") { Action = LinkAction },
                    valuator = new Valuator(Orientation.Horizontal, 5, TicksPosition.Reverse) { ValueChangedCB = ValChanged, Expand = Expand.Horizontal },
                    new DatePick() { Value = new DateTime(1973, 10, 19), Separator = "~" },
                  
                    
                    radio=new Radio(new VBox(
                        tog1=new Toggle("Alfa"),
                        tog2 = new Toggle("Beta"),
                        tog3 = new Toggle("Gamma"),
                        new Toggle("toggle me") { IgnoreRadio= true },
                        tog4 = new Toggle("Delta")
                        )),

                    animatedlabel= new AnimatedLabel() { AnimationName = ImageLib.CircleProgressAnimation }
                    
                )
               
            ) { CloseCB = ClosaCall,KAny=DialogKey,Shrink=false,DestroyOnClose=true};

            lst.SelectedPositions = new[] { 1, 4 };

            animatedlabel.Start();

            dlg.Menu = new Menu(
                new Submenu("File", new Menu(
                        new MenuItem("Kalle"),
                        new MenuItem("Olle"),
                        new MenuItem("Pelle",PelleClick)
                    )),
                editMenu=new Submenu("Edit", new Menu(
                        new MenuItem("Kalle 2"),
                        new MenuItem("Olle 2"),
                        new MenuItem("Pelle 2")
                    )),
                new Submenu("Help"));

            var col = btn.FgColor;
            //dropbtn.DropChild = dropdlg;

            btn.FgColor = Color.Empty;


            tt = new UITimer(50, OnTimer, true);
            
            dlg.Popup();

            

            Iup.Close();
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

            
        }

        private static void ToggleAction(ToggleActionData d)
        {
            
        }
    }
}
