using System;
using System.Collections.Generic;
using System.Text;

namespace TaflWeb.Model.Classes
{
    public class Move 
    {

        public double runTime { get; set; }
        public int startColumn { get; set; }
        public int startRow { get; set; }
        public int endColumn { get; set; }
        public int endRow { get; set; }

        public Move parent { get; set; }
        public int depth { get; set; }

        public double scoreSage { get; set; }
        public double scoreAssassin { get; set; }
        public double scoreGeneral { get; set; }
        public double scoreKingsCouncil { get; set; }

        public int numberTakesDefender { get; set; }
        public List<int> numberTakesDefenderAtDepth { get; set; } //Used on main parent move to evalute danger/success profile

        public int numberTakesAttacker { get; set; }
        public List<int> numberTakesAttackerAtDepth { get; set; } ////Used on main parent move to evalute danger/success profile

        public SimpleBoard board { get; set; }  //Board associated with the move.  Represents the state before and then after the move is made

        private string[] columnMappringArray = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P" };
        public List<string> columnMapping { get; set; }

        private string[] rowMappringArray = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };
        public List<string> rowMapping { get; set; }

        public string stringRespresentation { get; set; }

        public enum direction
        {
            FromBelow, FromAbove, FromLeft, FromRight
        };        

        public Move()
        {
            this.startColumn = 0;
            this.startRow = 0;
            this.endColumn = 0;
            this.endRow = 0;
            parent = null;
            scoreSage = 0.0d;
            scoreAssassin = 0.0d;
            scoreGeneral = 0.0d;
            scoreKingsCouncil = 0.0d;
            depth = 0;
            numberTakesAttacker = 0;
            numberTakesDefender = 0;

            numberTakesAttackerAtDepth = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            numberTakesDefenderAtDepth = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            columnMapping = new List<string>(columnMappringArray);  //init from array
            rowMapping = new List<string>(rowMappringArray);  //init from array


        }

        public Move(int iStartColumn, int iStartRow, int iEndColumn, int iEndRow, Move parentMove, int iDepth)
        {
            this.startColumn = iStartColumn;
            this.startRow = iStartRow;
            this.endColumn = iEndColumn;
            this.endRow = iEndRow;
            this.parent = parentMove;
            this.depth = iDepth;
            scoreSage = 0.0d;
            scoreAssassin = 0.0d;
            scoreGeneral = 0.0d;
            scoreKingsCouncil = 0.0d;
            this.numberTakesAttacker = 0;
            this.numberTakesDefender = 0;

            numberTakesAttackerAtDepth = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            numberTakesDefenderAtDepth = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            columnMapping = new List<string>(columnMappringArray);  //init from array
            rowMapping = new List<string>(rowMappringArray);  //init from array
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(columnMapping[this.startColumn]);
            sb.Append(rowMapping[this.startRow]);
            sb.Append(" to ");
            sb.Append(columnMapping[this.endColumn]);
            sb.Append(rowMapping[this.endRow]);
            this.stringRespresentation = sb.ToString();
            return sb.ToString();

        }

        /// <summary>
        /// Makes the move and process takes etc
        /// </summary>
        /// <param name="move"></param>
        public void  MakeMove(Move move, SimpleBoard initialBoard)
        {
            //Use copy contructor
            board = new SimpleBoard(initialBoard);
            //Determine who is making the move
            bool isDefender = false;
            bool isAttacker = false;
            bool isKing = false;
            bool Error = false;

            if (board.OccupationArray[move.startColumn, move.startRow] == Square.occupation_type.Defender)
                isDefender = true;
            if (board.OccupationArray[move.startColumn, move.startRow] == Square.occupation_type.Attacker)
                isAttacker = true;
            if (board.OccupationArray[move.startColumn, move.startRow] == Square.occupation_type.King)
                isKing = true;

            //Make the move
            if(board.OccupationArray[move.endColumn, move.endRow]!= Square.occupation_type.Empty)
            {
                //Error somewhere
                Error = true;
                throw new Exception();
            }
            board.OccupationArray[move.endColumn, move.endRow] = board.OccupationArray[move.startColumn, move.startRow];
            board.OccupationArray[move.startColumn, move.startRow] = Square.occupation_type.Empty;

            //Process takes
            CheckAndProcessTake();

            //Reassign kingSquare
            if(isKing)
            {
                board.kingSquare = new SimpleSquare(move.endColumn, move.endRow, Square.occupation_type.King, board.SquareTypeArray[move.endColumn, move.endRow]);
            }

        }

