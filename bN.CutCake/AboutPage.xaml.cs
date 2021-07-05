using bN.CutCake.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Store;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.ApplicationModel.Resources;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace bN.CutCake
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AboutPage : Page
	{
		private NavigationHelper navigationHelper;
		private ObservableDictionary defaultViewModel = new ObservableDictionary();

		public AboutPage()
		{
			this.InitializeComponent();

			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
			this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
		}

		/// <summary>
		/// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
		/// </summary>
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		/// <summary>
		/// Gets the view model for this <see cref="Page"/>.
		/// This can be changed to a strongly typed view model.
		/// </summary>
		public ObservableDictionary DefaultViewModel
		{
			get { return this.defaultViewModel; }
		}

		/// <summary>
		/// Populates the page with content passed during navigation.  Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="sender">
		/// The source of the event; typically <see cref="NavigationHelper"/>
		/// </param>
		/// <param name="e">Event data that provides both the navigation parameter passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
		/// a dictionary of state preserved by this page during an earlier
		/// session.  The state will be null the first time a page is visited.</param>
		private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
		}

		/// <summary>
		/// Preserves state associated with this page in case the application is suspended or the
		/// page is discarded from the navigation cache.  Values must conform to the serialization
		/// requirements of <see cref="SuspensionManager.SessionState"/>.
		/// </summary>
		/// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
		/// <param name="e">Event data that provides an empty dictionary to be populated with
		/// serializable state.</param>
		private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
		}

		#region NavigationHelper registration

		/// <summary>
		/// The methods provided in this section are simply used to allow
		/// NavigationHelper to respond to the page's navigation methods.
		/// <para>
		/// Page specific logic should be placed in event handlers for the  
		/// <see cref="NavigationHelper.LoadState"/>
		/// and <see cref="NavigationHelper.SaveState"/>.
		/// The navigation parameter is available in the LoadState method 
		/// in addition to page state preserved during an earlier session.
		/// </para>
		/// </summary>
		/// <param name="e">Provides data for navigation methods and event
		/// handlers that cannot cancel the navigation request.</param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ShowLoadingBar();
			ApplicationView.GetForCurrentView().SuppressSystemOverlays = false;
			this.navigationHelper.OnNavigatedTo(e);
#if DEBUG
			//try
			//{
				
			//	var storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(
			//			"WindowsStoreProxy.xml");
			//	await CurrentAppSimulator.ReloadSimulatorAsync(storageFile);
			//}
			//catch (Exception ex)
			//{

			//	throw;
			//}
			await Task.Delay(5000);
			var list = await CurrentAppSimulator.LoadListingInformationAsync();

			foreach (var item in list.ProductListings)
			{
				Debug.WriteLine("{0}, {1}, {2},{3}, {4}",
				  item.Key,
				  item.Value.Name,
				  item.Value.FormattedPrice,
				  item.Value.ProductType,
				  item.Value.Description);
			}

			if (list.ProductListings.ContainsKey("2"))
			{
				var product = list.ProductListings["2"];
				uxDonate1Button.Content = product.FormattedPrice;
			}
			else
			{
				uxDonate1Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				uxAboutText.Text =ResourceLoader.GetForCurrentView().GetString("ThankYou2");
			}
#else		
			var list = await CurrentApp.LoadListingInformationAsync();

			if (list.ProductListings.Count > 0)
			{
				if (list.ProductListings.ContainsKey("Donation"))
				{
					var product = list.ProductListings["Donation"];
					uxDonate1Button.Content = product.FormattedPrice;
				}
				else
				{
					uxDonate1Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				}

				if (list.ProductListings.ContainsKey("Donation2"))
				{
					var product = list.ProductListings["Donation2"];
					uxDonate2Button.Content = product.FormattedPrice;
				}
				else
				{
					uxDonate2Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				}

				if (list.ProductListings.ContainsKey("Donation5"))
				{
					var product = list.ProductListings["Donation5"];
					uxDonate5Button.Content = product.FormattedPrice;
				}
				else
				{
					uxDonate5Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				}
			}
			else
			{
				uxAboutText.Text = ResourceLoader.GetForCurrentView().GetString("ThankYou2");
			}
#endif

			HideLoadingBar();
		}

		private void HideLoadingBar()
		{
			uxLoadingBar.Visibility = Visibility.Collapsed;
			uxLoadingBar.IsEnabled = false;
		}

		private void ShowLoadingBar()
		{
			uxLoadingBar.Visibility = Visibility.Visible;
			uxLoadingBar.IsEnabled = true;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			this.navigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		private async void Donate1_Click(object sender, RoutedEventArgs e)
		{
			ShowLoadingBar();
#if DEBUG
			var res = await CurrentAppSimulator.RequestProductPurchaseAsync("1");
			Debug.WriteLine(res.Status);
#else
			var res = await CurrentApp.RequestProductPurchaseAsync("Donation"); 
#endif
			HideLoadingBar();
			await ShowPurchaseMessage(res, "Donation");
		}

		private static async System.Threading.Tasks.Task ShowPurchaseMessage(
			PurchaseResults res, string productId)
		{			
			if (res.Status == ProductPurchaseStatus.Succeeded
				|| res.Status == ProductPurchaseStatus.NotFulfilled)
			{
				await CurrentApp.ReportConsumableFulfillmentAsync(productId, 
					res.TransactionId);

				await ShowMessageDialog("ThankYou");
			}
			else if (res.Status == ProductPurchaseStatus.AlreadyPurchased)
			{
				await ShowMessageDialog("ItemAlreadyBought");
			}
			else
			{
				await ShowMessageDialog("PurchaseError");
			}
		}

		private static async System.Threading.Tasks.Task ShowMessageDialog(string resourceId)
		{
			await new MessageDialog(ResourceLoader.GetForCurrentView().GetString(resourceId))
							.ShowAsync();
		}

		private async void uxDonate2Button_Click(object sender, RoutedEventArgs e)
		{
			ShowLoadingBar();
#if DEBUG
			var res = await CurrentAppSimulator.RequestProductPurchaseAsync("2");
			Debug.WriteLine(res.Status);
#else
			var res = await CurrentApp.RequestProductPurchaseAsync("Donation2"); 
#endif
			HideLoadingBar();
			await ShowPurchaseMessage(res, "Donation2");
		}

		private async void uxDonate5Button_Click(object sender, RoutedEventArgs e)
		{
			ShowLoadingBar();
#if DEBUG
			var res = await CurrentAppSimulator.RequestProductPurchaseAsync("3");
			Debug.WriteLine(res.Status);
#else
			var res = await CurrentApp.RequestProductPurchaseAsync("Donation5"); 
#endif
			HideLoadingBar();
			await ShowPurchaseMessage(res, "Donation5");
		}
	}
}
