using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace bN.CutCake
{
	public sealed partial class MainPage : Page
	{
		private Windows.Media.Capture.MediaCapture mediaCapture;
		private MainViewModel MainViewModel { get; set; }
		private List<Line> ListeLine = new List<Line>();
		private Inclinometer _inclinometer;
		private readonly DisplayRequest _displayRequest = new DisplayRequest();
		private readonly SystemMediaTransportControls _systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
		private bool _isInitialized;
		private bool _torchEnabled;
		private bool _isRecording;

		public MainPage()
		{
			this.InitializeComponent();

			this.NavigationCacheMode = NavigationCacheMode.Required;

			DataContext = MainViewModel = new MainViewModel();
			MainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
			_inclinometer = Inclinometer.GetDefault();

			if (_inclinometer != null)
			{
				MainViewModel.InclinometerIsActive = true;
				MainViewModel.InclinometerCapability = true;
				_inclinometer.ReportInterval = 40;
				_inclinometer.ReadingChanged += _inclinometer_ReadingChanged;
			}

			Application.Current.Suspending += Current_Suspending;
			Application.Current.Resuming += Current_Resuming;

		}

		async void Current_Resuming(object sender, object e)
		{
			// Handle global application events only if this page is active
			if (Frame.CurrentSourcePageType == typeof(MainPage))
			{
				_systemMediaControls.PropertyChanged += SystemMediaControls_PropertyChanged;
				await InitMediaCapture();
			}
		}

		async void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
		{
			if (Frame.CurrentSourcePageType == typeof(MainPage))
			{
				var deferral = e.SuspendingOperation.GetDeferral();
				_systemMediaControls.PropertyChanged -= SystemMediaControls_PropertyChanged;
				await CleanupCameraAsync();
				deferral.Complete();
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
				await StopRecording(mediaCapture);
				mediaCapture.Failed -= MediaCapture_Failed;
				mediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
				mediaCapture.Dispose();
				mediaCapture = null;
			}
		}

		private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
		{
			await CleanupCameraAsync();
		}

		private async Task StopPreviewAsync()
		{
			// Stop the preview
			try
			{
				await mediaCapture.StopPreviewAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Exception when stopping the preview: {0}", ex.ToString());
			}

			// Use the dispatcher because this method is sometimes called from non-UI threads
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				// Cleanup the UI
				PhotoPreview.Source = null;

				// Allow the device screen to sleep now that the preview is stopped
				_displayRequest.RequestRelease();
			});
		}

		private async void SystemMediaControls_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
		{
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				// Only handle this event if this page is currently being displayed
				if (args.Property == SystemMediaTransportControlsProperty.SoundLevel && Frame.CurrentSourcePageType == typeof(MainPage))
				{
					// Check to see if the app is being muted. If so, it is being minimized.
					// Otherwise if it is not initialized, it is being brought into focus.
					if (sender.SoundLevel == SoundLevel.Muted)
					{
						await CleanupCameraAsync();
					}
					else if (!_isInitialized)
					{
						await InitMediaCapture();
					}
				}
			});
		}



		async void _inclinometer_ReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs args)
		{
			if (!MainViewModel.InclinometerIsActive || MainViewModel.IsFrozen)
			{
				return;
			}

			await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
				() =>
				{
					MainViewModel.Rotation = args.Reading.RollDegrees;
					MainViewModel.Roll = args.Reading.PitchDegrees;
				}
						);
		}

		private void nop()
		{

		}

		void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "NombrePart")
			{
				ClearLines();
				AddLines();
			}
		}

		private void AddLines()
		{
			AddLines(MainViewModel.NombrePart);
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			_systemMediaControls.PropertyChanged += SystemMediaControls_PropertyChanged;
			Window.Current.CoreWindow.VisibilityChanged += CoreWindow_VisibilityChanged;
			await InitMediaCapture();

			if (ListeLine.Count == 0)
			{
				uxEllipse.Visibility = Windows.UI.Xaml.Visibility.Visible;
				AddLines();
			}

			// Bon à savoir : les AppBarButton ne sont plus réactif du tout lorsque la 
			// barre de navigation est affichée. Ce n'est pas propre à mon appli, le 
			// comportement est le même dans toutes ! Je la masque quand même car elle
			// est gênante pour l'utilisateur...
			ApplicationView.GetForCurrentView().SuppressSystemOverlays = true;
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

		private async Task StartPreviewing()
		{
			// Prevent the device from sleeping while the preview is running
			_displayRequest.RequestActive();
			// start previewing
			PhotoPreview.Source = mediaCapture;
			await mediaCapture.StartPreviewAsync();
		}

		private async Task InitMediaCapture()
		{
			if (mediaCapture == null)
			{
				uxLoadingRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
				uxLoadingRing.IsActive = true;

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
					MainViewModel.IsFrozen = false;
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

					if (mediaCapture.VideoDeviceController.TorchControl.Supported)
					{
						uxFlashLedButton.Visibility = Visibility.Visible;
					}
					else
					{
						uxFlashLedButton.Visibility = Visibility.Collapsed;
					}

					mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
					mediaCapture.Failed += MediaCapture_Failed;

					if (_torchEnabled)
					{
						await InitializeAndToggleTorch();
					}
				}

				uxLoadingRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				uxLoadingRing.IsActive = false;
			}
		}

		private void AddLines(int nombre)
		{
			if (ListeLine.Count > 0)
			{
				return;
			}

			var radius = uxGrid.ActualHeight / 2;
			var center = new Point(uxGrid.ActualWidth / 2, radius);
			var initialAngle = (Math.PI * 2) / nombre;
			// Angle de départ pour avoir une tranche entière vers le bas
			double angle = (Math.PI * 3) / 2 + initialAngle / 2;
			//var lineBrush = new SolidColorBrush(Colors.White);

			for (int i = 0; i < nombre; i++)
			{
				var line = new Line();
				//line.Stroke = lineBrush;
				line.StrokeThickness = MainViewModel.StrokeThickness;
				line.X1 = center.X;
				line.Y1 = center.Y;
				line.X2 = line.X1 + Math.Cos(angle) * radius;
				line.Y2 = line.Y1 - Math.Sin(angle) * radius;
				ListeLine.Add(line);
				uxGrid.Children.Add(line);
				angle += initialAngle;
			}

		}

		protected override async void OnNavigatedFrom(NavigationEventArgs e)
		{
			_systemMediaControls.PropertyChanged -= SystemMediaControls_PropertyChanged;
			await CleanupCameraAsync();
		}

		private void ClearLines()
		{
			foreach (var line in ListeLine)
			{
				uxGrid.Children.Remove(line);
			}

			ListeLine.Clear();
		}

		private static async Task<DeviceInformation> GetCameraID(
			Windows.Devices.Enumeration.Panel desired)
		{
			DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
				.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desired);

			if (deviceID != null) return deviceID;
			else throw new Exception(string.Format("Camera of type {0} doesn't exist.", desired));
		}

		private void uxGrid2_ManipulationDelta(object sender, 
			ManipulationDeltaRoutedEventArgs e)
		{
			if (e.Delta.Rotation == 0)
			{
				return;
			}

			MainViewModel.InclinometerIsActive = false;
			var angle = e.Delta.Rotation;
			MainViewModel.ManipulateRotation(angle);
		}

		private void uxGrid2_ManipulationStarted(object sender, 
			ManipulationStartedRoutedEventArgs e)
		{
			if (MainViewModel.IsFrozen)
			{
				e.Complete();
				return;
			}
		}

		private async void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			await CleanupCameraAsync();
		}

		private async void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			await InitMediaCapture();
		}

		private void AboutButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(AboutPage));
		}

		private async void FlashLedButton_Click(object sender, RoutedEventArgs e)
		{
			uxFlashLedButton.Click -= FlashLedButton_Click;

			if (!_torchEnabled)
			{
				await InitializeAndToggleTorch();
			}
			else
			{
				await StopRecording(mediaCapture);
				_torchEnabled = false;
			}

			uxFlashLedButton.Click += FlashLedButton_Click;
		}

		private async Task InitializeAndToggleTorch()
		{
			if (mediaCapture.VideoDeviceController.TorchControl.Supported)
			{
				// Get resolutions and set to lowest available for temporary video file.
				var resolutions = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoRecord).Select(x => x as VideoEncodingProperties);
				var lowestResolution = resolutions.OrderBy(x => x.Height * x.Width).ThenBy(x => (x.FrameRate.Numerator / (double)x.FrameRate.Denominator)).FirstOrDefault();
				await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoRecord, lowestResolution);
			}
			else
			{
				// Torch not supported, exit method
				return;
			}

			_torchEnabled = true;
			// Prep for video recording
			// Get Application temporary folder to store temporary video file folder
			StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;

			// Create a temp Flash folder 
			var folder = await tempFolder.CreateFolderAsync("TempFlashlightFolder", 
				CreationCollisionOption.OpenIfExists);

			// Create video encoding profile as MP4 
			var videoEncodingProperties = MediaEncodingProfile.CreateMp4(
				VideoEncodingQuality.Vga);

			// Create new unique file in the Flash folder and record video
			var videoStorageFile = await folder.CreateFileAsync("TempFlashlightFile", CreationCollisionOption.GenerateUniqueName);

			// Start recording
			await mediaCapture.StartRecordToStorageFileAsync(videoEncodingProperties, videoStorageFile);
			
			// Now Toggle TorchControl property
			mediaCapture.VideoDeviceController.TorchControl.Enabled = true;
			_isRecording = true;
		}

		private async void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
		{
			await StopRecording(sender);
		}

		private async Task StopRecording(MediaCapture sender)
		{
			if (_isRecording)
			{
				_isRecording = false;
				if (sender.VideoDeviceController.TorchControl.Supported)
				{
					sender.VideoDeviceController.TorchControl.Enabled = false;
				}
				await sender.StopRecordAsync();
				StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
				var folder = await tempFolder.CreateFolderAsync("TempFlashlightFolder",
					CreationCollisionOption.OpenIfExists);
				await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
				
			}
		}

		private async void RateButton_Click(object sender, RoutedEventArgs e)
		{
			await Windows.System.Launcher.LaunchUriAsync(
				new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
		}

	}
}
