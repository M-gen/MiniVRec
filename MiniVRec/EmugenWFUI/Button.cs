using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;

namespace VoiceRecoder.EmugenWFUI
{
    public class UIConfig
    {
        public static readonly string FontPath = "data/font/mplus-TESTFLIGHT-063a/mplus-1c-medium.ttf";
    }

    public class Button
    {
        PictureBox pictureBox;
        Bitmap bitmap;
        Point offsetTextBitmap;
        bool isMoseOn = false;
        double fontSize;

        public Action Click;

        private string text;
        public string Text
        {
            set {
                text = value;
                bitmap.Dispose();

                bitmap = YoutubleComment.Text.DrawText.CreateBitmap(
                    text,
                    new YoutubleComment.Text.Font(UIConfig.FontPath, fontSize, Color.FromArgb(255, 255, 255, 255),
                    null, 0)
                );
                offsetTextBitmap = new Point((pictureBox.Size.Width - bitmap.Width) / 2, (pictureBox.Size.Height - bitmap.Height) / 2);

                pictureBox.Refresh();

            }
            get { return text; }
        }
        
        public Button( Form form, int x, int y, int w, int h, string text, double fontSize )
        {
            this.text = text;
            this.fontSize = fontSize;

            pictureBox = new PictureBox();
            pictureBox.Location = new System.Drawing.Point(x,y);
            pictureBox.Size = new System.Drawing.Size(w, h);
            pictureBox.Paint += PictureBox_Paint;
            form.Controls.Add(pictureBox);

            bitmap = YoutubleComment.Text.DrawText.CreateBitmap(
                text,
                new YoutubleComment.Text.Font(UIConfig.FontPath, fontSize, Color.FromArgb(255, 255, 255, 255),
                null, 0)
                //new YoutubleComment.Text.Font.FontFrame[] { new YoutubleComment.Text.Font.FontFrame(1, Color.FromArgb(255, 0, 175, 95)), new YoutubleComment.Text.Font.FontFrame(3, Color.FromArgb(255, 255, 255, 255)) }, 0)
            );

            offsetTextBitmap = new Point((pictureBox.Size.Width- bitmap.Width)/2, (pictureBox.Size.Height - bitmap.Height) / 2);

            pictureBox.MouseHover += PictureBox_MouseHover;
            pictureBox.MouseLeave += PictureBox_MouseLeave;
            pictureBox.MouseClick += PictureBox_MouseClick;
        }

        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if(Click!=null) Click();
        }

        private void PictureBox_MouseHover(object sender, EventArgs e)
        {
            isMoseOn = true;
            pictureBox.Refresh();
        }
        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            isMoseOn = false;
            pictureBox.Refresh();
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            if (isMoseOn)
            {
                var c = 30;
                g.FillRectangle(new SolidBrush(Color.FromArgb(91-c, 168-c, 255-c)), new RectangleF(0, 0, pictureBox.Size.Width, pictureBox.Size.Height));
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(91, 168, 255)), new RectangleF(0, 0, pictureBox.Size.Width, pictureBox.Size.Height));
            }
            g.DrawImage(bitmap, offsetTextBitmap);
        }
    }
}
