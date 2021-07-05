using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace bN.Ninjago
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private Windows.Media.Capture.MediaCapture mediaCapture;
		private Inclinometer _inclinometer;
		private bool _isInitialized;
		public MainViewModel MainViewModel { get { return DataContext as MainViewModel; } }
		private readonly DisplayRequest _displayRequest = new DisplayRequest();

		public MainPage()
		{
			this.InitializeComponent();

			this.NavigationCacheMode = NavigationCacheMode.Required;
			_inclinometer = Inclinometer.GetDefault();

			if (_inclinometer != null)
			{
				MainViewModel.InclinometerIsActive = true;
				MainViewModel.InclinometerCapability = true;
				_inclinometer.ReportInterval = 40;
				_inclinometer.ReadingChanged += _inclinometer_ReadingChanged;
			}
		}

		async void _inclinometer_ReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs args)
		{
			if (!MainViewModel.InclinometerIsActive)
			{
				return;
			}

			await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
				() =>
				{
					MainViewModel.Rotation = args.Reading.RollDegrees + 90;
					MainViewModel.Roll = args.Reading.PitchDegrees;
				}
						);
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			Window.Current.CoreWindow.VisibilityChanged += CoreWindow_VisibilityChanged;
			await InitMediaCapture();
			// TODO: Prepare page for display here.

			// TODO: If your application contains multiple pages, ensure that you are
			// handling the hardware Back button by registering for the
			// Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
			// If you are using the NavigationHelper provided by some templates,
			// this event is handled for you.
		}

		private async Task InitMediaCapture()
		{
			if (mediaCapture == null)
			{
				//uxLoadingRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
				//uxLoadingRing.IsActive = true;

				mediaCapture = new Windows.Media.Capture.MediaCapture();
				var cameraID = await GetCameraID(Windows.Devices.Enumeration.Panel.Back);

				try
				{
					await mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
					{
						StreamingCaptureMode = StreamingCaptureMode.Video,
						PhotoCaptureSource = PhotoCaptureSource.VideoPreview,
						AudioDeviceId = string.Empty,
						VideoDeviceId = cameraID.Id
					});

					_isInitialized = true;
					//MainViewModel.IsFrozen = false;
				}
				catch (UnauthorizedAccessException)
				{
					Debug.WriteLine("The app was denied access to the camera");
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Exception when initializing MediaCapture with {0}: {1}", cameraID.Id, ex.ToString());
				}

				if (_isInitialized)
				{
					await StartPreviewing();

					//if (mediaCapture.VideoDeviceController.TorchControl.Supported)
					//{
					//	uxFlashLedButton.Visibility = Visibility.Visible;
					//}
					//else
					//{
					//	uxFlashLedButton.Visibility = Visibility.Collapsed;
					//}

					//mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
					//mediaCapture.Failed += MediaCapture_Failed;

					//if (_torchEnabled)
					//{
					//	await InitializeAndToggleTorch();
					//}
				}

				//uxLoadingRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				//uxLoadingRing.IsActive = false;
			}
		}

		private static async Task<DeviceInformation> GetCameraID(
			Windows.Devices.Enumeration.Panel desired)
		{
			DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
				.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desired);

			if (deviceID != null) return deviceID;
			else throw new Exception(string.Format("Camera of type {0} doesn't exist.", desired));
		}

		private async Task StartPreviewing()
		{
			// Prevent the device from sleeping while the preview is running
			_displayRequest.RequestActive();
			// start previewing
			PhotoPreview.Source = mediaCapture;
			await mediaCapture.StartPreviewAsync();
		}

		async void CoreWindow_VisibilityChanged(CoreWindow sender, VisibilityChangedEventArgs args)
		{
			if (args.Visible)
			{
				await InitMediaCapture();
			}
			else
			{
				await CleanupCameraAsync();
			}
		}

		private async Task CleanupCameraAsync()
		{
			Debug.WriteLine("CleanupCameraAsync");

			if (_isInitialized)
			{

				// En ne stoppant pas la preview, on permet à la dernière image de la
				// caméra de rester à l'écran en mode "freeze" (bon ok j'exploite un
				// bug là :))
				//await StopPreviewAsync();				
				_isInitialized = false;
			}

			if (mediaCapture != null)
			{
				//await StopRecording(mediaCapture);
				//mediaCapture.Failed -= MediaCapture_Failed;
				//mediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
				mediaCapture.Dispose();
				mediaCapture = null;
			}
		}

	}
}
