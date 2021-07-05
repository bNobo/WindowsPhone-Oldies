using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace bN.Core
{
	public static class ResourceHelper
	{
		public static string GetString(string resourceId)
		{
			return ResourceLoader.GetForCurrentView().GetString(resourceId);
		}
	}
}
