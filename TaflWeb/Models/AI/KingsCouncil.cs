using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaflWeb.Model.Classes;
using static TaflWeb.Models.Classes.TurnDefinitions;

namespace TaflWeb.Model.AI
{
    /// <summary>
    /// Evaluator plans best route for King to get to Exit
    /// </summary>
    /// 
    public class KingsCouncil
    {
        private List<List<Move>> moveList;

        public double desireToWin = 1000000000.0;
        public double desrieToWinDepth2 = 1000.0;
        public double desireForFreeKing = 0.1;
        public double desireForKingToBeClearToCorner = 10.0;

        public List<Move> Evaluate(List<List<Move>> inputMoveList, TurnState currentTurnState)
        {
            moveList = inputMoveList;

            List<Move> suggestedMoves = new List<Move>();

            if (currentTurnState == TurnState.Defender)
            {
                SimpleSquare evalSquare = new SimpleSquare();
                
                bool foundWin = false;
                //Evaluate the if the King's path to the thrones is clear.  If so one of the moves will have the King on the throne. 
                moveList[0].ForEach((item) =>
                {
                    evalSquare = item.GetSquare(item.endRow, item.endColumn);
                    if(evalSquare.SquareType == Square.square_type.Corner)
                    {
                        item.scoreKingsCouncil = desireToWin;
                        suggestedMoves.Add(item);
                        foundWin = true;
                    }
                });
                if (foundWin)
                    return suggestedMoves;                

                int numMovesForKing = 0;
                List<Move> tempMoveList = new List<Move>();
                List<Move> kingsMoveList = new List<Move>();
                List<Move> winningKingMoveList = new List<Move>();
                SimpleSquare kingSquare = new SimpleSquare();
                int numberDepth2Wins = 0;

                int sizeX = moveList[0][0].board.OccupationArray.GetLength(0) - 1;
                int sizeY = moveList[0][0].board.OccupationArray.GetLength(1) - 1;

                //Look at all depth 2 moves (generated from move[0]'s) to see if any of these are wins. Don't just iterate over all depth[2] as this would
                //include depth [1] moves by attacker moving out of the way...
                //Also Award moves that have the King more free to move 
                moveList[0].ForEach((item) =>
                {
                    try
                    {
                        //Find the King
                        kingSquare = item.FindTheKing(item.board, true);

                        tempMoveList = item.board.GetPossibleMoves(TurnState.Defender, null, 0);

                        //filter the list based on moves of the king                        
                        kingsMoveList = tempMoveList.Where(move => move.startRow == kingSquare.Row && move.startColumn == kingSquare.Column).ToList();
                        numMovesForKing = kingsMoveList.Count;
                        //Give a bonus for the king being freed up 
                        item.scoreKingsCouncil += (double)numMovesForKing * desireForFreeKing;

                        winningKingMoveList = kingsMoveList.Where(mov => (mov.endColumn == 0 && mov.endRow == 0)
                                        || (mov.endColumn == 0 && mov.endRow == sizeY)
                                        || (mov.endColumn == sizeX && mov.endRow == 0)
                                        || (mov.endColumn == sizeX && mov.endRow == sizeY)).ToList();
                        if (winningKingMoveList.Count > 0)
                        {
                            numberDepth2Wins++;
                            item.scoreKingsCouncil += desrieToWinDepth2;
                        }
                    }
                    catch(Exception ex)
                    {
                        numberDepth2Wins += 0;
                    }
                });


                //if number of depth 2 wins is zero allow more full analyis
                if (numberDepth2Wins ==0)
                {                   
                                        
                    moveList[2].ForEach(item =>
                    {
                        
                        kingSquare = item.FindTheKing(item.board, true);
                        Piece king = new Piece(kingSquare.Column, kingSquare.Row, Piece.PieceType.King);
                        kingsMoveList = item.board.GetMovesForPiece(king, item, 3);
                        kingsMoveList = kingsMoveList.Where(mov =>  (mov.endColumn == 0 && mov.endRow == 0)
                                        ||(mov.endColumn == 0 && mov.endRow ==sizeY)
                                        || (mov.endColumn == sizeX && mov.endRow == 0)
                                        || (mov.endColumn == sizeX && mov.endRow == sizeY)
                        ).ToList();
                        if(kingsMoveList.Count>0)
                        {
                            item.parent.parent.scoreKingsCouncil += desireForKingToBeClearToCorner/ (double) moveList[1].Count;
                        }

                    });

                }  
                
                //return the best
                suggestedMoves.Add(moveList[0].MaxObject((item) => item.scoreKingsCouncil));
            }
            else
            {
                return suggestedMoves; //will be empty.
            }

            return suggestedMoves;

        }

        

    }
}
