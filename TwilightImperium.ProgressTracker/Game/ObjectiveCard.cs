using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightImperium.ProgressTracker.Game
{
    public class ObjectiveCard
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stage { get; set; }
        public int VictoryPoints { get; set; }
    }
}
