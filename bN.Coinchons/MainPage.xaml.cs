using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using bN.Core;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Popups;
using bN.Coinchons.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace bN.Coinchons
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
			this.NavigationCacheMode = NavigationCacheMode.Required;
			TrefleImage = (ImageSource)Application.Current.Resources["TrefleImage"];
			PiqueImage = (ImageSource)Application.Current.Resources["PiqueImage"];
			Window.Current.CoreWindow.VisibilityChanged += CoreWindow_VisibilityChanged;
		}

		private void CoreWindow_VisibilityChanged(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.VisibilityChangedEventArgs args)
		{
			var vm = DataContext as MainViewModel;		

			if (!args.Visible)
			{
				return;
			}

			// Calcul de la couleur d'accentuation inversée
			var color = GetPhoneAccentColor();
			PhoneAccentBrush = new SolidColorBrush(color);
			PhoneInvertAccentBrush = new SolidColorBrush(InverseTeinte(color));

			// Rechargement des images au cas où thème modifié
				

			if (vm != null)
			{
				if (vm.IsClubs)
				{
					SetTrefleImageInverse();
				}
				else
				{
					SetTrefleImage();
				}

				if (vm.IsSpades)
				{
					SetPiqueImageInverse();
				}
				else
				{
					SetPiqueImage();
				}

				ApplyTeamBrush(vm);
			}

			
			//Color.FromArgb(255, (byte)(255 ^ color.R), (byte)(255 ^ color.G), (byte)(255 ^ color.B)));
		}

		private static Color GetPhoneAccentColor()
		{
			var color = (Color)Application.Current.Resources["SystemColorControlAccentColor"];
			return color;
		}

		private void ApplyTeamBrush(MainViewModel vm)
		{
			if (vm.TeamUsFirstPlayerNumber == 1)
			{
				Team13ButtonBrush = PhoneAccentBrush;
				Team24ButtonBrush = PhoneInvertAccentBrush;
			}
			else
			{
				Team13ButtonBrush = PhoneInvertAccentBrush;
				Team24ButtonBrush = PhoneAccentBrush;
			}
		}

		private Color InverseTeinte(Color color)
		{
			var r = color.R / 255D;
			var g = color.G / 255D;
			var b = color.B / 255D;
			var tsv = RgbToTsv(r, g, b);
			//tsv = ToComplementaire(tsv);
			tsv.T = InverseTeinte(tsv.T);
			Rgb rgb = TsvToRgb(tsv.T, tsv.S, tsv.V);
			var newColor = Color.FromArgb(255, (byte)(rgb.R * 255), (byte)(rgb.G * 255), (byte)(rgb.B * 255));
			return newColor;
		}

		private Tsv ToComplementaire(Tsv tsv)
		{
			var t = tsv.T;
			var s = tsv.S;
			var v = tsv.V;

			if (t >= 180)
			{
				tsv.T = t - 180;
			}
			else
			{
				tsv.T = t + 180;
			}

			tsv.S = (v * s) / (v * (s - 1) + 1);
			tsv.V = v * (s - 1) + 1;

			return tsv;
		}

		private Rgb TsvToRgb(int t, double s, double v)
		{
			var ti = Convert.ToInt32(Math.Floor((double)t / 60)) % 6;
			var f = (double)t / 60 - Math.Floor((double)t / 60);
			var l = v * (1 - s);
			var m = v * (1 - f * s);
			var n = v * (1 - (1 - f) * s);
			Rgb res;

			switch ((int)ti)
			{
				case 0:
					res = new Rgb { R = v, G = n, B = l };
					break;
				case 1:
					res = new Rgb { R = m, G = v, B = l };
					break;
				case 2:
					res = new Rgb { R = l, G = v, B = n };
					break;
				case 3:
					res = new Rgb { R = l, G = m, B = v };
					break;
				case 4:
					res = new Rgb { R = n, G = l, B = v };
					break;
				case 5:
					res = new Rgb { R = v, G = l, B = m };
					break;
				default:
					throw new Exception("Universe is about to colapse");
			}

			return res;
		}

		private int InverseTeinte(int teinte)
		{
			if (teinte < 0 || teinte > 360)
			{
				throw new ArgumentException();
			}

			var newTeinte = teinte - 180;

			if (newTeinte < 0)
			{
				newTeinte = 360 + newTeinte;
			}

			return newTeinte;
		}

		public struct Tsv
		{
			public int T { get; set; }
			public double S { get; set; }
			public double V { get; set; }
		}

		public struct Rgb
		{
			public double R { get; set; }
			public double G { get; set; }
			public double B { get; set; }
		}

		private Tsv RgbToTsv(double r, double g, double b)
		{
			var max = Math.Max(r, Math.Max(g, b));
			var min = Math.Min(r, Math.Min(g, b));
			var delta = max - min;
			int t = 0;
			double s = 0;
			double v = 0;

			// Teinte
			if (min == max)
			{
				t = 0;
			}
			else if (max == r)
			{
				t = (int)((60 * ((g - b) / delta) + 360) % 360);
			}
			else if (max == g)
			{
				t = (int)(60 * ((b - r) / delta) + 120);
			}
			else if (max == b)
			{
				t = (int)(60 * ((r - g) / delta) + 240);
			}

			// Saturation
			if (max == 0)
			{
				s = 0;
			}
			else
			{
				s = 1 - min / max;
			}

			// Valeur
			v = max;

			return new Tsv { T = t, S = s, V = v };
		}

		public ImageSource TrefleImage
		{
			get { return (ImageSource)GetValue(TrefleImageProperty); }
			set { SetValue(TrefleImageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for TrefleImage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TrefleImageProperty =
			DependencyProperty.Register("TrefleImage", typeof(ImageSource), typeof(MainPage), new PropertyMetadata(null));


		public ImageSource PiqueImage
		{
			get { return (ImageSource)GetValue(PiqueImageProperty); }
			set { SetValue(PiqueImageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PiqueImage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PiqueImageProperty =
			DependencyProperty.Register("PiqueImage", typeof(ImageSource), typeof(MainPage), new PropertyMetadata(null));

		public Brush PhoneInvertAccentBrush
		{
			get { return (Brush)GetValue(PhoneInvertAccentBrushProperty); }
			set { SetValue(PhoneInvertAccentBrushProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PhoneInvertAccentBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PhoneInvertAccentBrushProperty =
			DependencyProperty.Register("PhoneInvertAccentBrush", typeof(Brush), typeof(MainPage), new PropertyMetadata(null));



		public Brush Team13ButtonBrush
		{
			get { return (Brush)GetValue(Team13ButtonBrushProperty); }
			set { SetValue(Team13ButtonBrushProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Team13ButtonBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty Team13ButtonBrushProperty =
			DependencyProperty.Register("Team13ButtonBrush", typeof(Brush), typeof(MainPage), new PropertyMetadata(null));



		public Brush Team24ButtonBrush
		{
			get { return (Brush)GetValue(Team24ButtonBrushProperty); }
			set { SetValue(Team24ButtonBrushProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Team24ButtonBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty Team24ButtonBrushProperty =
			DependencyProperty.Register("Team24ButtonBrush", typeof(Brush), typeof(MainPage), new PropertyMetadata(null));



		public Brush PhoneAccentBrush
		{
			get { return (Brush)GetValue(PhoneAccentBrushProperty); }
			set { SetValue(PhoneAccentBrushProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PhoneAccentBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PhoneAccentBrushProperty =
			DependencyProperty.Register("PhoneAccentBrush", typeof(Brush), typeof(MainPage), new PropertyMetadata(null));


		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			if ((bool)e.Parameter)
			{
				await ContextManager.RestoreContext(this.Frame);
			}

			var vm = DataContext as MainViewModel;

			if (vm != null)
			{
				await vm.LoadNameListAsync();
			}

			// TODO: Prepare page for display here.

			// TODO: If your application contains multiple pages, ensure that you are
			// handling the hardware Back button by registering for the
			// Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
			// If you are using the NavigationHelper provided by some templates,
			// this event is handled for you.
		}

		private void uxTrefleRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			SetTrefleImageInverse();
		}

		private void SetTrefleImageInverse()
		{
			TrefleImage = (ImageSource)Application.Current.Resources["TrefleImage_Inverse"];
		}

		private void uxTrefleRadioButton_Unchecked(object sender, RoutedEventArgs e)
		{
			var radio = sender as RadioButton;
			radio.Tag = null;
			SetTrefleImage();
		}

		private void SetTrefleImage()
		{
			TrefleImage = (ImageSource)Application.Current.Resources["TrefleImage"];
		}

		private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
		{
			var radio = sender as RadioButton;
			radio.Tag = null;
		}

		private void uxPiqueRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			SetPiqueImageInverse();
		}

		private void SetPiqueImageInverse()
		{
			PiqueImage = (ImageSource)Application.Current.Resources["PiqueImage_Inverse"];
		}

		private void uxPiqueRadioButton_Unchecked(object sender, RoutedEventArgs e)
		{
			var radio = sender as RadioButton;
			radio.Tag = null;
			SetPiqueImage();
		}

		private void SetPiqueImage()
		{
			PiqueImage = (ImageSource)Application.Current.Resources["PiqueImage"];
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			var textBox = sender as TextBox;
			textBox.SelectAll();
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as ContentControl;

			if ((string)button.Content == "SaveContext")
			{
				await ContextManager.SaveContext();
			}
			else
			{
				await ContextManager.RestoreContext();
			}
		}

		private void uxOurBidFlyout_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if (_openedFlyout == null)
			{
				return;
			}

			_openedFlyout.Hide();
		}

		private Flyout _openedFlyout;

		private void uxBidFlyout_Opened(object sender, object e)
		{
			_openedFlyout = sender as Flyout;
		}

		private void uxBidButton_Tapped(object sender, TappedRoutedEventArgs e)
		{
			_openedFlyout.Hide();
		}

		private void RadioButton_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var radio = sender as RadioButton;

			if (radio.IsChecked.Value && radio.Tag != null)
			{
				radio.IsChecked = false;
				//radio.Tag = null;
			}
			else if (radio.IsChecked.Value)
			{
				radio.Tag = "AlreadyChecked";
			}
		}

		private async void RateButton_Click(object sender, RoutedEventArgs e)
		{
			await RatingHelper.RateAndReviewApp("378f9770-9f84-4ad4-a1fe-db3f5d1b3b9b");
		}

		private void uxSwapTeamsButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as MainViewModel;

			if (vm != null)
			{
				vm.SwapTeams();
				ApplyTeamBrush(vm);
			}
		}

		private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			var box = sender as AutoSuggestBox;
			var vm = DataContext as MainViewModel;

			if (vm != null)
			{
				vm.FilterNameList(box.Text);
			}
		}
        
        private async void TeamsButton_Click(object sender, RoutedEventArgs e)
        {
            await uxMainHub.ScrollToSectionAnimated(uxTeamSection, 2);
        }

        private async void uxValidTeamsButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;

            if (vm != null)
            {
                vm.UpdateNameList();
				await vm.SaveNameList();
            }
           
            await uxMainHub.ScrollToSectionAnimated(uxGameSection, 0);
        }

        
        private async void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            string confirmMessage = ResourceHelper.GetString("ConfirmNewGame");
            string okMessage = ResourceHelper.GetString("Ok");
            string cancelMessage = ResourceHelper.GetString("Cancel");
            MessageDialog dialog = new MessageDialog(confirmMessage);
            dialog.Commands.Add(new UICommand(okMessage, async cmd =>
            {
                var vm = DataContext as MainViewModel;

                if (vm != null)
                {
                    vm.NewGame();
					await uxMainHub.ScrollToSectionAnimated(uxGameSection, 0);
                }
            }));
            dialog.Commands.Add(new UICommand(cancelMessage));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            await dialog.ShowAsync();
        }

		private async void uxDelButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as MainViewModel;

			if (vm != null)
			{
				await vm.DeleteNameListAsync();
			}
		}
    }
}
