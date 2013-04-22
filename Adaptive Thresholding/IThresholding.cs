using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace AdaptiveThresholding
{
    interface IThresholding
    {
        string ToString();
        int WindowSize { get; set; }
        int Tolerance { get; set; }

        byte[,] Process(byte[,] img, int width, int height);

    }
}
