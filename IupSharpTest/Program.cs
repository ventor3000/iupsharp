using IupSharp;

namespace IupSharpTest
{
    internal class Program
    {
        

        static void Main(string[] args)
        {
            Iup.Open();


            Dialog dlg = new Dialog(
                new VBox(
                    new Button("My button") { Expand = Expand.Horizontal, Action = ButtonAction },
                    new Toggle("My toggle") { Action = ToggleAction }
                )
                { }

            )
            { CloseCB = ClosaCall,KAny=DialogKey };
            


            dlg.Popup();

            dlg.Destroy();
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
