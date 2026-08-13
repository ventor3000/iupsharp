using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IupSharp
{
    public class VBoxHBox:ContainerControl
    {
        internal VBoxHBox(nint handle) : base(handle)
        {
        }
    }

    public class VBox:VBoxHBox
    {
        public VBox(params Control[] children):base(IupNative.IupVboxv(IntPtr.Zero))
        {
            foreach(var w in children)
                Append(w);
        }
    }

    public class HBox : VBoxHBox
    {
        public HBox(params Control[] children) : base(IupNative.IupHboxv(IntPtr.Zero))
        {
            foreach (var w in children)
                Append(w);
        }
    }
}
