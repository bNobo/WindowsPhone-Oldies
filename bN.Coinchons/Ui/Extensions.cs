using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.Extensions;

namespace bN.Coinchons.UI
{
    public static class Extensions
    {
        public async static Task ScrollToSectionAnimated(this Hub hub, HubSection section, int index)
        {
            var viewer = hub.GetFirstDescendantOfType<ScrollViewer>();
            double offset =  index * section.ActualWidth;
#if DEBUG
            Debug.WriteLine(offset);
#endif
            await viewer.ScrollToHorizontalOffsetWithAnimation(offset);
            //await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => viewer.ChangeView(offset, null, null, false));
                
        }

    }
}
