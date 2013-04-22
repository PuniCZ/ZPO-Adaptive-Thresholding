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
using System.Threading;

namespace AdaptiveThresholding
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        CamDevice[] camDevices;
        CameraPreviewWindow camPreviewWindow;
        CamDevice camDevice;
        string fileName;

        BitmapSource srcImage;

        public MainWindow()
        {
            InitializeComponent();


            camPreviewWindow = new CameraPreviewWindow();

            //dev = DeviceManager.GetAllDevices()[0];

            //dev.ShowWindow(wi);
            
            

        }


        private void Process()
        {            
            IThresholding th = new IntergalImageThresholding();
            th.Tolerance = Int32.Parse(textBox2.Text);
            th.WindowSize = Int32.Parse(textBox1.Text);

            byte[,] srcData = Convertor.ToGrayscaleArray(srcImage);

            var start = System.DateTime.Now;

            var outData = th.Process(srcData, srcImage.PixelWidth, srcImage.PixelHeight);

            Console.WriteLine((System.DateTime.Now - start).Milliseconds);


            image1.Source = Convertor.ToBitmapSource(outData, srcImage);
        }
        
        private void button1_Click(object sender, RoutedEventArgs e)
        {

            Process();
            
        }

        private void srcSelCamera_Checked(object sender, RoutedEventArgs e)
        {
            camDevices = DeviceManager.GetAllDevices();
            if (camDevices.Length == 0)
            {
                MessageBox.Show("Nelze přepnout do režimu kamery, protože k tomuto počítači nejsou žádné připojeny.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                srcSelCamera.IsChecked = false;
                srcSelFile.IsChecked = true;
                return;
            }
            srcCamList.ItemsSource = camDevices;
            srcCamList.IsEnabled = true;
            srcFileName.IsEnabled = false;
            srcFileOpen.IsEnabled = false;
            camProcess.IsEnabled = true;
        }

        private void srcSelFile_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            srcCamList.IsEnabled = false;
            camPreviewWindow.Hide();
            srcFileName.IsEnabled = true;
            srcFileOpen.IsEnabled = true;
            camProcess.IsEnabled = false;
        }

        private void srcCamList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (camDevice != null)
                camDevice.Stop();
            
            camDevice = (CamDevice)srcCamList.SelectedItem;                
            camDevice.ShowWindow(camPreviewWindow);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            camPreviewWindow.Close();
        }

        private void srcFileOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Title = "Vyberte obrázek";
            dlg.Filter = "Všechny podporované obrázky|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            Nullable<bool> result = dlg.ShowDialog(this);

            if (result == true)
            {
                fileName = dlg.FileName;
                srcFileName.Content = System.IO.Path.GetFileName(fileName);
                srcImage = new BitmapImage(new Uri(fileName, UriKind.RelativeOrAbsolute));
                Process();
            }
        }

        private void camProcess_Click(object sender, RoutedEventArgs e)
        {
            if (camDevice == null)
                return;

            camDevice.CaptureImage();
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Bitmap))
            {
                srcImage = (BitmapSource)data.GetData(DataFormats.Bitmap, true);
                Process();
            }
        }
    }
}
