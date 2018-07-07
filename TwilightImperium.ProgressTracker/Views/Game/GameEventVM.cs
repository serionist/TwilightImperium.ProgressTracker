using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TwilightImperium.ProgressTracker.GameEvents;

namespace TwilightImperium.ProgressTracker.Views.Game
{
    public class GameEventVM:ChildViewModel<Game>
    {
        public GameEvent Event { get; }
        public GameEventVM(Game parent, GameEvent @event) : base(parent)
        {
            Event = @event;
        }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime Created => Event.Timestamp;
        public TimeSpan Gametime => Event.Gametime;
        public ICommand DeleteLog => new DelegateCommand(()=>Parent.DeleteLog(this));
    }
}
