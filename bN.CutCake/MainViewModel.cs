using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using bN.CutCake.Common;

namespace bN.CutCake
{
	[Magic]
	public class MainViewModel : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

		private const int _minimumPart = 3;
		private const int _maximumPart = 15;

		public MainViewModel()
		{
			NombrePart = 7;
			StrokeThickness = 5;
			Rotation = -45;

			AddPartCommand = new RelayCommand(AddPart, CanAddPart);
			RemovePartCommand = new RelayCommand(RemovePart, CanRemovePart);

			PropertyChanged += MainViewModel_PropertyChanged;
		}

		void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "NombrePart")
			{

			}
		}

		private bool CanRemovePart()
		{
			return NombrePart > _minimumPart;
		}

		private void RemovePart()
		{
			NombrePart = Math.Max(NombrePart - 1, _minimumPart);
			RaiseCanExecuteChanged();
		}

		private void RaiseCanExecuteChanged()
		{
			AddPartCommand.RaiseCanExecuteChanged();
			RemovePartCommand.RaiseCanExecuteChanged();
		}

		private bool CanAddPart()
		{
			return NombrePart < _maximumPart;
		}

		private void AddPart()
		{
			NombrePart = Math.Min(NombrePart + 1, _maximumPart);
			RaiseCanExecuteChanged();
		}

		public int NombrePart { get; set; }
		public int StrokeThickness { get; set; }
		public double Rotation { get; set; }
		public double Roll { get; set; }
		public bool InclinometerIsActive { get; set; }
		public bool IsFrozen { get; set; }
		public bool InclinometerCapability { get; set; }
		public RelayCommand AddPartCommand { get; set; }
		public RelayCommand RemovePartCommand { get; set; }

		internal void ManipulateRotation(float angle)
		{
			Rotation = Limit(Rotation + angle, -75, -15);
		}

		private static double Limit(double value, double minimum, double maximum)
		{
			if (value < minimum)
			{
				value = minimum;
			}
			else if (value > maximum)
			{
				value = maximum;
			}

			return value;
		}
	}
}
