using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace AdaptiveThresholding
{
    class GrayscaleConvertor
    {
        public static Bitmap ToGrayscale(Bitmap img)
        {
            Bitmap output = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            Graphics g = Graphics.FromImage(output);

            ColorMatrix cMat = new ColorMatrix(
                new float[][] 
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            ImageAttributes attrib = new ImageAttributes();
            attrib.SetColorMatrix(cMat);

            g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attrib);

            g.Dispose();

            return output;

        }
    }
}
