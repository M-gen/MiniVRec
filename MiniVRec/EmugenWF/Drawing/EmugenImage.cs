using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emugen.Image.Drawing
{
    public class EmugenImage
    {
        Color[] colors;
        int size;
        public int width;
        public int height;

        public EmugenImage(int w, int h)
        {
            size = w * h;
            this.width = w;
            this.height = h;
            this.colors = new Color[size];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var pos = (x + y * width);
                    colors[pos] = new Color(255, 255, 255, 255);
                }
            }
        }

        public EmugenImage(System.Drawing.Bitmap bitmap)
        {
            __Init__ByBitmap(bitmap);
        }

        public void __Init__ByBitmap(System.Drawing.Bitmap bitmap)
        {

            this.width = bitmap.Width;
            this.height = bitmap.Height;
            size = this.width * this.height;
            this.colors = new Color[this.width * this.height];

            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmap_data = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var stride = bitmap_data.Stride;
            var ptr = bitmap_data.Scan0;
            var pixels = new byte[bitmap_data.Stride * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, pixels.Length);
            bitmap.UnlockBits(bitmap_data);
            bitmap.Dispose();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var pos1 = (x + y * width);
                    var pos2 = (x + (height - y - 1) * width);
                    colors[pos1] = new Color(
                        pixels[pos2 * 4 + 2],
                        pixels[pos2 * 4 + 1],
                        pixels[pos2 * 4 + 0],
                        pixels[pos2 * 4 + 3]);
                }
            }

        }

        public EmugenImage(string filePath)
        {
            var image = System.Drawing.Image.FromFile(filePath);
            var bitmap = (System.Drawing.Bitmap)image;
            __Init__ByBitmap(bitmap);
        }

        public EmugenImage Filter(EmugenPlaneImage plane, UInt64 div)
        {
            var hw = plane.width / 2;
            var hh = plane.height / 2;

            var newImage = new EmugenImage(this.width, this.height);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    //var pos = (x + y * width);
                    //colors[pos].WriteUnityEngineColor(ref ucolors[pos]);
                    var col = new Color(0, 0, 0, 0);
                    for (var fx = 0; fx < plane.width; fx++)
                    {
                        for (var fy = 0; fy < plane.height; fy++)
                        {
                            //var fi = fx + fy * plane.width;
                            //Debug.Log($"DD {fx} {fy}");
                            var fcol = plane.At(fx, fy);
                            var tx = x + fx - hw;
                            var ty = y + fy - hh;
                            var colSrc = At(tx, ty);
                            //var col = new Color(  )
                            col.a += colSrc.a * (UInt64)fcol;
                            col.r += colSrc.r * (UInt64)fcol;
                            col.g += colSrc.g * (UInt64)fcol;
                            col.b += colSrc.b * (UInt64)fcol;
                        }
                    }
                    col.a = col.a / div; if (col.a > 255) col.a = 255;
                    col.r = col.r / div; if (col.r > 255) col.r = 255;
                    col.g = col.g / div; if (col.g > 255) col.g = 255;
                    col.b = col.b / div; if (col.b > 255) col.b = 255;
                    newImage.SetColor(x, y, col);
                }
            }
            return newImage;
        }

        public void DrawImage(EmugenImage src)
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var dstCol = At(x, y);
                    var srcCol = src.At(x, y);
                    dstCol.r = (srcCol.r * srcCol.a + dstCol.r * (255 - srcCol.a)) / 255;
                    dstCol.g = (srcCol.g * srcCol.a + dstCol.g * (255 - srcCol.a)) / 255;
                    dstCol.b = (srcCol.b * srcCol.a + dstCol.b * (255 - srcCol.a)) / 255;
                    dstCol.a += srcCol.a;

                    if (dstCol.a > 255) dstCol.a = 255;
                    if (dstCol.r > 255) dstCol.a = 255;
                    if (dstCol.g > 255) dstCol.a = 255;
                    if (dstCol.b > 255) dstCol.a = 255;
                }
            }
        }

        // RGBのみ変更する
        public void FillRGB(Color color)
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var col = At(x, y);
                    col.r = color.r;
                    col.g = color.g;
                    col.b = color.b;
                }
            }

        }

        public void SetColor(int x, int y, Color color)
        {
            if (x < 0) return;
            else if (width <= x) return;
            if (y < 0) return;
            else if (height <= y) return;

            var pos = x + y * width;
            colors[pos].a = color.a;
            colors[pos].r = color.r;
            colors[pos].g = color.g;
            colors[pos].b = color.b;
        }

        public Color At(int x, int y)
        {
            // 色の抜き取り
            if (x < 0) x = 0;
            else if (width <= x) x = width - 1;
            if (y < 0) y = 0;
            else if (height <= y) y = height - 1;

            return colors[x + y * width];
        }

        public System.Drawing.Bitmap ToBitmap()
        {
            var bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //var p = data.Scan0;

            int bytes = System.Math.Abs(data.Stride) * bmp.Height;
            byte[] p = new byte[bytes];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var col = At(x, y);
                    //col.r = color.r;
                    //col.g = color.g;
                    //col.b = color.b; rgba bgra
                    //p[x + y * data.Stride + 0] = col.b;
                    var ry = height - y - 1;
                    p[(x * 4 + ry * data.Stride)  + 0] = (byte)col.b;
                    p[(x *4  + ry * data.Stride)  + 1] = (byte)col.g;
                    p[(x *4 + ry * data.Stride) + 2] = (byte)col.r;
                    p[(x *4 + ry * data.Stride) + 3] = (byte)col.a;
                    //p[x + y * data.Stride + 0] = (byte)255;
                    //p[x + y * data.Stride + 3] = (byte)255;
                }
            }
            //System.Runtime.InteropServices.Marshal.Copy(data.Scan0, p, 0, bytes);
            //System.Runtime.InteropServices.Marshal.Copy(data.Scan0, p, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(p, 0, data.Scan0, bytes);
            bmp.UnlockBits(data);

            return bmp;
        }

        //public Texture2D CreateTextrue()
        //{
        //    var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        //    var ucolors = new UnityEngine.Color32[size];
        //    for (var x = 0; x < width; x++)
        //    {
        //        for (var y = 0; y < height; y++)
        //        {
        //            var pos = (x + y * width);
        //            colors[pos].WriteUnityEngineColor(ref ucolors[pos]);
        //        }
        //    }

        //    texture.SetPixels32(0, 0, width, height, ucolors);
        //    texture.Apply();
        //    return texture;
        //}
    }
}
