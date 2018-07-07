using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightImperium.ProgressTracker.Game.EventParameters
{
    public class AssignPlanetsParameter
    {
        public string TargetUser { get; set; }
        public string[] TargetPlanets { get; set; }
    }
}
