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

namespace TwilightImperium.ProgressTracker.Views
{
    public class ObjectiveSelectorVM : ChildViewModel<Game.Game>
    {
        public ObjectiveSelectorVM(Game.Game g,List<ObjectiveVM> existingObjectives, List<ObjectiveVM> publicObjectives) : base(g)
        {
            Objectives = new FilterableCollection<ObjectiveVM>(
                new ObservableCollection<ObjectiveVM>(g.AllObjectiveCards
                    .Where(e => (e.Stage ==0 || publicObjectives.Exists(p=>p.Model.Name == e.Name)) && !existingObjectives.Exists(ex => ex.Model.Name.Equals(e.Name)))
                    .Select(e =>
                    {
                        var ret = new ObjectiveVM(g, e);
                        ret.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == nameof(PlanetVM.IsSelected))
                                PropChanged(nameof(SelectedObjectives));
                        };
                        return ret;
                    }).OrderBy(e=>e.Model.Stage != 0).ThenBy(e=>e.Model.Stage).ThenBy(e=>e.Model.Name)),
                (vm, s) => vm.Model.Name.Trim().Replace(" ", "").StartsWith(s.Trim().Replace(" ", ""),
                    StringComparison.CurrentCultureIgnoreCase), new ObjectiveVMComparer());
        }

        public FilterableCollection<ObjectiveVM> Objectives { get; }

        public int SelectedObjectives => Objectives.AllItems.Count(e => e.IsSelected);

        public ObjectiveCard[] GetSelectedObjectives()
        {
            return Objectives.AllItems.Where(e => e.IsSelected).Select(e => e.Model).ToArray();
        }

        public ICommand SelectAllCommand => new DelegateCommand(() =>
        {
            foreach (var pl in Objectives.FilteredItems)
                pl.IsSelected = true;
        });
        public ICommand UnselectAllCommand => new DelegateCommand(() =>
        {
            foreach (var p in Objectives.FilteredItems)
                p.IsSelected = false;
        });

    }
}
