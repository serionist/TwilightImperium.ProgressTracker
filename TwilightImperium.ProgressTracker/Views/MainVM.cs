using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TwilightImperium.ProgressTracker.Views
{
    public class MainVM:ViewModel
    {
        private int _numberOfPlayers;
        private Game.Game _currentGame;

        public MainVM()
        {
            NumberOfPlayers = 3;
        }
        public Game.Game CurrentGame
        {
            get => _currentGame;
            set
            {
                _currentGame = value;
                ThisPropChanged();
            }
        }

        public int NumberOfPlayers
        {
            get => _numberOfPlayers;
            set
            {
                _numberOfPlayers = value;
                ThisPropChanged();
                if (Usernames.Count < value)
                    for (int i = Usernames.Count;i<value;i++)
                        Usernames.Add(new UserNameVM(this));
                if (Usernames.Count > value)
                    for (int i = Usernames.Count - 1; i >= value; i--)
                        Usernames.RemoveAt(i);
                PropChanged(nameof(CanStartNewGame));
            }
        }

        public ObservableCollection<UserNameVM> Usernames { get; }= new ObservableCollection<UserNameVM>();

        public bool CanStartNewGame => Usernames.All(e => !string.IsNullOrEmpty(e.UserName)) && Usernames.Count == Usernames.Select(e=>e.UserName).Distinct(StringComparer.CurrentCultureIgnoreCase).Count();
        public ICommand NewGameCommand => new DelegateCommand(()=>CurrentGame= Controller.I.NewGame(Usernames.Select(e=>e.UserName).ToArray()));

    }

    public class UserNameVM : ChildViewModel<MainVM>
    {
        private string _userName;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                ThisPropChanged();
                Parent.PropChanged(nameof(MainVM.CanStartNewGame));
            }
        }

        public UserNameVM(MainVM parent) : base(parent)
        {
        }
    }
}
