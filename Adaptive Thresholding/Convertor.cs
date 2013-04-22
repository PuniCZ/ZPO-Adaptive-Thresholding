using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;

namespace AdaptiveThresholding
{
    class Convertor
    {
        public static byte[,] ToGrayscaleArray(BitmapSource img)
        {
            int inputBytesPerPixel = ((img.Format.BitsPerPixel + 7) / 8);
            int inputStride = img.PixelWidth * inputBytesPerPixel;
            byte[] rgbValues = new byte[img.PixelHeight * img.PixelWidth * inputBytesPerPixel];

            img.CopyPixels(rgbValues, inputStride, 0);

            byte[,] grayscaleValues = new byte[img.PixelHeight, img.PixelWidth];
            for (int i = 0; i < img.PixelHeight; i++)
            {
                for (int j = 0; j < img.PixelWidth; j++)
                {
                    int addr = img.PixelWidth * i + j;
                    grayscaleValues[i, j] = Convert.ToByte(
                        .229f * rgbValues[addr * inputBytesPerPixel] +
                        .587f * rgbValues[addr * inputBytesPerPixel + 1] +
                        .114f * rgbValues[addr * inputBytesPerPixel + 2]
                    );
                }
            }

            return grayscaleValues;
        }

        public static BitmapSource ToBitmapSource(byte[,] data, BitmapSource originalImage)
        {
            return ToBitmapSource(data, originalImage.PixelWidth, originalImage.PixelHeight);
        }

        public static BitmapSource ToBitmapSource(byte[,] data, int sizeX, int sizeY)
        {
            WriteableBitmap output = new WriteableBitmap(sizeX, sizeY, 96, 96, PixelFormats.Gray8, null);
            int bytesPerPixel = (output.Format.BitsPerPixel + 7) / 8;
            int stride = output.PixelWidth * bytesPerPixel;
            output.WritePixels(new Int32Rect(0, 0, sizeX, sizeY), data, stride, 0);
            return output;
        }
    }
}
