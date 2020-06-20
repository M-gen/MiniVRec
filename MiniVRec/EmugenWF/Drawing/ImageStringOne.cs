using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emugen.Image.Drawing
{

    public class ImageStringOne
    {
        FontFamily fontFamily;
        public FontFamily.FontStringInfo info;
        public EmugenImage image;

        public ImageStringOne(FontFamily fontFamily, string text, double fontSize, int bitmap_bounds_margin, Color bodyColor, double frameSize, Color frameColor)
        {
            this.fontFamily = fontFamily;
            var plane = new EmugenPlaneImage((int)(frameSize * 2 + 1), (int)(frameSize * 2 + 1));
            plane.DrawCircle(frameSize, frameSize, frameSize, 1);

            var image = new EmugenImage(fontFamily.CreateBitmap(text, fontSize, new Color(255, 0, 0, 255), (int)frameSize + 1, out info));
            var image2 = image.Filter(plane, 255);
            image.FillRGB(bodyColor);
            image2.FillRGB(frameColor);
            image2.DrawImage(image);

            this.image = image2;
        }

        public ImageStringOne(FontFamily fontFamily, string text, double fontSize, int bitmap_bounds_margin, Color bodyColor, double frameSize, Color frameColor, double frame2Size, Color frame2Color)
        {
            var plane = new EmugenPlaneImage((int)(frameSize * 2 + 1), (int)(frameSize * 2 + 1));
            plane.DrawCircle(frameSize, frameSize, frameSize, 1);

            var plane2 = new EmugenPlaneImage((int)(frame2Size * 2 + 1), (int)(frame2Size * 2 + 1));
            plane2.DrawCircle(frame2Size, frame2Size, frame2Size, 1);

            var image = new EmugenImage(fontFamily.CreateBitmap(text, fontSize, new Color(255, 0, 0, 255), (int)(frameSize + frame2Size + 1), out info));
            var image2 = image.Filter(plane, 255);
            var image3 = image2.Filter(plane2, 255);
            image.FillRGB(bodyColor);
            image2.FillRGB(frameColor);
            image3.FillRGB(frame2Color);
            image3.DrawImage(image2);
            image3.DrawImage(image);

            this.image = image3;
        }
    }
}
