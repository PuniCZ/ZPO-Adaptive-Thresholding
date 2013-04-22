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



        public byte[,] Process(byte[,] grayscaleValues, int width, int height)
        {
            // Compute integral image
            int[,] integralImage = new int[height, width];
            for (int y = 0; y < height; y++)
            {
                int sum = 0;
                for (int x = 0; x < width; x++)
                {
                    sum += grayscaleValues[y, x];
                    if (y == 0)
                        integralImage[y, x] = sum;
                    else
                        integralImage[y, x] = integralImage[y - 1, x] + sum;
                }
            }

            byte[,] outValues = new byte[height, width];
            // Perform thresholding
            for (int y = 0; y < height; y++)
            {
                int y1 = Math.Max(y - WindowSize, 0);
                int y2 = Math.Min(y + WindowSize, height - 1);
                for (int x = 0; x < width; x++)
                {
                    int x1 = Math.Max(x - WindowSize, 0);
                    int x2 = Math.Min(x + WindowSize, width - 1);
                    int count = (x2 - x1) * (y2 - y1);
                    int sum = integralImage[y2, x2] - integralImage[y2, Math.Max(x1 - 1, 0)] - integralImage[Math.Max(y1 - 1, 0), x2] + integralImage[Math.Max(y1 - 1, 0), Math.Max(x1 - 1, 0)];
                    
                    if (grayscaleValues[y, x] * count <= (sum * (100 - Tolerance) / 100))
                        outValues[y, x] = 0;
                    else
                        outValues[y, x] = 255;

                }
            }
            return outValues;
        }

        
    }
}