        public SimpleSquare GetSquare(int row, int column)   //Need to be careful throughout as have a habit of swapping these.  Using row, column (y,x) here and in Check and process take
        {
            SimpleSquare retSquare = new SimpleSquare();

            try
            {
                if (column >= board.OccupationArray.GetLength(0) || column < 0 || row >= board.OccupationArray.GetLength(1) || row < 0)
                {
                    return null;
                }
                retSquare.Occupation = board.OccupationArray[column, row];
                retSquare.SquareType = board.SquareTypeArray[column, row];
                retSquare.Row = row;
                retSquare.Column = column;
            }
            catch(Exception ex)
            {
                return null;
            }
            
            return retSquare;

        }

        private void CheckAndProcessTake()
        {

            SimpleSquare endSquare = GetSquare(this.endRow, this.endColumn);
            //Check UP 1 ROW
           
            SimpleSquare squareToCheck = GetSquare(endSquare.Row - 1, endSquare.Column);
            if (squareToCheck != null) //Is a valid square
            {
                SearchAroundForTake(squareToCheck, endSquare, direction.FromBelow);
            }

            //Check DOWN 1 ROW
            squareToCheck = GetSquare(endSquare.Row + 1, endSquare.Column);
            if (squareToCheck != null) //Is a valid square
            {
                SearchAroundForTake(squareToCheck, endSquare, direction.FromAbove);
            }

            //Check LEFT 1 COLUMN
            squareToCheck = GetSquare(endSquare.Row, endSquare.Column - 1);
            if (squareToCheck != null) //Is a valid square
            {
                SearchAroundForTake(squareToCheck, endSquare, direction.FromRight);
            }

            //Check RIGHT 1 COLUMN
            squareToCheck = GetSquare(endSquare.Row, endSquare.Column + 1);
            if (squareToCheck != null) //Is a valid square
            {
                SearchAroundForTake(squareToCheck, endSquare, direction.FromLeft);
            }

        }

        public int NumberOfAttackersAroundKing()
        {
            int numberOfAttackers = 0;
            SimpleSquare kingSquare = FindTheKing(board, true);
            SimpleSquare up = GetSquare(kingSquare.Row - 1, kingSquare.Column);
            SimpleSquare down = GetSquare(kingSquare.Row + 1, kingSquare.Column);
            SimpleSquare left = GetSquare(kingSquare.Row, kingSquare.Column - 1);
            SimpleSquare right = GetSquare(kingSquare.Row, kingSquare.Column + 1);

            if (up != null && up.AttackerPresent)
                numberOfAttackers++;
            if (down != null && down.AttackerPresent)
                numberOfAttackers++;
            if (left != null && left.AttackerPresent)
                numberOfAttackers++;
            if (right != null && right.AttackerPresent)
                numberOfAttackers++;


            return numberOfAttackers;
        }

        public SimpleSquare FindTheKing(SimpleBoard board, bool quick)
        {

            if (quick)
                return board.kingSquare;
            else
            {
                SimpleSquare kingSquare = null;
                for (int i = 0; i < board.OccupationArray.GetLength(0); i++) //Rows
                {
                    for (int j = 0; j < board.OccupationArray.GetLength(1); j++) //Columns
                    {
                        if (board.OccupationArray[j, i] == Square.occupation_type.King)
                        {
                            kingSquare = new SimpleSquare(j, i, Square.occupation_type.King, board.SquareTypeArray[j, i]);
                            return kingSquare;
                        }

                    }
                }
                if (kingSquare == null)
                {
                    return null;
                }

                return kingSquare;
            }
        }

        

