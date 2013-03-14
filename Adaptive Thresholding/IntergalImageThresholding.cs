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
        

        public string ToString()
        {
            return "Integral Image";
        }



        public WriteableBitmap Process(Bitmap img)
        {
            WriteableBitmap output = new WriteableBitmap(img.Width, img.Height, 96, 96, PixelFormats.Gray8, null);

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
            System.Drawing.Imaging.BitmapData bmpData = img.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, img.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int bytes  = Math.Abs(bmpData.Stride) * img.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            // Unlock the bits.
            img.UnlockBits(bmpData);


            byte[,] grayscaleValues = new byte[img.Height, img.Width];
            // Conver to grayscale (.299 * R + .587 * G + .114 * B)        
            //for (int i = 0; i < (img.Height * img.Width); i++)
            //{
            //    grayscaleValues[i / img.Width, i % img.Width] = Convert.ToByte(.229f * rgbValues[i * 3] + .587f * rgbValues[i * 3 + 1] + .114f * rgbValues[i * 3 + 2]);
            //}

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    int addr = (img.Width + 1) * i + j;
                    grayscaleValues[i, j] = Convert.ToByte(.229f * rgbValues[addr * 3] + .587f * rgbValues[addr * 3 + 1] + .114f * rgbValues[addr * 3 + 2]);
                }
            }


            // Compute integral image
            int[,] integralImage = new int[img.Height, img.Width];
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


            byte[,] outValues = new byte[img.Height, img.Width];
            // Perform thresholding
            for (int y = 0; y < img.Height; y++)
            {
                int y1 = Math.Max(y - WindowSize, 0);
                int y2 = Math.Min(y + WindowSize, img.Height - 1);
                for (int x = 0; x < img.Width; x++)
                {
                    int x1 = Math.Max(x - WindowSize, 0);
                    int x2 = Math.Min(x + WindowSize, img.Width - 1);
                    int count = (x2 - x1) * (y2 - y1);
                    int sum = integralImage[y2, x2] - integralImage[y2, Math.Max(x1 - 1, 0)] - integralImage[Math.Max(y1 - 1, 0), x2] + integralImage[Math.Max(y1 - 1, 0), Math.Max(x1 - 1, 0)];
                    
                    if (grayscaleValues[y, x] * count <= (sum * (100 - Tolerance) / 100))
                        outValues[y, x] = 0;
                    else
                        outValues[y, x] = 255;

                }
            }

            //int bytesPerPixel = (output.Format.BitsPerPixel + 7) / 8;
            //int stride = output.PixelWidth * bytesPerPixel;

            //output.WritePixels(new Int32Rect(0, 0, output.PixelWidth-1, output.PixelHeight-1), outValues, stride, 0);

            // Copy the RGB values back to the bitmap
            //System.Drawing.Imaging.BitmapData outData = output.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, output.PixelFormat);
            //ptr = outData.Scan0;
            //System.Runtime.InteropServices.Marshal.Copy(outValues, 0, ptr, outValues.Length);
            //output.UnlockBits(outData);

            int bytesPerPixel = (output.Format.BitsPerPixel + 7) / 8;
            int stride = output.PixelWidth * bytesPerPixel;
            output.WritePixels(new Int32Rect(0, 0, img.Width, img.Height), outValues, stride, 0);


            return output;
        }

        
    }
}
