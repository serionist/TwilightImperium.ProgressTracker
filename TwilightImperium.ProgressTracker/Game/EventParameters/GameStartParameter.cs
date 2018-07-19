using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TwilightImperium.ProgressTracker.Game.EventParameters
{
    public class GameStartParameter
    {
        public PlanetCard[] PlanetCards { get; set; }
        public ObjectiveCard[] ObjectiveCards { get; set; }

        public GameStartUser[] Users { get; set; }
        
    }

    public class GameStartUser
    {
        public string UserName { get; set; }
        public Color Color { get; set; }
    }
}
