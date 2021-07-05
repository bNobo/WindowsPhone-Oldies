using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using bN.Core;

namespace bN.Coinchons
{
	public class MainViewModel : BaseViewModel
	{
		[IgnoreDataMember]
		public int OurTotal { get { return History.Sum(x => x.OurScore); } }
		[IgnoreDataMember]
		public int TheirTotal { get { return History.Sum(x => x.TheirScore); } }
		#region INotifyPropertyChanged properties
		private bool _isHearts;

		public bool IsHearts
		{
			get { return _isHearts; }
			set
			{
				if (value != _isHearts)
				{
					_isHearts = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _isDiamonds;

		public bool IsDiamonds
		{
			get { return _isDiamonds; }
			set
			{
				if (value != _isDiamonds)
				{
					_isDiamonds = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _isClubs;

		public bool IsClubs
		{
			get { return _isClubs; }
			set
			{
				if (value != _isClubs)
				{
					_isClubs = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _isSpades;

		public bool IsSpades
		{
			get { return _isSpades; }
			set
			{
				if (value != _isSpades)
				{
					_isSpades = value;
					RaisePropertyChanged();
				}
			}
		}

		private int _ourBid;

		public int OurBid
		{
			get { return _ourBid; }
			set
			{
				if (value != _ourBid)
				{
					_ourBid = value;
					RaisePropertyChanged();
				}
			}
		}
		private int _theirBid;

		public int TheirBid
		{
			get { return _theirBid; }
			set
			{
				if (value != _theirBid)
				{
					_theirBid = value;
					RaisePropertyChanged();
				}
			}
		}
		
		private bool _dealerIsPlayer1;

		public bool DealerIsPlayer1
		{
			get { return _dealerIsPlayer1; }
			set
			{
				if (value != _dealerIsPlayer1)
				{
					_dealerIsPlayer1 = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _dealerIsPlayer2;

		public bool DealerIsPlayer2
		{
			get { return _dealerIsPlayer2; }
			set
			{
				if (value != _dealerIsPlayer2)
				{
					_dealerIsPlayer2 = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _dealerIsPlayer3;

		public bool DealerIsPlayer3
		{
			get { return _dealerIsPlayer3; }
			set
			{
				if (value != _dealerIsPlayer3)
				{
					_dealerIsPlayer3 = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _dealerIsPlayer4;

		public bool DealerIsPlayer4
		{
			get { return _dealerIsPlayer4; }
			set
			{
				if (value != _dealerIsPlayer4)
				{
					_dealerIsPlayer4 = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _isBeloteOurs;

		public bool IsBeloteOurs
		{
			get { return _isBeloteOurs; }
			set
			{
				if (value != _isBeloteOurs)
				{
					_isBeloteOurs = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _isBeloteTheirs;

		public bool IsBeloteTheirs
		{
			get { return _isBeloteTheirs; }
			set
			{
				if (value != _isBeloteTheirs)
				{
					_isBeloteTheirs = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _isCoinche;

		public bool IsCoinche
		{
			get { return _isCoinche; }
			set
			{
				if (value != _isCoinche)
				{
					_isCoinche = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _isSurcoinche;

		public bool IsSurcoinche
		{
			get { return _isSurcoinche; }
			set
			{
				if (value != _isSurcoinche)
				{
					_isSurcoinche = value;
					RaisePropertyChanged();
				}
			}
		}
		private ObservableCollection<ScoreItemViewModel> _history;

		public ObservableCollection<ScoreItemViewModel> History
		{
			get { return _history; }
			set
			{
				if (value != _history)
				{
					_history = value;
					RaisePropertyChanged();
				}
			}
		} 
		#endregion
		[IgnoreDataMember]
		public RelayCommand Done { get; set; }
		[IgnoreDataMember]
		public RelayCommand Fallen { get; set; }
		[IgnoreDataMember]
		public ICommand OurBidCommand { get; set; }
		[IgnoreDataMember]
		public ICommand TheirBidCommand { get; set; }

		public MainViewModel()
		{
			_history = new ObservableCollection<ScoreItemViewModel>();
			DealerIsPlayer1 = true;
			InitGame();
			PropertyChanged += MainViewModel_PropertyChanged;
		}

		private void InitGame()
		{
			Done = new RelayCommand(DoDone, CanDoDone);
			Fallen = new RelayCommand(DoFallen, CanDoFallen);
			OurBidCommand = new RelayCommand(SelectOurBid);
			TheirBidCommand = new RelayCommand(SelectTheirBid);
		}

		private void SelectTheirBid(object parameter)
		{
			TheirBid = int.Parse(parameter.ToString());
		}

		private void SelectOurBid(object parameter)
		{
			OurBid = int.Parse(parameter.ToString());
		}

		void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
#if DEBUG
			Debug.WriteLine("PropertyChanged {0}", e.PropertyName); 
#endif
			PropertyChanged -= MainViewModel_PropertyChanged;

			if (e.PropertyName == "OurBid")
			{
				TheirBid = 0;
				Done.RaiseCanExecuteChanged();
				Fallen.RaiseCanExecuteChanged();
			}
			else if (e.PropertyName == "TheirBid")
			{
				OurBid = 0;
				Done.RaiseCanExecuteChanged();
				Fallen.RaiseCanExecuteChanged();
			}

			PropertyChanged += MainViewModel_PropertyChanged;
		}

		private bool CanDoFallen()
		{
			return IsThereBid();
		}

		private void DoFallen()
		{
			CalculateFallenScore();
			SelectNextDealer();
			ResetBid();
		}

		private void CalculateFallenScore()
		{
			var score = 160;
			bool weLost;

			if (weLost = OurBid > TheirBid)
			{
				if (OurBid == 250)
				{
					score = 250;
				}
			}
			else
			{
				if (TheirBid == 250)
				{
					score = 250;
				}
			}

			if (IsCoinche)
			{
				score *= 2;
			}
			else if (IsSurcoinche)
			{
				score *= 4;
			}

			var scoreItem = CreateScoreItem();

			if (weLost)
			{
				scoreItem.TheirScore = score;

				if (IsBeloteOurs || IsBeloteTheirs)
				{
					scoreItem.TheirScore += 20;
				}
			}
			else
			{
				scoreItem.OurScore = score;

				if (IsBeloteOurs || IsBeloteTheirs)
				{
					scoreItem.OurScore += 20;
				}
			}

			SaveScoreAndRefreshTotals(scoreItem);
		}

		private ScoreItemViewModel CreateScoreItem()
		{
			var scoreItem = new ScoreItemViewModel();
			scoreItem.PropertyChanged += scoreItem_PropertyChanged;
			return scoreItem;
		}

		void scoreItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RefreshTotals();
		}

		private bool CanDoDone()
		{
			return IsThereBid();
		}

		private bool IsThereBid()
		{
			return OurBid > 0 || TheirBid > 0;
		}

		private void DoDone()
		{
			CalculateDoneScore();
			SelectNextDealer();
			ResetBid();
		}

		private void ResetBid()
		{
			IsBeloteOurs = false;
			IsBeloteTheirs = false;
			IsHearts = false;
			IsDiamonds = false;
			IsClubs = false;
			IsSpades = false;
			IsCoinche = false;
			IsSurcoinche = false;
			OurBid = 0;
			TheirBid = 0;
		}

		private void SelectNextDealer()
		{
			if (DealerIsPlayer1)
			{
				DealerIsPlayer2 = true;
			}
			else if (DealerIsPlayer2)
			{
				DealerIsPlayer3 = true;
			}
			else if (DealerIsPlayer3)
			{
				DealerIsPlayer4 = true;
			}
			else
			{
				DealerIsPlayer1 = true;
			}
		}

		private void CalculateDoneScore()
		{
			var score = 0;
			bool weWon;

			if (weWon = OurBid > TheirBid)
			{
				score = OurBid;
			}
			else
			{
				score = TheirBid;
			}

			if (IsCoinche)
			{
				score *= 2;
			}
			else if (IsSurcoinche)
			{
				score *= 4;
			}

			var scoreItem = CreateScoreItem();

			if (weWon)
			{
				scoreItem.OurScore = score;
			}
			else
			{
				scoreItem.TheirScore = score;
			}

			if (IsBeloteOurs)
			{
				scoreItem.OurScore += 20;
			}

			if (IsBeloteTheirs)
			{
				scoreItem.TheirScore += 20;
			}

			SaveScoreAndRefreshTotals(scoreItem);
		}

		private void SaveScoreAndRefreshTotals(ScoreItemViewModel scoreItem)
		{
			History.Add(scoreItem);
			RefreshTotals();
		}

		private void RefreshTotals()
		{
			RaisePropertyChanged("OurTotal");
			RaisePropertyChanged("TheirTotal");
		}
		
		public void RegisterScoreItemPropertyChanged()
		{
			foreach (var scoreItem in _history)
			{
				scoreItem.PropertyChanged += scoreItem_PropertyChanged;
			}
		}
	}
}
