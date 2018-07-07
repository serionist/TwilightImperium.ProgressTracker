using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TwilightImperium.ProgressTracker.Game;
using TwilightImperium.ProgressTracker.Game.EventParameters;
using TwilightImperium.ProgressTracker.GameEvents;

namespace TwilightImperium.ProgressTracker.Views.Game
{
    public class Game:ViewModel, IDisposable
    {
        private readonly List<GameEvent> _gameEvents = new List<GameEvent>();
        private GameEvent latestTimerEvent = null;
        private readonly CancellationTokenSource _timerCancelToken = new CancellationTokenSource();
        private bool _isGameRunning;
        private TimeSpan _timeElapsed;
        private UserVM[] _users;
        private UserVM _selectedUser;

        public Game()
        {
            startTimerLoop();
        }
        private void startTimerLoop()
        {
            Task.Factory.StartNew(async() =>
            {
                while (!_timerCancelToken.IsCancellationRequested)
                {
                    if (latestTimerEvent != null)
                        if (latestTimerEvent.Type == GameEventType.TimerStop || latestTimerEvent.Type == GameEventType.GameStart)
                            TimeElapsed = latestTimerEvent.Gametime;
                        else if (latestTimerEvent.Type == GameEventType.TimerStart)
                            TimeElapsed = DateTime.UtcNow.Subtract(latestTimerEvent.Timestamp).Add(latestTimerEvent.Gametime);
                    await Task.Delay(100, _timerCancelToken.Token);

                }
            }, TaskCreationOptions.LongRunning);
        }


        public ObjectiveCard[] AllObjectiveCards { get; set; }
        public PlanetCard[] AllPlanetCards { get; set; }

        public bool IsGameRunning
        {
            get => _isGameRunning;
            set
            {
                _isGameRunning = value;
                ThisPropChanged();
            }
        }

        public TimeSpan TimeElapsed
        {
            get => _timeElapsed;
            set
            {
                _timeElapsed = value;
                ThisPropChanged();
            }
        }
        

        public ICommand ToggleGameRunningCommand => new DelegateCommand(()=>Controller.I.ToggleTimer(this));

        public ICommand ClearPlanetsCommand => new DelegateCommand(()=> Controller.I.RefreshPlanets(this));


        public ObservableCollection<GameEventVM> GameLog { get; set; } = new ObservableCollection<GameEventVM>();

        public UserVM[] Users
        {
            get => _users;
            set
            {
                _users = value;
                ThisPropChanged();
            }
        }

        public UserVM SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                ThisPropChanged();
            }
        }


        public void UpdateWithLogs(params GameEvent[] events)
        {
            foreach (var ev in events)
            {
                _gameEvents.Add(ev);

                var eventVM = new GameEventVM(this, ev);
                switch (ev.Type)
                {
                    case GameEventType.GameStart:
                    {
                        if (latestTimerEvent == null || latestTimerEvent.Timestamp < ev.Timestamp)
                            latestTimerEvent = ev;
                            var p = ev.Parameters.ToObject<GameStartParameter>();
                        AllObjectiveCards = p.ObjectiveCards;
                        AllPlanetCards = p.PlanetCards;
                        Users = p.UserNames.Select(e => new UserVM(this, e)).ToArray();
                        eventVM.Title = "Game started";
                        eventVM.Description = $"Users: {Users.Length}";
                        IsGameRunning = false;
                        }
                        break;
                    case GameEventType.TimerStart:
                        if (latestTimerEvent == null || latestTimerEvent.Timestamp < ev.Timestamp)
                            latestTimerEvent = ev;
                        IsGameRunning = true;

                        //eventVM = null;
                        eventVM.Description = "Timer started";
                        break;
                    case GameEventType.TimerStop:
                        if (latestTimerEvent == null || latestTimerEvent.Timestamp < ev.Timestamp)
                            latestTimerEvent = ev;
                        IsGameRunning = false;
                        //eventVM = null;
                        eventVM.Description = "Timer stopped";
                        break;
                    case GameEventType.SetObjective:
                        break;
                    case GameEventType.AssignPlanets:
                    {
                        var p = ev.Parameters.ToObject<AssignPlanetsParameter>();
                        var targetUser = Users.First(e => e.Name.Equals(p.TargetUser));
                        foreach (var planetName in p.TargetPlanets)
                        {
                            var planet = AllPlanetCards.First(e =>
                                e.Name.Equals(planetName, StringComparison.CurrentCultureIgnoreCase));

                            foreach (var user in Users.Where(e=>!e.Name.Equals(p.TargetUser)))
                            {
                                var ex = user.Planets.AllItems.FirstOrDefault(e =>
                                    e.Model.Name.Equals(planet.Name, StringComparison.CurrentCultureIgnoreCase));
                                if (ex != null)
                                    user.Planets.AllItems.Remove(ex);
                            }
                            if (targetUser.Planets.AllItems.FirstOrDefault(e =>
                                e.Model.Name.Equals(planet.Name, StringComparison.CurrentCultureIgnoreCase)) == null)
                                targetUser.Planets.AllItems.Add(new PlanetVM(targetUser, planet));
                        }

                        eventVM.Title = $"Planets assigned to {p.TargetUser}";
                        eventVM.Description = $"Planets: {string.Join(",", p.TargetPlanets)}";

                    }
                        break;
                    case GameEventType.RefreshPlanets:
                    {
                        var planetNames = ev.Parameters.ToObject<string[]>();
                        foreach (var u in Users)
                        foreach (var pl in u.Planets.AllItems)
                            if (pl.IsExhausted && planetNames.Contains(pl.Model.Name, StringComparer.CurrentCultureIgnoreCase))
                                pl.IsExhausted = pl.IsSelected = false;
                        eventVM.Title = $"Planets refreshed";
                        eventVM.Description = $"Planets: {string.Join(",", planetNames)}";
                        }
                        break;
                    case GameEventType.ExhaustPlanets:
                    {
                        var planetNames = ev.Parameters.ToObject<string[]>();
                        foreach (var u in Users)
                        foreach (var pl in u.Planets.AllItems)
                            if (!pl.IsExhausted && planetNames.Contains(pl.Model.Name, StringComparer.CurrentCultureIgnoreCase))
                                pl.IsExhausted = pl.IsSelected = true;
                        eventVM.Title = $"Planets exhausted";
                        eventVM.Description = $"Planets: {string.Join(",", planetNames)}";
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                        break;
                }
                if (eventVM != null)
                    GameLog.Insert(0, eventVM);
            }
        }

        public void DeleteLog(GameEventVM vm)
        {
            if (vm.Event.Type == GameEventType.GameStart)
                return;
            var selectedUserName = SelectedUser?.Name;
            _gameEvents.Remove(vm.Event);
            var events = _gameEvents.ToArray();
            _gameEvents.Clear();
            GameLog.Clear();
            latestTimerEvent = null;
            UpdateWithLogs(events);
            SelectedUser = selectedUserName != null ? Users.FirstOrDefault(e => e.Name == selectedUserName) : null;
        }
        public void Dispose()
        {
            _timerCancelToken.Cancel();
        }
    }
}
