using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaflWeb.Model.Classes;
using static TaflWeb.Models.Classes.TurnDefinitions;

namespace TaflWeb.Model.AI
{
    //Evaluator look at the potential for a move to enhance the capture likelihood of the King and to surround pieces
    public class Assassin 
    {
        public double desireForWinDepth0 = 1000000000.0;  // Always win.
        public double desireForWinDepth2 = 10000.0;

        public double desireNotToLoseDepth1 = 1000000000.0;  // Always move to block a loss 
        public double desireNotToLoseDepth3 = 100000;

        public List<Move> Evaluate(List<List<Move>> inputMoveList, TurnState currentTurnState)
        {
            List<Move> suggestedMoves = new List<Move>();

            //Look to have the outer pieces on the maximum number of rows
            DateTime start = DateTime.Now;
            DateTime start2 = DateTime.Now;
            TimeSpan duration = new TimeSpan();
            double runTime = 0.0d;
            double runTime2 = 0.0d;
            SimpleSquare kingSquare = new SimpleSquare();
            List<Move> kingsMoveList = new List<Move>();

            int sizeX = inputMoveList[0][0].board.OccupationArray.GetLength(0);
            int sizeY = inputMoveList[0][0].board.OccupationArray.GetLength(1);

            int numberMovesDepth0 = 0;
            int numberOfLosesDepth1 = 0;
            bool foundwin = false;

            inputMoveList[0].ForEach(item =>
            {
                if(item.CheckForAttackerVictory())
                {
                    item.scoreAssassin = desireForWinDepth0;
                    numberMovesDepth0++;
                    foundwin = true;
                }
            });

            //check to see if a move blocks a depth 1 loss
            if (!foundwin)
            {          

                inputMoveList[1].ForEach(item =>
                {
                    //If a loss is possible all moves in which the loss can happen should be penalised.
                    SimpleSquare kingSquareWin = item.FindTheKing(item.board, true);
                    if (kingSquareWin.SquareType == Square.square_type.Corner)
                    {
                        numberOfLosesDepth1++;
                        item.parent.scoreAssassin -= desireNotToLoseDepth1;
                    }
                    //if the king can get to a square next to a corner it is also a definte loss effectively at depth 3
                    //so penalise at depth 1
                    if (IsNextToCorner(kingSquareWin, item.board))
                    {
                        numberOfLosesDepth1++;
                        item.parent.scoreAssassin -= desireNotToLoseDepth1;
                    }
                    

                });

                runTime = (DateTime.Now - start).TotalSeconds;

                start2 = DateTime.Now;

                //Check to see if can win, but not if can lose on next defender move
                bool worthLookingDeep = false;
                
                if (numberOfLosesDepth1 < 1)
                {
                    //Check if king is at least surrounded by 2 attackers  -  if so look for a 2 move combo that might win
                    inputMoveList[0].ForEach(item =>
                    {
                        if(item.NumberOfAttackersAroundKing() >= 2)
                        {
                            worthLookingDeep = true;
                        }
                         
                    });

                    if (worthLookingDeep)
                    {
                        Parallel.ForEach(inputMoveList[2], item =>
                        {
                            if (item.CheckForAttackerVictory())
                            {
                                item.parent.parent.scoreAssassin += desireForWinDepth2 / (double)inputMoveList[1].Count;
                            }
                        });
                    }
                }
                runTime2 = (DateTime.Now - start2).TotalSeconds;
                runTime = (DateTime.Now - start).TotalSeconds;

                //look for moves of the king at depth 3 
                //Look for routes for the king at depth 3 to get to corner
                sizeX = inputMoveList[0][0].board.OccupationArray.GetLength(0) - 1;
                sizeY = inputMoveList[0][0].board.OccupationArray.GetLength(1) - 1;
                

                Object _lock = new Object();
                if (numberOfLosesDepth1 < 1)
                {
                    inputMoveList[1].ForEach( item =>  //This could be moveList[2] but would be slow.   This only evolves from the defenders last move (i.e. skips the attackers move)
                    {

                        kingSquare = item.FindTheKing(item.board, true);
                        Piece king = new Piece(kingSquare.Column, kingSquare.Row, Piece.PieceType.King);
                        kingsMoveList = item.board.GetMovesForPiece(king, item, 3); // This is very expensive
                        kingsMoveList = kingsMoveList.Where(mov => (mov.endColumn == 0 && mov.endRow == 0)
                                        || (mov.endColumn == 0 && mov.endRow == sizeY)
                                        || (mov.endColumn == sizeX && mov.endRow == 0)
                                        || (mov.endColumn == sizeX && mov.endRow == sizeY)
                        ).ToList();
                        if (kingsMoveList.Count > 0)
                        {
                            item.parent.scoreAssassin -= desireNotToLoseDepth3 / (double)inputMoveList[1].Count;
                        }                        

                    });

                }
            }
            runTime = (DateTime.Now - start).TotalSeconds;

            suggestedMoves.Add(inputMoveList[0].MaxObject(item => item.scoreAssassin));

            return suggestedMoves;

        }

        private double GetFractionOfRowsAndColumnsControlled(SimpleBoard board)
        {
            double fraction = 0.0d;
            return fraction;
        }

        /// <summary>
        /// Checks to see if a square is next to a corner
        /// </summary>
        /// <param name="square"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public bool IsNextToCorner(SimpleSquare square, SimpleBoard board)
        {
            int sizeX = board.OccupationArray.GetLength(0);
            int sizeY = board.OccupationArray.GetLength(1);
            int posX = square.Column;
            int posY = square.Row;

            if (posX == 0 && posY == 1)
                return true;
            if (posX == 1 && posY == 0)
                return true;
            if (posX == 0 && posY == sizeY -2)
                return true;
            if (posX == 1 && posY == sizeY - 1)
                return true;
            if (posX == sizeX -2 && posY == 0)
                return true;
            if (posX == sizeX - 1 && posY == 1)
                return true;
            if (posX == sizeX -2 && posY == sizeY - 1)
                return true;
            if (posX == sizeX - 1 && posY == sizeY - 2)
                return true;

            return false;

        }

    }
}
