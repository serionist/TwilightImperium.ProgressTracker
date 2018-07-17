using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightImperium.ProgressTracker.Game
{
    public enum PlanetType
    {
        Cultural,
        Hazardous,
        Industrial,
        Homeplanet,
        MecatolRex
    }


    public enum PlanetTechnology
    {
        Blue,
        Yellow,
        Red,
        Green,
        None
    }
    public class PlanetCard
    {
        public string Name { get; set; }
        public PlanetType Type { get; set; }
        public PlanetTechnology Technology { get; set; }

        public int Influence { get; set; }
        public int Resource { get; set; }
        public string Description { get; set; }

    }
}
