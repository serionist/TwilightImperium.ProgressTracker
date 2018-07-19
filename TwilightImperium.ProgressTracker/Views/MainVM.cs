using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

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

        public SolidColorBrush[] AppColors => Controller.I.Colors.Select(e => new SolidColorBrush(e)).ToArray();
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

        public bool CanStartNewGame => Usernames.All(e => !string.IsNullOrEmpty(e.UserName) && e.Color != null) && Usernames.Count == Usernames.Select(e=>e.UserName).Distinct(StringComparer.CurrentCultureIgnoreCase).Count()
                && Usernames.Count == Usernames.Select(e=>e.Color.Color).Distinct().Count();
        


        public ICommand NewGameCommand => new DelegateCommand(()=>CurrentGame= Controller.I.NewGame(this, Usernames.Select(e=>e.UserName).ToArray()));

        public ICommand LoadGameCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var ofd = new OpenFileDialog();
                    var startDir = Properties.Settings.Default.LastSaveDir;
                    if (string.IsNullOrEmpty(startDir))
                        startDir = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                    ofd.InitialDirectory = startDir;
                    ofd.Filter = $"Twilight Progress files|*.{Controller.I.SaveFileExt}|All files|*";
                    ofd.DefaultExt = Controller.I.SaveFileExt;
                    if (ofd.ShowDialog() != true)
                        return;
                    try
                    {
                        CurrentGame = Controller.I.LoadGame(this, ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load game\r\n{ex}");
                    }
                });
            }
        }


    }

    public class UserNameVM : ChildViewModel<MainVM>
    {
        private string _userName;
        private SolidColorBrush _color = null;

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

        public SolidColorBrush Color
        {
            get => _color; set
            {
                
                _color = value;
                ThisPropChanged();
                Parent.PropChanged(nameof(MainVM.CanStartNewGame));
            }
        }

        public ICommand SelectColor => new DelegateCommand((o)=> Color = (SolidColorBrush)o);
        public UserNameVM(MainVM parent) : base(parent)
        {
        }
    }
}
