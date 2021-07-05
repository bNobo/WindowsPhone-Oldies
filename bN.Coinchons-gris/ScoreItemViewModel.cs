using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using bN.Core;

namespace bN.Coinchons
{
	public class ScoreItemViewModel : BaseViewModel
	{
		private int _ourScore;

		public int OurScore
		{
			get { return _ourScore; }
			set
			{
				if (value != _ourScore)
				{
					_ourScore = value;
					RaisePropertyChanged();
				}
			}
		}
		private int _theirScore;

		public int TheirScore
		{
			get { return _theirScore; }
			set
			{
				if (value != _theirScore)
				{
					_theirScore = value;
					RaisePropertyChanged();
				}
			}
		}
	}
}
