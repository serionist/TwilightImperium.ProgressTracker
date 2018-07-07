using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TwilightImperium.ProgressTracker.GameEvents
{
    public enum GameEventType
    {
        GameStart,
        TimerStart,
        TimerStop,
        SetObjective,
        AssignPlanets,
        RefreshPlanets,
        ExhaustPlanets
    }

    public class GameEvent
    {
        public GameEventType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Gametime { get; set; }

        public JToken Parameters { get; set; }
    }
}
