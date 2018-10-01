using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaflWeb.Model.AI;
using TaflWeb.Model.Classes;
using static TaflWeb.Models.Classes.TurnDefinitions;
using static TaflWeb.Models.Game;

namespace TaflWeb.Models
{
    public interface IGame
    {
        Sage sage { get; set; }

        BoardModel board { get; set; }

        Task<string> GetBoardAsJson(); //returns functional description as JSON

        Task<string> GetBoardPatternAsJSON();  //returns the initial randomised view for visual interest.

        TurnState currentTurnState { get; set; }

        bool attackerIsAI { get; set; }

        bool defenderIsAI { get; set; }

        List<Move> moveHistory { get; set; }

        string GetString();

        void SetString(string value);

        Task<Move> RunAITurn(SimpleBoard startBoard);

    }

}
