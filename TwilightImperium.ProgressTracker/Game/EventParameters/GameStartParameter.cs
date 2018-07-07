using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightImperium.ProgressTracker.Game.EventParameters
{
    public class GameStartParameter
    {
        public PlanetCard[] PlanetCards { get; set; }
        public ObjectiveCard[] ObjectiveCards { get; set; }

        public string[] UserNames { get; set; }


    }
}
