using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emugen.Image.Drawing
{
    public class Color
    {
        public UInt64 r;
        public UInt64 g;
        public UInt64 b;
        public UInt64 a;

        public Color(int r, int g, int b, int a)
        {
            this.r = (UInt64)r;
            this.g = (UInt64)g;
            this.b = (UInt64)b;
            this.a = (UInt64)a;
        }

        public Color( System.Drawing.Color color )
        {
            this.r = (UInt64)color.R;
            this.g = (UInt64)color.G;
            this.b = (UInt64)color.B;
            this.a = (UInt64)color.A;

        }

    }

}
