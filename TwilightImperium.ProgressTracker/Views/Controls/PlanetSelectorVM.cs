using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TwilightImperium.ProgressTracker.Common;
using TwilightImperium.ProgressTracker.Game;
using TwilightImperium.ProgressTracker.Views.Game;

namespace TwilightImperium.ProgressTracker.Views.Controls
{
    public class PlanetSelectorVM:ChildViewModel<Game.Game>
    {
        public PlanetSelectorVM(Game.Game g, bool onlyBelongingToUsers, bool checkByDefault) : base(g)
        {
            Planets = new FilterableCollection<PlanetVM>(
                new ObservableCollection<PlanetVM>(g.AllPlanetCards
                    .Where(e => !onlyBelongingToUsers || g.Users.Any(us =>
                                    us.Planets.AllItems.ToList().Exists(pl =>
                                        pl.Model.Name.Equals(e.Name, StringComparison.CurrentCultureIgnoreCase))))
                    .Select(e =>
                    {
                        var ret = new PlanetVM(null, e);
                        ret.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == nameof(PlanetVM.IsSelected))
                                PropChanged(nameof(SelectedPlanets));
                        };
                        ret.IsSelected = checkByDefault;
                        return ret;
                    })),
                (vm, s) => vm.Model.Name.Trim().Replace(" ", "").StartsWith(s.Trim().Replace(" ", ""),
                    StringComparison.CurrentCultureIgnoreCase), new PlanetVMComparer());
        }

        public FilterableCollection<PlanetVM> Planets { get; }

        public int SelectedPlanets => Planets.AllItems.Count(e => e.IsSelected);

        public PlanetCard[] GetSelectedPlanets()
        {
            return Planets.AllItems.Where(e => e.IsSelected).Select(e => e.Model).ToArray();
        }
        
        public ICommand SelectAllCommand => new DelegateCommand(() =>
        {
            foreach (var pl in Planets.FilteredItems)
                pl.IsSelected = true;
        });
        public ICommand UnselectAllCommand => new DelegateCommand(() =>
        {
            foreach (var p in Planets.FilteredItems)
                p.IsSelected = false;
        });

    }
}
