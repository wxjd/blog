using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace VerificationCode
{
    public class VerifyCode
    {

        public string MakeValidateCode()
        {
            char[] s = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
            string num = "";
            Random r = new Random();
            for (int i = 0; i < 4; i++)
            {
                num += s[r.Next(0, s.Length)].ToString();
            }
            return num;
        }

        public byte[] CreateCheckCodeImage(string checkCode)
        {
            Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Brown, Color.DarkCyan, Color.Purple };
            int len = colors.Length;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling(checkCode.Length * 14.5d), 25);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                Font font = new System.Drawing.Font("Arial", 12, FontStyle.Bold);
                System.Drawing.Brush brush = new SolidBrush(colors[random.Next(0, len)]);
                SizeF size = g.MeasureString(checkCode, font);
                g.DrawString(checkCode, font, brush, (image.Width - size.Width) / 2, (image.Height - size.Height) / 2);

                Twist tw = new Twist();
                //正弦扭曲 沿X轴
                image = tw.TwistBitmap(image, false, 4, Math.PI);
                //正弦扭曲 沿Y轴
                image = tw.TwistBitmap(image, true, 2, Math.PI / 2);

                g.Dispose();

                g = Graphics.FromImage(image);
                //画图片的背景噪音线
                for (int i = 0; i < 2; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}