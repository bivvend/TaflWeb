using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TaflWeb.Models.Classes.TurnDefinitions;

namespace TaflWeb.Models.Classes
{
    public class GameStateTransferObject
    {
        private bool _attackerIsAI;
        public bool attackerIsAI
        {
            get
            {
                return _attackerIsAI;
            }
            set
            {
                _attackerIsAI = value;
            }
        }

        private bool _defenderIsAI;
        public bool defenderIsAI
        {
            get
            {
                return _defenderIsAI;
            }
            set
            {
                _defenderIsAI = value;
            }
        }

        private TurnState _turnState;
        public TurnState turnState
        {
            get
            {
                return _turnState;
            }
            set
            {
                _turnState = value;
            }
        }



    }
}
