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

        string GetBoardAsJson();

        string SquareClickResponse(int column, int row); // Return infomation to client regarding result of click.

        TurnState currentTurnState { get; set; }

        Task<string> RunAI(int turnState); ///runs the AI and returns a board state

        bool attackerIsAI { get; set; }

        bool defenderIsAI { get; set; }

        bool requestReDraw { get; set; }

        string responseText { get; set; }

        List<Move> moveHistory { get; set; }

        string GetString();


    }

}