        public bool CheckForAttackerVictory()
        {
            //Check to see if King is surrounded on 4 sides
            try
            {
                SimpleSquare kingSquare = FindTheKing(this.board, true);
                if (kingSquare == null)
                    return false;
                SimpleSquare up = GetSquare(kingSquare.Row - 1, kingSquare.Column);
                SimpleSquare down = GetSquare(kingSquare.Row + 1, kingSquare.Column);
                SimpleSquare left = GetSquare(kingSquare.Row, kingSquare.Column - 1);
                SimpleSquare right = GetSquare(kingSquare.Row, kingSquare.Column + 1);

                if (up != null && down != null && left != null && right != null)
                {
                    if ((up.AttackerPresent || up.SquareType == Square.square_type.Throne) && (down.AttackerPresent || down.SquareType == Square.square_type.Throne) && (left.AttackerPresent || left.SquareType == Square.square_type.Throne) && (right.AttackerPresent || right.SquareType == Square.square_type.Throne))
                    {
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return false;
        }

        private void SearchAroundForTake(SimpleSquare squareToCheck, SimpleSquare endSquare, direction dir)
        {
            //squareToCheck is the square with the possible piece to be taken,  endSquare is the square into which the possible taker moved, direction is the direction that the taker moved w.r.t takee.
            SimpleSquare squareTwoAway = null;
            if (squareToCheck.Occupation != Square.occupation_type.Empty && squareToCheck.Occupation != Square.occupation_type.King) //Something in the square
            {
                if (squareToCheck.AttackerPresent && (endSquare.KingPresent || endSquare.DefenderPresent))
                {
                    //Defender or King moved next to Attacker
                    //Look 2 squares away in given direction for defender or King
                    switch (dir)
                    {
                        case direction.FromAbove:
                            squareTwoAway = GetSquare(endSquare.Row + 2, endSquare.Column);
                            break;
                        case direction.FromBelow:
                            squareTwoAway = GetSquare(endSquare.Row - 2, endSquare.Column);
                            break;
                        case direction.FromLeft:
                            squareTwoAway = GetSquare(endSquare.Row, endSquare.Column + 2);
                            break;
                        case direction.FromRight:
                            squareTwoAway = GetSquare(endSquare.Row, endSquare.Column - 2);
                            break;
                    }
                    if (squareTwoAway != null)
                    {
                        if (squareTwoAway.DefenderPresent || squareTwoAway.KingPresent || squareTwoAway.SquareType == Square.square_type.Corner)
                        {
                            board.OccupationArray[squareToCheck.Column, squareToCheck.Row] = Square.occupation_type.Empty;
                            numberTakesDefender++;
                        }
                    }

                }
                if (squareToCheck.DefenderPresent && endSquare.AttackerPresent)
                {
                    //Attacker moved next to defender
                    switch (dir)
                    {
                        case direction.FromAbove:
                            squareTwoAway = GetSquare(endSquare.Row + 2, endSquare.Column);
                            break;
                        case direction.FromBelow:
                            squareTwoAway = GetSquare(endSquare.Row - 2, endSquare.Column);
                            break;
                        case direction.FromLeft:
                            squareTwoAway = GetSquare(endSquare.Row, endSquare.Column + 2);
                            break;
                        case direction.FromRight:
                            squareTwoAway = GetSquare(endSquare.Row, endSquare.Column - 2);
                            break;
                    }
                    if (squareTwoAway != null)
                    {
                        if (squareTwoAway.AttackerPresent || squareTwoAway.SquareType == Square.square_type.Corner)
                        {
                            board.OccupationArray[squareToCheck.Column, squareToCheck.Row] = Square.occupation_type.Empty;
                            numberTakesAttacker++;
                        }
                    }
                }
            }
        }


    }
}
