using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bno1.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QRPage : Page
    {
        private MediaCapture captureMgr;
        private bool isCameraFound;
    
        private object res;
        private object QrCodeContent;

        public QRPage()
        {
            this.InitializeComponent();
        }

        
        private async void cameraCaptureControlUC_EmailDecoded(object sender, Bno1.UserControls.CameraClickedEventArgs e)
        {
            this.textResult.Text = e.EncodedData.ToString();


            await Finalyze();
        }

        private async Task Finalyze()
        {
            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ms);
                // Get pixels of the WriteableBitmap object 

                WriteableBitmap writeableBitmap = null;
                // Save the image file with jpg extension 
                var uri = new System.Uri("ms-appx:///Assets/correct_qr.png");
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    writeableBitmap = await Windows.UI.Xaml.Media.Imaging.BitmapFactory.New(1, 1).FromStream(fileStream);
                }
                Stream pixelStream = writeableBitmap.PixelBuffer.AsStream();
                byte[] pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint) writeableBitmap.PixelWidth,
                    (uint) writeableBitmap.PixelHeight, 96.0, 96.0, pixels);

                await encoder.FlushAsync();


                this.image.Source = await Windows.UI.Xaml.Media.Imaging.BitmapFactory.New(1, 1).FromStream(ms);
                ;
            }


            this.CamaraGrid.Visibility = Visibility.Collapsed;
            this.FinishedGrid.Visibility = Visibility.Visible;

            //var uriTarget = new System.Uri(e.EncodedData);
            var uriTarget = new Uri("https://drive.google.com/file/d/0B_eDInKw2zx2UmdNM0tyZWN2Mlk/view?usp=sharing");
            bool success = await Windows.System.Launcher.LaunchUriAsync(uriTarget);
        }
    }
}
