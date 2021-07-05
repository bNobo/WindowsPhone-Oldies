using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bN.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace bN.Coinchons
{
	public class BidToTextValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			int bid = (int)value;

			if (bid == 0)
			{
				return ResourceHelper.GetString("SelectBid");
			}
			else
			{
				return bid.ToString();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
