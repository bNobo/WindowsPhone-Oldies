using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using bN.Core;
using Windows.Storage;
using System.IO;
using Windows.Foundation;
using bN.Coinchons.DataObjects;

namespace bN.Coinchons
{
	public class MainViewModel : BaseViewModel
	{
		[IgnoreDataMember]
		public int OurTotal { get { return History.Sum(x => x.OurScore); } }
		[IgnoreDataMember]
		public int TheirTotal { get { return History.Sum(x => x.TheirScore); } }
		#region INotifyPropertyChanged properties
		#region IsHearts

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
		#endregion
		#region IsDiamonds
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
		#endregion
		#region IsClubs

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
		#endregion
		#region IsSpades
 
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
		#endregion

		#region OurBid
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
		#endregion
		#region TheirBid
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
		#endregion

		#region DealerIsPlayer1
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
		#endregion
		#region DealerIsPlayer2
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
		#endregion
		#region DealerIsPlayer3
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
		#endregion
		#region DealerIsPlayer4
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
		#endregion
		#region IsBeloteOurs
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
		#endregion
		#region IsBeloteTheirs

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
		#endregion
		#region IsCoinche
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
		#endregion
		#region IsSurcoinche
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
		#endregion
		#region History
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

		#region TeamUsFirstPlayerNumber
		private int _teamUsFirstPlayerNumber;

