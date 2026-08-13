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
                    new Button("My button") { Expand = Expand.Horizontal,Action=ButtonAction },
                    new Toggle("My toggle") { Action = ToggleAction }
                )
                { }
                
            );
            


            dlg.Popup();

            dlg.Destroy();
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
