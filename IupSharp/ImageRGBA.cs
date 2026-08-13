using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IupSharp
{
    public class ImageRGBA:IupObject
    {
        public ImageRGBA(int width,int height, byte[] pixels):base(IupNative.IupImageRGBA(width, height, pixels))
        {
            
        }

        

        public string AutoScale => GetAttribute("AUTOSCALE");
        public int BPP => int.Parse(GetAttribute("BPP"), CultureInfo.InvariantCulture);
        public int Channels => int.Parse(GetAttribute("CHANNELS"), CultureInfo.InvariantCulture);
    }
}
