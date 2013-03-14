using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace AdaptiveThresholding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            IntergalImageThresholding th = new IntergalImageThresholding();
            th.Tolerance = Int32.Parse(textBox2.Text);
            th.WindowSize = Int32.Parse(textBox1.Text); ;
            //Bitmap bmp = new Bitmap(100, 100,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //Bitmap bmp = new Bitmap("test.bmp");

            BitmapImage bmp = new BitmapImage(new Uri("test.bmp", UriKind.Relative));

            var start = System.DateTime.Now;

            var img = th.Process(bmp);

            Console.WriteLine(System.DateTime.Now - start);

            image1.Source = img;
        }
    }
}
