using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace AdaptiveThresholding
{
    class IntergalImageThresholding: IThresholding
    {
        public int WindowSize { get; set; }

        public int Tolerance { get; set; }
        

        public override string  ToString()
        {
            return "Integral Image";
        }



        public WriteableBitmap Process(BitmapSource img)
        {
            WriteableBitmap output = new WriteableBitmap(img.PixelWidth, img.PixelHeight, 96, 96, PixelFormats.Gray8, null);

            int inputBytesPerPixel = ((img.Format.BitsPerPixel + 7) / 8);
            int inputStride = img.PixelWidth * inputBytesPerPixel;
            byte[] rgbValues = new byte[img.PixelHeight * img.PixelWidth * inputBytesPerPixel];

            img.CopyPixels(rgbValues, inputStride, 0);

            byte[,] grayscaleValues = new byte[img.PixelHeight, img.PixelWidth];
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    int addr = img.PixelWidth * i + j;
                    grayscaleValues[i, j] = Convert.ToByte(
                        .229f * rgbValues[addr * inputBytesPerPixel] + 
                        .587f * rgbValues[addr * inputBytesPerPixel + 1] + 
                        .114f * rgbValues[addr * inputBytesPerPixel + 2]
                    );
                }
            }

            // Compute integral image
            int[,] integralImage = new int[img.PixelHeight, img.PixelWidth];
            for (int y = 0; y < img.Height; y++)
            {
                int sum = 0;
                for (int x = 0; x < img.Width; x++)
                {
                    sum += grayscaleValues[y, x];
                    if (y == 0)
                        integralImage[y, x] = sum;
                    else
                        integralImage[y, x] = integralImage[y - 1, x] + sum;
                }
            }

            byte[,] outValues = new byte[img.PixelHeight, img.PixelWidth];
            // Perform thresholding
            for (int y = 0; y < img.Height; y++)
            {
                int y1 = Math.Max(y - WindowSize, 0);
                int y2 = Math.Min(y + WindowSize, img.PixelHeight - 1);
                for (int x = 0; x < img.Width; x++)
                {
                    int x1 = Math.Max(x - WindowSize, 0);
                    int x2 = Math.Min(x + WindowSize, img.PixelWidth - 1);
                    int count = (x2 - x1) * (y2 - y1);
                    int sum = integralImage[y2, x2] - integralImage[y2, Math.Max(x1 - 1, 0)] - integralImage[Math.Max(y1 - 1, 0), x2] + integralImage[Math.Max(y1 - 1, 0), Math.Max(x1 - 1, 0)];
                    
                    if (grayscaleValues[y, x] * count <= (sum * (100 - Tolerance) / 100))
                        outValues[y, x] = 0;
                    else
                        outValues[y, x] = 255;

                }
            }

            int bytesPerPixel = (output.Format.BitsPerPixel + 7) / 8;
            int stride = output.PixelWidth * bytesPerPixel;
            output.WritePixels(new Int32Rect(0, 0, img.PixelWidth, img.PixelHeight), outValues, stride, 0);

            return output;
        }

        
    }
}
