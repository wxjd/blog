using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace VerificationCode
{
    public class Twist
    {
        /// <summary>
        /// 正弦曲线Wave扭曲图片   
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">沿Y轴扭曲则选择为True</param>
        /// <param name="dMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns>扭曲后的位图</returns>
        public Bitmap TwistBitmap(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            int w = srcBmp.Width;
            int h = srcBmp.Height;
            System.Drawing.Bitmap destBmp = new Bitmap(w, h);
            using (System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp))
            {
                graph.Clear(Color.White);
            }
            double dBaseAxisLen = bXDir ? (double)h : (double)w;
            BitmapData destData = destBmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, srcBmp.PixelFormat);
            BitmapData srcData = srcBmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
             //调式一定注意PixelFormat是多少，别处都没有解释的。如果是24下面指针和跨度就3倍，要是32就是4倍。
            unsafe
            {
                byte* p = (byte*)srcData.Scan0;
                byte* p2 = (byte*)destData.Scan0;
                for (int j = 0; j < h; j++)
                {
                    for (int i = 0; i < w; i++)
                    {
                        double dx = 0;
                        dx = bXDir ? (Math.PI * (double)j * 2) / dBaseAxisLen : (Math.PI * (double)i * 2) / dBaseAxisLen;
                        dx += dPhase;
                        double dy = Math.Sin(dx);
                        // 取得当前点  
                        int nOldX = 0, nOldY = 0;
                        nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                        nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                        if ((nOldX >= 0 && nOldX < w) && (nOldY >= 0 && nOldY < h))
                        {
                            p2[(nOldY * destData.Stride) + (nOldX * 4)] = p[(j * srcData.Stride) + (i * 4)];
                            p2[(nOldY * destData.Stride) + (nOldX * 4) + 1] = p[(j * srcData.Stride) + (i * 4) + 1];
                            p2[(nOldY * destData.Stride) + (nOldX * 4) + 2] = p[(j * srcData.Stride) + (i * 4) + 2];
                        }
                    }
                }
            }
            destBmp.UnlockBits(destData);
            srcBmp.UnlockBits(srcData);
            if (srcBmp != null)
                srcBmp.Dispose();
            return destBmp;
        }
    }
}
