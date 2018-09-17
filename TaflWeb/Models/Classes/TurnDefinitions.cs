using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaflWeb.Models.Classes
{
    public class TurnDefinitions
    {
        public enum TurnState
        {
            Attacker, Defender, VictoryDefender, VictoryAttacker, Resetting
        };
    }
}