		public int TeamUsFirstPlayerNumber
		{
			get { return _teamUsFirstPlayerNumber; }
			set
			{
				if (value != _teamUsFirstPlayerNumber)
				{
					_teamUsFirstPlayerNumber = value;
					RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region TeamUsSecondPlayerNumber
		private int _teamUsSecondPlayerNumber;

		public int TeamUsSecondPlayerNumber
		{
			get { return _teamUsSecondPlayerNumber; }
			set
			{
				if (value != _teamUsSecondPlayerNumber)
				{
					_teamUsSecondPlayerNumber = value;
					RaisePropertyChanged();
				}
			}
		} 
		#endregion

		private int _teamThemFirstPlayerNumber;

		public int TeamThemFirstPlayerNumber
		{
			get { return _teamThemFirstPlayerNumber; }
			set
			{
				if (value != _teamThemFirstPlayerNumber)
				{
					_teamThemFirstPlayerNumber = value;
					RaisePropertyChanged();
				}
			}
		}

		private int _teamThemSecondPlayerNumber;

		public int TeamThemSecondPlayerNumber
		{
			get { return _teamThemSecondPlayerNumber; }
			set
			{
				if (value != _teamThemSecondPlayerNumber)
				{
					_teamThemSecondPlayerNumber = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _teamUsFirstPlayerName;

		public string TeamUsFirstPlayerName
		{
			get { return _teamUsFirstPlayerName; }
			set
			{
				if (_teamUsFirstPlayerName != value)
				{
					_teamUsFirstPlayerName = value.Trim();
					RaisePropertyChanged();
					RaisePropertyChanged("FirstPlayerInitials");
					RaisePropertyChanged("SecondPlayerInitials");
				}
			}
		}

		private string _teamUsSecondPlayerName;

		public string TeamUsSecondPlayerName
		{
			get { return _teamUsSecondPlayerName; }
			set
			{
				if (value != _teamUsSecondPlayerName)
				{
					_teamUsSecondPlayerName = value.Trim();
					RaisePropertyChanged();
					RaisePropertyChanged("ThirdPlayerInitials");
					RaisePropertyChanged("FourthPlayerInitials");
				}
			}
		}

		private string _teamThemFirstPlayerName;

		public string TeamThemFirstPlayerName
		{
			get { return _teamThemFirstPlayerName; }
			set
			{
				if (value != _teamThemFirstPlayerName)
				{
					_teamThemFirstPlayerName = value.Trim();
					RaisePropertyChanged();
					RaisePropertyChanged("FirstPlayerInitials");
					RaisePropertyChanged("SecondPlayerInitials");
				}
			}
		}


		private string _teamThemSecondPlayerName;

		public string TeamThemSecondPlayerName
		{
			get { return _teamThemSecondPlayerName; }
			set
			{
				if (value != _teamThemSecondPlayerName)
				{
					_teamThemSecondPlayerName = value.Trim();
					RaisePropertyChanged();
					RaisePropertyChanged("ThirdPlayerInitials");
					RaisePropertyChanged("FourthPlayerInitials");
				}
			}
		}

		[IgnoreDataMember]
		public string FirstPlayerInitials
		{
			get
			{
				if (TeamUsFirstPlayerNumber == 1)
				{
					return GetInitials(TeamUsFirstPlayerName);
				}
				else
				{
					return GetInitials(TeamThemFirstPlayerName);
				}
			}
		}

		[IgnoreDataMember]
		public string SecondPlayerInitials
		{
			get
			{
				if (TeamUsFirstPlayerNumber == 1)
				{
					return GetInitials(TeamThemFirstPlayerName);
				}
				else
				{
					return GetInitials(TeamUsFirstPlayerName);
				}
			}
		}

		[IgnoreDataMember]
		public string ThirdPlayerInitials
		{
			get
			{
				if (TeamUsFirstPlayerNumber == 1)
				{
					return GetInitials(TeamUsSecondPlayerName);
				}
				else
				{
					return GetInitials(TeamThemSecondPlayerName);
				}
			}
		}

		[IgnoreDataMember]
		public string FourthPlayerInitials
		{
			get
			{
				if (TeamUsFirstPlayerNumber == 1)
				{
					return GetInitials(TeamThemSecondPlayerName);
				}
				else
				{
					return GetInitials(TeamUsSecondPlayerName);
				}
			}
		}


		private string GetInitials(string name)
		{
			var cleanName = name.Trim();
			return string.Concat(cleanName.FirstOrDefault(), cleanName.LastOrDefault());
		}

		private ObservableCollection<string> _nameList;

		[IgnoreDataMember]
		public ObservableCollection<string> NameList
		{
			get { return _nameList; }
			set { 
				_nameList = value;;
			}
		}

		private ObservableCollection<string> _filteredNameList;

		[IgnoreDataMember]
		public ObservableCollection<string> FilteredNameList
		{
			get { return _filteredNameList; }
			set { _filteredNameList = value;
			RaisePropertyChanged();
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
		[IgnoreDataMember]
		public bool IsDebug { get; set; }

		private string _player1;
		private string _player2; 
		private string _player3; 
		private string _player4;

		public MainViewModel()
		{
			_history = new ObservableCollection<ScoreItemViewModel>();
			DealerIsPlayer1 = true;
			CreateCommands();
			TeamUsFirstPlayerNumber = 1;
			TeamUsSecondPlayerNumber = 3;
			TeamThemFirstPlayerNumber = 2;
			TeamThemSecondPlayerNumber = 4;
			_player1 = TeamUsFirstPlayerName = ResourceHelper.GetString("Player1");
			_player2 = TeamThemFirstPlayerName = ResourceHelper.GetString("Player2");
			_player3 = TeamUsSecondPlayerName = ResourceHelper.GetString("Player3");
			_player4 = TeamThemSecondPlayerName = ResourceHelper.GetString("Player4");
			PropertyChanged += MainViewModel_PropertyChanged;
#if DEBUG
			IsDebug = true;
#endif
		}

		private const string _fileName = "NameList.json";

		public async Task DeleteNameListAsync()
		{
			try
			{
				var file = await ApplicationData.Current.RoamingFolder
					.GetFileAsync(_fileName);

				await file.DeleteAsync();
			}
			catch
			{

			}
		} 

		public async Task LoadNameListAsync()
		{
			try
			{
				var file = await ApplicationData.Current.RoamingFolder
					.CreateFileAsync(_fileName, CreationCollisionOption.OpenIfExists);

				using (var stream = await file.OpenStreamForReadAsync())
				{
					NameList = (ObservableCollection<string>)new DataContractJsonSerializer(
							typeof(ObservableCollection<string>)).ReadObject(stream);
				}
			}
			catch
			{
				
			}

			if (NameList == null)
			{
				NameList = new ObservableCollection<string>(); 
			}

			FilteredNameList = new ObservableCollection<string>(NameList.Take(4));
		}

		//private static IAsyncOperation<StorageFile> GetNameListFileAsync()
		//{
		//	return ApplicationData.Current.RoamingFolder
		//										   .GetFileAsync(_fileName);
			
		//}

		private void CreateCommands()
		{
			Done = new RelayCommand(async () => await DoDone(), CanDoDone);
			Fallen = new RelayCommand(async () => await DoFallen(), CanDoFallen);
			OurBidCommand = new RelayCommand(SelectOurBid);
			TheirBidCommand = new RelayCommand(SelectTheirBid);
		}

		public void SwapTeams()
		{
			if (TeamUsFirstPlayerNumber == 1)
			{
				TeamUsFirstPlayerNumber = 2;
				TeamUsSecondPlayerNumber = 4;
				TeamThemFirstPlayerNumber = 1;
				TeamThemSecondPlayerNumber = 3;
			}
			else
			{
				TeamUsFirstPlayerNumber = 1;
				TeamUsSecondPlayerNumber = 3;
				TeamThemFirstPlayerNumber = 2;
				TeamThemSecondPlayerNumber = 4;
			}

			RaisePropertyChanged("FirstPlayerInitials");
			RaisePropertyChanged("SecondPlayerInitials");
			RaisePropertyChanged("ThirdPlayerInitials");
			RaisePropertyChanged("FourthPlayerInitials");
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

		private Task DoFallen()
		{
			CalculateFallenScore();
			SelectNextDealer();
			ResetBid();
			return EndGameAsync();
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

        public void NewGame()
        {
            _history.Clear();
            RefreshTotals();
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

		private Task DoDone()
		{
			CalculateDoneScore();
			SelectNextDealer();
			ResetBid();
			return EndGameAsync();
		}

		private async Task EndGameAsync()
		{
			if (OurTotal >= 1000 || TheirTotal >= 1000)
			{
				TodoItem item = new TodoItem
				{
					Text = "Awesome item",
					Complete = false
				};

				await App.MobileService.GetTable<TodoItem>().InsertAsync(item);//DataContext toto;
				var utilisateurTable = App.MobileService.GetTable<Utilisateur>();
				
				var utilisateur = utilisateurTable.Where(x => x.LiveId == "test")
					.Query.SingleOrDefault();

				if (utilisateur == null)
				{
					utilisateur = new Utilisateur
							{
								LiveId = "test"
							};

					await utilisateurTable.InsertAsync(utilisateur);
				}

				var joueur1 = new Joueur { Nom = TeamUsFirstPlayerName };
				var joueur2 = new Joueur { Nom = TeamUsSecondPlayerName };
				var joueur3 = new Joueur { Nom = TeamThemFirstPlayerName };
				var joueur4 = new Joueur { Nom = TeamThemSecondPlayerName };

				utilisateur.Joueurs.Add(joueur1);
				utilisateur.Joueurs.Add(joueur2);
				utilisateur.Joueurs.Add(joueur3);
				utilisateur.Joueurs.Add(joueur4);

				await utilisateurTable.UpdateAsync(utilisateur);
			}
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

		private void AddNameToList(string name)
		{
			var cleanName = name.Trim();

			if (string.IsNullOrEmpty(cleanName) || cleanName.Length < 2)
			{
				return;
			}

			if (!NameList.Contains(cleanName) && cleanName != _player1 && cleanName != _player2
				&& cleanName != _player3 && cleanName != _player4)
			{
				NameList.Add(cleanName);
			}
		}

		public void FilterNameList(string filter)
		{
			var cleanFilter = filter.Trim();

			FilteredNameList = new ObservableCollection<string>(
				NameList.Where(name => name != TeamUsFirstPlayerName
					&& name != TeamUsSecondPlayerName
					&& name != TeamThemFirstPlayerName
					&& name != TeamThemSecondPlayerName
					&& name.StartsWith(cleanFilter))
					.OrderBy(x => x));
		}
        
        public void UpdateNameList()
        {
            AddNameToList(TeamUsFirstPlayerName);
            AddNameToList(TeamUsSecondPlayerName);
            AddNameToList(TeamThemFirstPlayerName);
            AddNameToList(TeamThemSecondPlayerName);
        }

		public async Task SaveNameList()
		{
			var memoryStream = new MemoryStream();
			var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<string>));
			serializer.WriteObject(memoryStream, NameList);

			var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(
				_fileName, CreationCollisionOption.ReplaceExisting);

			using (Stream fileStream = await file.OpenStreamForWriteAsync())
			{
				memoryStream.Seek(0, SeekOrigin.Begin);
				await memoryStream.CopyToAsync(fileStream);
			}
		}
	}
}
