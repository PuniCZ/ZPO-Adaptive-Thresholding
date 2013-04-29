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
    class FastMedianImageTresholding: IThresholding
    {
        public int WindowSize { get; set; }

        public int Tolerance { get; set; }
        

        public override string  ToString()
        {
            return "Fast Median Image";
        }



        public byte[,] Process(byte[,] grayscaleValues, int width, int height)
        {
            byte[,] outValues = new byte[height, width];
            List<byte> vector = new List<byte>();
            int position = 0;

            // Perform thresholding
            for (int y = 0; y < height; y++)
            {
                int y1 = Math.Max(y - WindowSize, 0);
                int y2 = Math.Min(y + WindowSize, height - 1);
                int WindowSize2d = WindowSize * WindowSize;
                int clearValue = WindowSize2d / 2;
                int clear = clearValue;
                
                for (int x = 0; x < width; x++)
                {
                    int x1 = Math.Max(x - WindowSize, 0);
                    int x2 = Math.Min(x + WindowSize, width - 1);

                    if (clear == clearValue)
                    {
                        vector.Clear();
                        for (int n = y1; n < y2; n++)
                        {
                            for (int m = x1; m < x2; m++)
                            {
                                vector.Add(grayscaleValues[n, m]);
                            }
                        }

                        vector.Sort();
                        position = (byte)(vector.Count / 2);
                        clear = 0;
                    }

                    if (grayscaleValues[y, x] <= (vector[position] * (100 - Tolerance) / 100))
                        outValues[y, x] = 0;
                    else
                        outValues[y, x] = 255;

                    clear++;
                }
            }
            return outValues;
        }  
    }
}

