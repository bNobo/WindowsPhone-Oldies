using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace bN.Core
{
	public static class RatingHelper
	{
		public static async Task RateAndReviewCurrentApp()
		{
			await RateAndReviewApp(CurrentApp.AppId.ToString());
		}

		public static async Task RateAndReviewApp(string appId)
		{
			await Windows.System.Launcher.LaunchUriAsync(
				new Uri("ms-windows-store:reviewapp?appid=" + appId));
		}
	}
}
