using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using TwilightImperium.ProgressTracker.Game;
using TwilightImperium.ProgressTracker.Game.EventParameters;
using TwilightImperium.ProgressTracker.GameEvents;
using TwilightImperium.ProgressTracker.Views;
using TwilightImperium.ProgressTracker.Views.Controls;

namespace TwilightImperium.ProgressTracker
{
    public sealed class Controller
    {
        private static Controller _c = null;
        public static Controller I => (_c ?? (_c = new Controller()));
        private Controller() { }

        public PlanetCard[] PlanetCards { get; private set; }
        public ObjectiveCard[] ObjectiveCards { get; private set; }
        public void InitApp()
        {
            if (!getPlanetCards()) Environment.Exit(1);
            if (!getObjectiveCards()) Environment.Exit(1);

            new MainWindow().ShowDialog();
        }

        private bool getPlanetCards()
        {
            try
            {
                var cards = new List<PlanetCard>();
                using (var reader = new StreamReader("Planets.txt"))
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                            continue;
                        var values = line.Split(',');
                        if (values.Length < 5)
                            throw new Exception($"Invalid record: '{line}'. Not enough columns");
                        if (cards.Exists(e=>e.Name.Equals(values[0], StringComparison.CurrentCultureIgnoreCase)))
                            throw new Exception($"Invalid record: '{line}'. Duplicate name");
                        try
                        {
                            cards.Add(new PlanetCard
                            {
                                Name = values[0],
                                Type = (PlanetType) Enum.Parse(typeof(PlanetType), values[1]),
                                Technology = (PlanetTechnology) Enum.Parse(typeof(PlanetTechnology), values[2]),
                                Influence = int.Parse(values[3]),
                                Resource = int.Parse(values[4]),
                                Description = values.ElementAtOrDefault(5)
                            });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Invalid record: '{line}'. Parse error.\r\n{ex}");
                        }
                    }

                PlanetCards = cards.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse Planetcards. Details:\r\n{ex}");
                return false;
            }

            return true;
        }
        private bool getObjectiveCards()
        {
            try
            {
                var cards = new List<ObjectiveCard>();
                using (var reader = new StreamReader("Objectives.txt"))
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                            continue;
                        var values = line.Split(',');
                        if (values.Length < 3)
                            throw new Exception($"Invalid record: '{line}'. Not enough columns");
                        if (cards.Exists(e => e.Name.Equals(values[0], StringComparison.CurrentCultureIgnoreCase)))
                            throw new Exception($"Invalid record: '{line}'. Duplicate name");
                        try
                        {
                            cards.Add(new ObjectiveCard()
                            {
                                Name = values[0],
                                Stage = int.Parse(values[1]),
                                VictoryPoints = int.Parse(values[2]),
                                Description = values.ElementAtOrDefault(3)
                            });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Invalid record: '{line}'. Parse error.\r\n{ex}");
                        }
                    }

                ObjectiveCards = cards.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse Objectivecards. Details:\r\n{ex}");
                return false;
            }

            return true;
        }


        public Views.Game.Game NewGame(string[] userNames)
        {
            var ret = new Views.Game.Game();
            var ev = new GameEvent()
            {
                Type = GameEventType.GameStart,
                Timestamp = DateTime.UtcNow,
                Gametime = TimeSpan.Zero,
                Parameters = JObject.FromObject(new GameStartParameter()
                {
                    PlanetCards = PlanetCards,
                    ObjectiveCards = ObjectiveCards,
                    UserNames = userNames
                })
            };
            ret.UpdateWithLogs(ev);
            return ret;
        }

        public void ToggleTimer(Views.Game.Game g)
        {
            var ev = new GameEvent()
            {
                Type = g.IsGameRunning?GameEventType.TimerStop:GameEventType.TimerStart,
                Timestamp = DateTime.UtcNow,
                Gametime = g.TimeElapsed,
            };
            g.UpdateWithLogs(ev);
        }

        public void AssignPlanets(Views.Game.Game g)
        {
            var window = new PlanetSelectorWindow(new PlanetSelectorVM(g, false, false));
            window.ShowDialog();
            if (window.ReturnCards == null)
                return;
            var allUserPlanets = g.Users.SelectMany(e => e.Planets.AllItems).ToList();
            var existingOccupations = window.ReturnCards.Where(e =>
                    allUserPlanets.Exists(u => u.Model.Name.Equals(e.Name, StringComparison.CurrentCultureIgnoreCase)))
                .ToList();
            if (existingOccupations.Any())
                if (MessageBox.Show(
                        string.Join("\r\n",
                            existingOccupations.Select(e =>
                                $"{e.Name} belongs to {g.Users.First(u => u.Planets.AllItems.Any(p => p.Model.Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase))).Name}")) + "\r\nAre you sure to continue?",
                        "The following planets are already occupied.", MessageBoxButton.YesNo) == MessageBoxResult.No)                    
                    return;
            var ev = new GameEvent()
            {
                Type = GameEventType.AssignPlanets,
                Timestamp = DateTime.UtcNow,
                Gametime = g.TimeElapsed,
                Parameters = JToken.FromObject(new AssignPlanetsParameter()
                {
                    TargetUser = g.SelectedUser.Name,
                    TargetPlanets = window.ReturnCards.Select(e=>e.Name).ToArray()
                    
                })
            };
            g.UpdateWithLogs(ev);
        }

        public void RefreshPlanets(Views.Game.Game g)
        {

            var window = new PlanetSelectorWindow(new PlanetSelectorVM(g, true, true));
            window.ShowDialog();
            if (window.ReturnCards?.Any() != true)
                return;

            var ev = new GameEvent()
            {
                Type = GameEventType.RefreshPlanets,
                Timestamp = DateTime.UtcNow,
                Gametime = g.TimeElapsed,
                Parameters = JToken.FromObject(window.ReturnCards.Select(e=>e.Name))
            };
            g.UpdateWithLogs(ev);
        }

        public void ExhaustPlanets(Views.Game.Game g)
        {
            var models = g.SelectedUser.Planets.AllItems.Where(e => e.IsSelected).ToArray();
            if (!models.Any())
                return;
            var ev = new GameEvent()
            {
                Type = GameEventType.ExhaustPlanets,
                Timestamp = DateTime.UtcNow,
                Gametime = g.TimeElapsed,
                Parameters = JToken.FromObject(models.Select(e=>e.Model.Name))
            };
            g.UpdateWithLogs(ev);
        }
    }
}
