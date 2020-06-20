using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emugen.Image.Drawing
{

    public class FontFamily
    {
        public class FontStringInfo
        {
            public int offsetX;
            public int offsetY;
        }

        static Dictionary<string, FontFamily> ffs = new Dictionary<string, FontFamily>();
        static System.Drawing.Text.PrivateFontCollection pfc;

        private int id;
        private System.Drawing.FontFamily fontFamily;
        private System.Drawing.FontStyle  fontStyle;

        public FontFamily(string fontPath )
        {
            if (ffs.ContainsKey(fontPath))
            {
                var ff = ffs[fontPath];
                this.id = ff.id;
                this.fontFamily = ff.fontFamily;
                this.fontStyle = ff.fontStyle;
            }
            else
            {
                if (pfc == null) pfc = new System.Drawing.Text.PrivateFontCollection();
                id = pfc.Families.Length;

                pfc.AddFontFile(fontPath);
                fontFamily = pfc.Families[id];

                // 自動でフォントスタイルを設定する
                if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                {
                    this.fontStyle = System.Drawing.FontStyle.Regular;
                }
                else if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Bold))
                {
                    this.fontStyle = System.Drawing.FontStyle.Bold;
                }
                else if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Italic))
                {
                    this.fontStyle = System.Drawing.FontStyle.Italic;
                }
                else if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Strikeout))
                {
                    this.fontStyle = System.Drawing.FontStyle.Strikeout;
                }
                else if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Underline))
                {
                    this.fontStyle = System.Drawing.FontStyle.Strikeout;
                }

                ffs.Add(fontPath, this);
            }
        }

        static public void LogFontNames()
        {
            var ifc = new System.Drawing.Text.InstalledFontCollection();
            var ffs = ifc.Families;

            var text = "";
            foreach (var ff in ffs)
            {
                if (ff.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                {
                    if (text != "") text += "\n";
                    text += ff.Name;
                }
            }
        }

        public System.Drawing.Bitmap CreateBitmap(string text, double fontSize, Color color, int bitmap_bounds_margin, out FontStringInfo info)
        {
            var bitmap = default(System.Drawing.Bitmap);
            info = new FontStringInfo();

            var font = default(System.Drawing.Font);
            var emFontFize = default(float);
            var stringFormat = default(System.Drawing.StringFormat);
            var path = default(System.Drawing.Drawing2D.GraphicsPath);
            var path2 = default(System.Drawing.Drawing2D.GraphicsPath);
            var bounds = default(System.Drawing.RectangleF);

            //var fontStyle = System.Drawing.FontStyle.Bold;
            //var fontName = "Rounded-X Mgen+ 1cp bold";
            font = new System.Drawing.Font(fontFamily, (float)fontSize, fontStyle);
            //try {}
            ////try { font = new System.Drawing.Font(fontName, (float)fontSize, System.Drawing.FontStyle.Bold); }
            //catch (Exception exp) {
            //    Log.WriteLine($"CreateBitmap 1-1\n{exp}");
            //    try
            //    {
            //        font = new System.Drawing.Font("ＭＳ ゴシック", (float)fontSize, System.Drawing.FontStyle.Regular);
            //        Log.WriteLine($"CreateBitmap 1-1 Success\n代替えフォントに\"ＭＳ ゴシック\"を利用");
            //    }
            //    catch
            //    {
            //        Log.WriteLine($"CreateBitmap 1-1-1\n{exp}");
            //    }
            //}

            //EmugenUnityDll.Common.Log.WriteLine($"CreateBitmap UseFontName {font.Name}");

            emFontFize = (float)font.Height * font.FontFamily.GetEmHeight(font.Style) / font.FontFamily.GetLineSpacing(font.Style);
            stringFormat = new System.Drawing.StringFormat();
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddString(text, font.FontFamily, (int)fontStyle, emFontFize, new System.Drawing.PointF(0, 0), stringFormat);
            bounds = path.GetBounds();
            path2 = new System.Drawing.Drawing2D.GraphicsPath();
            path2.AddString(text, fontFamily, (int)fontStyle, emFontFize,
                new System.Drawing.PointF((float)(-bounds.Left + bitmap_bounds_margin), (float)(-bounds.Top + bitmap_bounds_margin)),
                stringFormat);
            bitmap = new System.Drawing.Bitmap(
                (int)(bounds.Width + bitmap_bounds_margin * 2 + 2),
                (int)(bounds.Height + bitmap_bounds_margin * 2 + 2), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            info.offsetX = (int)bounds.Left - bitmap_bounds_margin;
            info.offsetY = (int)bounds.Top - bitmap_bounds_margin;



            using (var g = System.Drawing.Graphics.FromImage(bitmap))
            {
                var brush = default(System.Drawing.SolidBrush);
                brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.FillPath(brush, path2);
                //try {  }
                //catch (Exception exp) { Log.WriteLine($"CreateBitmap 1-11\n{exp}"); }

                //try {  }
                //catch (Exception exp) { Log.WriteLine($"CreateBitmap 1-12\n{exp}"); }

                //try { }
                //catch (Exception exp) { Log.WriteLine($"CreateBitmap 1-13\n{exp}"); }

            }

            return bitmap;
        }
    }
}
