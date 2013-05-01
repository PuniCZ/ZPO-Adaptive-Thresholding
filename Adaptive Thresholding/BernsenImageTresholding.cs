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
    class BernsenImageTresholding: IThresholding
    {
        public int WindowSize { get; set; }

        public int Tolerance { get; set; }
        

        public override string  ToString()
        {
            return "Bernsen Image";
        }



        public byte[,] Process(byte[,] grayscaleValues, int width, int height)
        {
            byte[,] outValues = new byte[height, width];
            List<byte> vector = new List<byte>();

            int localContrast;
            int midGray;

            // Perform thresholding
            for (int y = 0; y < height; y++)
            {
                int y1 = Math.Max(y - WindowSize, 0);
                int y2 = Math.Min(y + WindowSize, height - 1);
                
                for (int x = 0; x < width; x++)
                {
                    int x1 = Math.Max(x - WindowSize, 0);
                    int x2 = Math.Min(x + WindowSize, width - 1);

                    for (int n = y1; n < y2; n++)
                    {
                        for (int m = x1; m < x2; m++)                       
                        {
                            vector.Add(grayscaleValues[n, m]);
                        }
                    }

                    localContrast = vector.Max() - vector.Min();
                    midGray = (vector.Max() + vector.Min()) / 2;

                    if (localContrast < 15)
                    {
                        if (midGray <= (128 *(100 - Tolerance) / 100))
                            outValues[y, x] = 255;
                        else
                            outValues[y, x] = 0;
                    }
                    else
                    {
                        if (grayscaleValues[y, x] >= (midGray * (100 - Tolerance) / 100))
                            outValues[y, x] = 255;
                        else
                            outValues[y, x] = 0;
                    }
                    vector.Clear();
                }
            }
            return outValues;
        }  
    }
}
