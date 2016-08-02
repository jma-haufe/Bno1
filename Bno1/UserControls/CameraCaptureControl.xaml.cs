using System;
using System.Linq;
using System.Threading;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using ZXing;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Bno1.UserControls
{
    /// <summary>
    /// User control to display the camera feed inside a CaptureElement
    /// </summary>
    public sealed partial class CameraCaptureControl : UserControl
    {
        #region Private Fields
        private static Timer timer;
        private Result res;
        private ZXing.BarcodeReader br;
        private WriteableBitmap wrb;
        private MediaCapture captureMgr = null;
        private VideoRotation currRotation = VideoRotation.Clockwise90Degrees;
        private bool isCameraFound = false;
        private int _failCount;
        private string qrCodeContent;

        #endregion

        #region Public Properties and Events
        /// <summary>
        /// Event to let the consumer of the User control know that the email
        /// has been decoded from the QR code that is scanned
        /// </summary>
        public event EventHandler<CameraClickedEventArgs> EmailDecoded = delegate { };

        /// <summary>
        /// Property to get/set the email address field 
        /// </summary>
        public string QrCodeContent
        {
            get { return qrCodeContent; }
            set { qrCodeContent = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for the user control
        /// </summary>
        public CameraCaptureControl()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to handle the Loaded event of the user control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded( object sender, RoutedEventArgs e )
        {
            InitializeMediaCapture();
            ScanQRCode();
        }

        private async void ScanQRCode()
        {
            br = new BarcodeReader { PossibleFormats = new BarcodeFormat[] { BarcodeFormat.QR_CODE } };
            br.Options.TryHarder = true;
            br.AutoRotate = true;
            TimerCallback callBack = new TimerCallback(CaptureQRCodeFromCamera);
            timer = new Timer(callBack, null, 500, Timeout.Infinite);
        }

        /// <summary>
        /// Method to Initialize the Camera present in the device 
        /// </summary>
        private async void InitializeMediaCapture()
        {
            try
            {
                DeviceInformationCollection devices = null;
                captureMgr = new MediaCapture();
                devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);


                // Use the front camera if found one
                if (devices == null || devices.Count == 0)
                {
                    isCameraFound = false;
                    return;
                }

                DeviceInformation info = null;
                info = devices[0];

                MediaCaptureInitializationSettings settings;
                settings = new MediaCaptureInitializationSettings { VideoDeviceId = info.Id }; // 0 => front, 1 => back
                

                await captureMgr.InitializeAsync(settings);
                VideoEncodingProperties resolutionMax = null;
                int max = 0;
                var resolutions = captureMgr.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo);

                for (var i = 0 ; i < resolutions.Count ; i++)
                {
                    VideoEncodingProperties res = (VideoEncodingProperties)resolutions[i];
                    if (res.Width * res.Height > max)
                    {
                        max = (int)(res.Width * res.Height);
                        resolutionMax = res;
                    }
                    
                }

                await captureMgr.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, resolutionMax);
                capturePreview.Source = captureMgr;
                isCameraFound = true;
                captureMgr.SetPreviewRotation(currRotation);

                try
                {
                    if (captureMgr.VideoDeviceController.FocusControl.Supported &&
                        captureMgr.VideoDeviceController.FocusControl.SupportedFocusModes.Any(m => m ==FocusMode.Auto))
                    {
                        captureMgr.VideoDeviceController.FocusControl.Configure(new FocusSettings() { AutoFocusRange = AutoFocusRange.Macro, Distance = ManualFocusDistance.Nearest, Mode = FocusMode.Auto});
                    }
                }
                catch 
                {
                    
                }
                await captureMgr.StartPreviewAsync();
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog("Error while initializing media capture device: " + ex.Message);
                dialog.ShowAsync();
                GC.Collect();
            }
        }

        /// <summary>
        /// Method to handle the Click event of the Capture Code button
        /// </summary>
        /// <param name="sender">Sender of the Event</param>
        /// <param name="e">Arguments of the event</param>
        private async void CaptureQRCodeFromCamera(object data)
        {
            MessageDialog dialog = new MessageDialog(string.Empty);
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (!isCameraFound)
                    {
                        return;
                    }

                    ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();
                    // create storage file in local app storage
                    StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "temp.jpg",
                    CreationCollisionOption.GenerateUniqueName);
                    // take photo
                    await captureMgr.CapturePhotoToStorageFileAsync(imgFormat, file);
                    // Get photo as a BitmapImage
                    BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));
                    bmpImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        wrb = await Windows.UI.Xaml.Media.Imaging.BitmapFactory.New(1, 1).FromStream(fileStream);
                    }
                    
                    res = br.Decode(wrb);
                    
                    if (res != null)
                    {
                        CameraClickedEventArgs cameraArgs = null;
                        timer.Dispose();
                        await captureMgr.StopPreviewAsync();

                        QrCodeContent = res.Text;
                        cameraArgs = new CameraClickedEventArgs {EncodedData = this.QrCodeContent, Image = wrb};
                        if (this.EmailDecoded != null)
                        {
                            EmailDecoded(this, cameraArgs);
                        }
                    }
                    else
                    {
                        _failCount++;
                        if (_failCount > 50)
                        {
                            Closethis();
                        }
                        else
                        {
                            timer.Change(750, Timeout.Infinite);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                dialog = new MessageDialog("Error: " + ex.Message);
                dialog.ShowAsync();
            }
        }

        public void Closethis()
        {
            try
            {
                timer.Dispose();
                CameraClickedEventArgs cameraArgs = new CameraClickedEventArgs
                {
                    EncodedData = "http://abc.de",
                    Image = wrb
                };
                if (this.EmailDecoded != null)
                {
                    EmailDecoded(this, cameraArgs);
                }
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Method to handle the unload event of the User Control. Here, all the MediaCapture resources
        /// are cleaned up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded( object sender, RoutedEventArgs e )
        {
            captureMgr = null;
            wrb = null;
            res = null;
            br = null;
            if (timer != null)
                timer.Dispose();

            GC.Collect();
        }

        #endregion

        //private void capturePreview_Tapped(object sender, TappedRoutedEventArgs e)
        //{
        //switch (currRotation)
        //{
        //        case VideoRotation.None: currRotation=VideoRotation.Clockwise90Degrees;
        //        break;
        //    case VideoRotation.Clockwise90Degrees:
        //        currRotation = VideoRotation.Clockwise180Degrees;
        //        break;
        //    case VideoRotation.Clockwise180Degrees:
        //        currRotation = VideoRotation.Clockwise270Degrees;
        //        break;
        //    default:
        //        currRotation = VideoRotation.None;
        //        break;
        //}
        //this.captureMgr.SetPreviewRotation(currRotation); 
        //}

        private void capturePreview_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Closethis();
        }

        //private async void capturePreview_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        //{
        //    var uriTarget = new Uri("https://drive.google.com/file/d/0B_eDInKw2zx2UmdNM0tyZWN2Mlk/view?usp=sharing");
        //    bool success = await Windows.System.Launcher.LaunchUriAsync(uriTarget);
        //}
    }

    /// <summary>
    /// Class to pass the Event data from the Control to the consumer
    /// </summary>
    public class CameraClickedEventArgs : EventArgs
    {
        private string encodedData;

        /// <summary>
        /// Property to get/set the e-mail address scanned from the QR code
        /// </summary>
        public string EncodedData
        {
            get { return encodedData; }
            set { encodedData = value; }
        }

        public WriteableBitmap Image { get;set; }
    }
}
