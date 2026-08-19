using IupSharp;
using System.Drawing;

namespace IupSharpTest
{
    internal class Program
    {
        

        static void Main(string[] args)
        {
            Iup.Open();

            Button btn;

            Dialog dlg = new Dialog(
                new VBox(
                    btn=new Button("My button") { Expand = Expand.Horizontal, Action = ButtonAction, BgColor = Color.CornflowerBlue,FgColor=Color.Red },
                    new Toggle("My toggle") { Action = ToggleAction }
                )
                { }

            )
            { CloseCB = ClosaCall,KAny=DialogKey };

            var col = btn.FgColor;

            btn.FgColor = Color.Empty;

            dlg.Popup();

            dlg.Destroy();

            Iup.Close();
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
            Iup.MessageError("Error!");
        }

        private static void ToggleAction(ToggleActionData d)
        {
            
        }
    }
}
