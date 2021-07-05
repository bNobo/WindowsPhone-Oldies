using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using bN.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
        }

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
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

		
		private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
		{
			var radio = sender as RadioButton;
			radio.Tag = null;
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
				radio.Tag = null;
			}
			else if (radio.IsChecked.Value)
			{
				radio.Tag = "AlreadyChecked";
			}
		}

		private async void RateButton_Click(object sender, RoutedEventArgs e)
		{
			await RatingHelper.RateAndReviewCurrentApp();
		}
    }
}
