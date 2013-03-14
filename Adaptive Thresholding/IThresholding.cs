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
        WriteableBitmap Process(BitmapSource img);

        int WindowSize { get; set; }
    }
}
