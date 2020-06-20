using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace YoutubleComment.Text
{
    public class Font
    {
        public class FontFrame
        {
            public double size;
            public Color color;

            public FontFrame(double size, Color color)
            {
                this.size = size;
                this.color = color;
            }
        }

        public string fontPath;
        public double fontSize;
        public Color bodyColor;
        public int lineTextMargin;
        public List<FontFrame> fontFrames = new List<FontFrame>();

        public Font(string fontPath, double fontSize, Color bodyColor, FontFrame[] fontFrames = null, int lineTextMargin = 0)
        {
            this.fontPath = fontPath;
            this.fontSize = fontSize;
            this.bodyColor = bodyColor;
            this.lineTextMargin = lineTextMargin;

            if (fontFrames != null)
            {
                foreach (var i in fontFrames) this.fontFrames.Add(i);
            }
        }

    }
}
