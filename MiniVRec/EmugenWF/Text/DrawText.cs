using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Emugen.Image.Drawing;

namespace YoutubleComment.Text
{
    public class DrawText
    {

        private class Layer
        {
            public EmugenPlaneImage plane;
            public EmugenImage image;
            public double circleSize = 0;
            public System.Drawing.Color color;
        }

        static public void Do(string text, Font efont, Graphics graphics, PointF position)
        {
            var frameSize = 0.0;
            var frameSizeMax = 0.0;
            var rfontFrames = new List<Font.FontFrame>();
            foreach (var frame in efont.fontFrames)
            {
                frameSizeMax += frame.size;
                rfontFrames.Insert(0, frame);
            }
            frameSize = frameSizeMax;

            // Todo : フォントフレームの幅分だけマージンの対応が必要
            var margin = (int)System.Math.Floor(frameSize + 3);

            var font = CreateFont(efont.fontPath, efont.fontSize);
            var emFontFize = (float)font.Height * font.FontFamily.GetEmHeight(font.Style) / font.FontFamily.GetLineSpacing(font.Style);
            var stringFormat = new System.Drawing.StringFormat();

            // 描画
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddString(text, font.FontFamily, (int)font.Style, emFontFize,
                new System.Drawing.PointF((float)(position.X + margin), (float)(position.Y + margin)),
                stringFormat);

            {
                var g = graphics;

                var brush = default(System.Drawing.SolidBrush);
                brush = new System.Drawing.SolidBrush(efont.bodyColor);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                frameSize = frameSizeMax;

                foreach (var frame in rfontFrames)
                {
                    var pen = new System.Drawing.Pen(frame.color, (float)frameSize);
                    g.DrawPath(pen, path);

                    frameSize -= frame.size;
                }

                g.FillPath(brush, path);
            }
        }



        static public Bitmap CreateBitmap(string text, Font efont)
        {
            // Todo このままだと 重いので、この情報を記録しておく必要がある
            var frameSize = 0.0;
            var frameSizeMax = 0.0;
            var rfontFrames = new List<Font.FontFrame>();
            var layers = new List<Layer>();

            var fontFamily = new Emugen.Image.Drawing.FontFamily(efont.fontPath);
            var font = CreateFont(efont.fontPath, efont.fontSize);
            var fontSize = efont.fontSize;

            // layerがボディ部分が足りてない
            {
                var layer = new Layer();
                layer.circleSize = frameSizeMax;
                layer.color = efont.bodyColor;
                //layer.plane = null;
                layer.plane = new EmugenPlaneImage((int)(frameSizeMax * 2 + 1), (int)(frameSizeMax * 2 + 1));
                //layer.plane.DrawCircle(frameSizeMax, frameSizeMax, frameSizeMax, 1);

                layers.Add(layer);
            }

                //var i = 0;
            foreach (var frame in efont.fontFrames)
            {
                frameSizeMax += frame.size;
                rfontFrames.Insert(0, frame);

                var layer = new Layer();
                layer.circleSize = frameSizeMax;
                layer.color = frame.color;
                layer.plane = null;

                layer.plane = new EmugenPlaneImage((int)(frameSizeMax * 2 + 1), (int)(frameSizeMax * 2 + 1));
                layer.plane.DrawCircle(frameSizeMax, frameSizeMax, frameSizeMax, 1);

                layers.Add(layer);

                //if(i==0)
                //{
                //    var image = new EmugenImage(fontFamily.CreateBitmap(text, fontSize, layer.color, (int)frameSize + 1, out info));
                //}
                //i++;
            }
            frameSize = frameSizeMax;

            {
                var i = 0;
                foreach (var layer in layers)
                {
                    Emugen.Image.Drawing.FontFamily.FontStringInfo info;
                    layer.image = new EmugenImage(fontFamily.CreateBitmap(text, fontSize, new Emugen.Image.Drawing.Color(0, 0, 0, 255), (int)frameSize + 1, out info));

                    
                    if (i!=0)
                    {
                        layer.image = layer.image.Filter(layer.plane, 255);
                    }


                    i++;
                }
            }

            {
                var i = 0;
                foreach (var layer in layers)
                {
                    layer.image.FillRGB(new Emugen.Image.Drawing.Color(layer.color));
                    i++;
                }
            }

            for( var i=(layers.Count-2); i>=0; i--)
            {
                layers[(layers.Count-1)].image.DrawImage(layers[i].image);
            }

            {
                var image = layers[layers.Count-1].image;
                var bmp = image.ToBitmap();
                return bmp;
            }
        }

        static private System.Drawing.Font CreateFont(string fontPath, double fontSize)
        {
            //var fontPath = "data/font/migmix-2p-bold.ttf";
            var pfc = new System.Drawing.Text.PrivateFontCollection();
            var id = pfc.Families.Length;
            pfc.AddFontFile(fontPath);
            var fontFamily = pfc.Families[id];
            var fontStyle = System.Drawing.FontStyle.Regular;

            if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Regular))
            {
                fontStyle = System.Drawing.FontStyle.Regular;
            }
            else if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Bold))
            {
                fontStyle = System.Drawing.FontStyle.Bold;
            }
            else if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Italic))
            {
                fontStyle = System.Drawing.FontStyle.Italic;
            }
            else if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Strikeout))
            {
                fontStyle = System.Drawing.FontStyle.Strikeout;
            }
            else if (fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Underline))
            {
                fontStyle = System.Drawing.FontStyle.Strikeout;
            }

            var font = new System.Drawing.Font(fontFamily, (float)fontSize, fontStyle);
            return font;
        }
    }
}
