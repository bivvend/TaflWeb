using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaflWeb.Model.AI;
using TaflWeb.Model.Classes;
using TaflWeb.Models.Classes;
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

        private BoardModel _board;
        public BoardModel board
        {
            get
            {
                return _board;
            }
            set
            {
                _board = value;
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
            board = new BoardModel(11, 11);
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
            await Task.Delay(200);
            return move;
        }
        /// <summary>
        /// Returns the situation of the board (tile types and occupations)
        /// </summary>
        /// <returns></returns>
        async public Task<string> GetBoardAsJson()
        {
            string boardString = await Task<string>.Factory.StartNew(()=>board.GetSimpleBoardAsJSON());
            return boardString;
        }
        /// <summary>
        /// Returns the image number within the set of similar images to give visual diversity to board
        /// </summary>
        /// <returns></returns>
        async public Task<string> GetBoardPatternAsJSON()
        {
            string boardViewString = await Task<string>.Factory.StartNew(() => board.GetBoardPatternAsJSON());
            return boardViewString;
        }

        async public Task<string> GetSelectionsAndHighlightsAsJSON()
        {
            string boardSelectionString = await Task<string>.Factory.StartNew(() => board.GetSelectionsAndHighlightsAsJSON());
            return boardSelectionString;
        }
        
        public string GetPlayStateAsJson()
        {
            GameStateTransferObject state = new GameStateTransferObject() { attackerIsAI = this.attackerIsAI, defenderIsAI = this.defenderIsAI, turnState = this.currentTurnState };
            string json = JsonConvert.SerializeObject(state);
            return json;
        }

        public string SquareClickResponse(int column, int row)
        {

            return "hello";
        }


    }
}