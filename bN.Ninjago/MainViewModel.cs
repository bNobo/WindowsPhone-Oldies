using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bN.Core;

namespace bN.Ninjago
{
	public class MainViewModel : BaseViewModel
	{
		public double Rotation { get; set; }
		public double Roll { get; set; }

		public static bool InclinometerIsActive { get; set; }

		public static bool InclinometerCapability { get; set; }
	}
}
