using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using TwilightImperium.ProgressTracker.Common;
using TwilightImperium.ProgressTracker.Game;

namespace TwilightImperium.ProgressTracker.Views.Game
{
    public class UserVM:ChildViewModel<Game>
    {
        public UserVM(Game parent, string name, Color Color) : base(parent)
        {
            Name = name;
            this.Color = new SolidColorBrush(Color);
            Planets = new FilterableCollection<PlanetVM>(new ObservableCollection<PlanetVM>(),
                (vm, s) => vm.Model.Name.Trim().Replace(" ", "").StartsWith(s.Trim().Replace(" ", ""),
                    StringComparison.CurrentCultureIgnoreCase), new PlanetVMComparer());
            Planets.AllItems.CollectionChanged += (sender, args) =>
            {
                PropChanged(nameof(PlanetsCount), nameof(ResourceString), nameof(InfluenceString), nameof(SelectedString));
            };
            Planets.PropertyChanged += (sender, args) =>
            {
                PropChanged(nameof(ResourceString), nameof(InfluenceString),nameof(SelectedString));
            };
            FinishedObjectives.CollectionChanged += (sender, args) =>
            {
                PropChanged(nameof(VPstring));
            };
        }

        public string Name { get;  }
        public SolidColorBrush Color { get; }

        public FilterableCollection<PlanetVM> Planets { get; }

        public ObservableCollection<ObjectiveVM> FinishedObjectives { get; } = new ObservableCollection<ObjectiveVM>();

        public int PlanetsCount => Planets.AllItems.Count;

        public string ResourceString => $"{AllResource} ({RemainingResource})";
        public int AllResource => Planets.AllItems.Sum(e => e.Model.Resource);
        public int RemainingResource => Planets.AllItems.Where(e => !e.IsExhausted).Sum(e => e.Model.Resource);
        public int SelectedResource => Planets.FilteredItems.Where(e => e.IsSelected && !e.IsExhausted).Sum(e => e.Model.Resource);
        public string InfluenceString => $"{AllInfluence} ({RemainingInfluence})";
        public int AllInfluence => Planets.AllItems.Sum(e => e.Model.Influence);
        public int RemainingInfluence => Planets.AllItems.Where(e => !e.IsExhausted).Sum(e => e.Model.Influence);
        public int SelectedInfluence => Planets.AllItems.Where(e => e.IsSelected && !e.IsExhausted).Sum(e => e.Model.Influence);
        public string SelectedString => $"Selected R: {SelectedResource} I: {SelectedInfluence}";
        public int VP => FinishedObjectives.Sum(e => e.Model.VictoryPoints);
        public string VPstring => $" VP: {VP}";
        public bool CanExhaustPlanet => Planets.AllItems.Any(e => e.IsSelected && !e.IsExhausted);

        public ICommand AssignPlanetCommand => new DelegateCommand(()=>Controller.I.AssignPlanets(this.Parent));

        public ICommand ExhaustSelectedPlanetsCommand => new DelegateCommand(() => Controller.I.ExhaustPlanets(this.Parent));

        public ICommand SelectAllPlanets => new DelegateCommand(() =>
        {
            foreach (var pl in Planets.FilteredItems)
                pl.IsSelected = true;
        });
        public ICommand UnselectAllPlanets => new DelegateCommand(()=>
        {
            foreach (var p in Planets.FilteredItems.Where(e => e.IsSelected && !e.IsExhausted))
                p.IsSelected = false;
        });

        public ICommand AssignObjectives => new DelegateCommand(() => Controller.I.AssignObjectives(this.Parent));


    }
}
