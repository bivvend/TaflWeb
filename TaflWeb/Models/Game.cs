using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaflWeb.Model.AI;
using TaflWeb.Model.Classes;
using static TaflWeb.Models.Classes.TurnDefinitions;

namespace TaflWeb.Models
{
    public class Game : IGame
    {
        private string test { get; set; }

        private Sage _sage;
        public Sage sage   //Sage evalutes the move tree
        {
            get
            {
                return _sage;
            }
            set
            {
                _sage = value;
            }
        }

        private SimpleBoard _baseBoard;
        public SimpleBoard baseBoard
        {
            get
            {
                return _baseBoard;
            }
            set
            {
                _baseBoard = value;
            }
        }

        private TurnState _currentTurnState;
        public TurnState currentTurnState
        {
            get
            {
                return _currentTurnState;
            }
            set
            {
                _currentTurnState = value;

            }

        }


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

        private List<Move> _moveHistory;
        public List<Move> moveHistory
        {
            get
            {
                return _moveHistory;
            }
            set
            {
                _moveHistory = value;
            }
        }

   
        public Game()
        {
            test = "Hello from API.";
        }
        public void SetString(string value)
        {
            test = value;
        }

        public string GetString()
        {
            return test;
        }

        async public Task<Move> RunAITurn(SimpleBoard startBoard)
        {
            Move move = new Move();
            return move;
        }
    }
}