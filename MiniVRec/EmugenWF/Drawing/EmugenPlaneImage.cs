using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emugen.Image.Drawing
{

    public class EmugenPlaneImage
    {
        int[] datas;
        int size;
        public int width;
        public int height;

        public EmugenPlaneImage(int w, int h)
        {
            size = w * h;
            this.width = w;
            this.height = h;
            this.datas = new int[size];
        }

        public void DrawCircle(double x, double y, double range, double hosei)
        {
            for (var x2 = 0; x2 < width; x2++)
            {
                for (var y2 = 0; y2 < height; y2++)
                {
                    var i = (int)(x2 + y2 * width);
                    var r = System.Math.Sqrt((float)((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y)));
                    if (r < range)
                    {
                        datas[i] = 255;
                        //Debug.Log( $"{x2} {y2} 255" );
                    }
                    else if (r < (range + hosei))
                    {
                        var a = (int)((1.0 - (r - range) / hosei) * 255);
                        datas[i] = a;
                        //Debug.Log($"{x2} {y2} {a}");
                    }
                }
            }
        }

        public void Normalization()
        {
            var sum = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = (int)(x + y * width);
                    sum += datas[i];
                }
            }
            var scale = 255.0 / sum;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = (int)(x + y * width);
                    datas[i] = (int)(datas[i] * scale);
                }
            }


        }

        public int At(int x, int y)
        {
            return datas[x + y * width];
        }

        //public void DebugLog()
        //{
        //    for (var y = 0; y < height; y++)
        //    {
        //        var line = "";
        //        for (var x = 0; x < width; x++)
        //        {
        //            var i = (int)(x + y * width);
        //            line += $"{datas[i]} ";
        //        }
        //        Debug.Log(line);
        //    }
        //}

    }
}
