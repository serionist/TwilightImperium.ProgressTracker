using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwilightImperium.ProgressTracker.Game;

namespace TwilightImperium.ProgressTracker.Views.Game
{
    public class ObjectiveVM : ChildViewModel<Views.Game.Game>
    {
        private bool _isSelected;

        public ObjectiveVM(Views.Game.Game vm, ObjectiveCard model) : base(vm)
        {
            Model = model;

        }

        public ObjectiveCard Model { get; }

        public UserVM[] CompletedBy => Parent.Users
            .Where(user => user.FinishedObjectives.Any(e => e.Model.Name == Model.Name)).ToArray();

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                ThisPropChanged();
            }
        }
    }
    public class ObjectiveVMComparer : IComparer<ObjectiveVM>
    {
        public int Compare(ObjectiveVM x, ObjectiveVM y)
        {
            return StringComparer.CurrentCultureIgnoreCase.Compare(x.Model.Name, y.Model.Name);
        }
    }
}
