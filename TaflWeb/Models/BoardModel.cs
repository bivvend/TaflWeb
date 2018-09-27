using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaflWeb.Model.Classes;

namespace TaflWeb.Models
{
    public class BoardModel
    {

        public List<Square> board;

        private int sizeX;
        public int SizeX
        {
            get
            {
                return sizeX;
            }
            set
            {
                if (value > 0)
                {
                    sizeX = value;

                }
            }
        }

        private int sizeY;

        public int SizeY
        {
            get
            {
                return sizeY;
            }
            set
            {
                if (value > 0)
                {
                    sizeY = value;

                }
            }
        }

        public BoardModel(int sizeXIn, int sizeYIn)
        {
            board = new List<Square>();
            SizeX = sizeXIn;
            SizeY = sizeYIn;
            CreateBoard();
        }

        public bool CheckForAttackerVictory()
        {
            //Check to see if King is surrounded on 4 sides
            Square kingSquare = null;
            List<Square> squaresFound = board.Where((item) => item.KingPresent).ToList();
            if (squaresFound.Count > 0)
            {
                kingSquare = squaresFound[0];
            }
            if (kingSquare == null)
                return false;
            Square up = GetSquare(kingSquare.Row - 1, kingSquare.Column);
            Square down = GetSquare(kingSquare.Row + 1, kingSquare.Column);
            Square left = GetSquare(kingSquare.Row, kingSquare.Column - 1);
            Square right = GetSquare(kingSquare.Row, kingSquare.Column + 1);

            if (up != null && down != null && left != null && right != null)
            {
                if ((up.AttackerPresent || up.SquareType == Square.square_type.Throne) && (down.AttackerPresent || down.SquareType == Square.square_type.Throne) && (left.AttackerPresent || left.SquareType == Square.square_type.Throne) && (right.AttackerPresent || right.SquareType == Square.square_type.Throne))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckForDefenderVictory()
        {
            //Check to see if King is surrounded on 4 sides
            Square kingSquare = null;
            List<Square> squaresFound = board.Where((item) => item.KingPresent).ToList();
            if (squaresFound.Count > 0)
            {
                kingSquare = squaresFound[0];
            }
            if (kingSquare == null)
                return false;

            if (kingSquare.SquareType == Square.square_type.Corner)
            {
                return true;
            }

            return false;
        }

        public bool MovePiece(int startRow, int startColumn, int endRow, int endColumn)
        {
            Square startSquare = GetSquare(startRow, startColumn);
            Square endSquare = GetSquare(endRow, endColumn);

            if (startSquare != null && endSquare != null)
            {

                endSquare.Occupation = startSquare.Occupation;
                startSquare.Occupation = Square.occupation_type.Empty;
                CheckAndProcessTake(endSquare);

                return true;
            }
            else
            {
                return false;
            }
        }

        private enum direction
        {
            FromBelow, FromAbove, FromLeft, FromRight
        };



        private void SearchAroundForTake(Square squareToCheck, Square endSquare, direction dir)
        {
            //squareToCheck is the square with the possible piece to be taken,  endSquare is the square into which the possible taker moved, direction is the direction that the taker moved w.r.t takee.
            Square squareTwoAway = null;
            if (squareToCheck.Occupation != Square.occupation_type.Empty) //Something in the square
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
                            squareToCheck.Occupation = Square.occupation_type.Empty;
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
                            squareToCheck.Occupation = Square.occupation_type.Empty;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Look at all the pieces next to the piece that just moved and see if these can be taken
        /// </summary>
        /// <param name="endSquare"></param>
        public void CheckAndProcessTake(Square endSquare)
        {
            //Check UP 1 ROW
            Square squareToCheck = GetSquare(endSquare.Row - 1, endSquare.Column);
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


        public SimpleBoard GetSimpleBoard()
        {
            SimpleBoard simpleBoard = new SimpleBoard();
            simpleBoard.SquareTypeArray = new Square.square_type[SizeX, SizeY];
            simpleBoard.OccupationArray = new Square.occupation_type[SizeX, SizeY];

            board.ToList().ForEach((item) =>
            {
                simpleBoard.SquareTypeArray[item.Column, item.Row] = item.SquareType;
                simpleBoard.OccupationArray[item.Column, item.Row] = item.Occupation;
            });

            return simpleBoard;
        }

        public string GetSimpleBoardAsJSON()
        {            

            SimpleBoard simpleBoard = new SimpleBoard();
            simpleBoard.SquareTypeArray = new Square.square_type[SizeX, SizeY];
            simpleBoard.OccupationArray = new Square.occupation_type[SizeX, SizeY];

            board.ToList().ForEach((item) =>
            {
                simpleBoard.SquareTypeArray[item.Column, item.Row] = item.SquareType;
                simpleBoard.OccupationArray[item.Column, item.Row] = item.Occupation;
            });

            return JsonConvert.SerializeObject(simpleBoard);
        }

        private Square GetSquare(int Row, int Column)
        {
            Square squareFound = null;

            List<Square> squaresFound = board.Where((item) => item.Column == Column && item.Row == Row).ToList();
            if (squaresFound.Count > 0)
            {
                squareFound = squaresFound[0];
            }
            return squareFound;
        }

        public void CreateBoard()
        {
            this.board.Clear();
            for (int j = 0; j < SizeY; j++)
            {
                for (int i = 0; i < SizeX; i++)
                {
                    Square a_square = new Square(i, j, Square.occupation_type.Empty, Square.square_type.Normal);
                    if (a_square.BareTileType == Square.bare_tile_type.tile1)
                        a_square.ImageName = "tile1.bmp";
                    if (a_square.BareTileType == Square.bare_tile_type.tile2)
                        a_square.ImageName = "tile2.bmp";
                    if (a_square.BareTileType == Square.bare_tile_type.tile3)
                        a_square.ImageName = "tile3.bmp";
                    if (a_square.BareTileType == Square.bare_tile_type.tile4)
                        a_square.ImageName = "tile4.bmp";


                    //Make corner squares
                    if ((i == 0 && j == 0) || (i == 0 && j == SizeY - 1) || (i == SizeX - 1 && j == 0) || (i == SizeX - 1 && j == SizeY - 1))
                    {
                        a_square.SquareType = Square.square_type.Corner;
                        if (a_square.BareTileType == Square.bare_tile_type.tile1)
                            a_square.ImageName = "/Tafl;component/Resources/deftile1.bmp";
                        if (a_square.BareTileType == Square.bare_tile_type.tile2)
                            a_square.ImageName = "/Tafl;component/Resources/deftile2.bmp";
                        if (a_square.BareTileType == Square.bare_tile_type.tile3)
                            a_square.ImageName = "/Tafl;component/Resources/deftile3.bmp";
                        if (a_square.BareTileType == Square.bare_tile_type.tile4)
                            a_square.ImageName = "/Tafl;component/Resources/deftile4.bmp";

                    }
                    //Make central (Defender) zone
                    if ((i == 3 && j == 5) || (i == 4 && j > 3 && j < 7) || (i == 5 && j > 2 && j < 8) || (i == 6 && j > 3 && j < 7) || (i == 7 && j == 5))
                    {
                        a_square.SquareType = Square.square_type.DefenderStart;
                        if (a_square.BareTileType == Square.bare_tile_type.tile1)
                            a_square.ImageName = "/Tafl;component/Resources/deftile1.bmp";
                        if (a_square.BareTileType == Square.bare_tile_type.tile2)
                            a_square.ImageName = "/Tafl;component/Resources/deftile2.bmp";
                        if (a_square.BareTileType == Square.bare_tile_type.tile3)
                            a_square.ImageName = "/Tafl;component/Resources/deftile3.bmp";
                        if (a_square.BareTileType == Square.bare_tile_type.tile4)
                            a_square.ImageName = "/Tafl;component/Resources/deftile4.bmp";
                        a_square.Occupation = Square.occupation_type.Defender;
                    }
                    //Make throne
                    if (i == SizeX / 2 && j == SizeY / 2)
                    {
                        a_square.SquareType = Square.square_type.Throne;
                        a_square.ImageName = "throne.bmp";
                        a_square.Occupation = Square.occupation_type.King;
                    }
                    //Make edge zones for attackers
                    if (((i == 0 && j > 2 && j < 8) || (i == 1 && j == 5)) || ((i == 10 && j > 2 && j < 8) || (i == 9 && j == 5)) || ((j == 0 && i > 2 && i < 8) || (j == 1 && i == 5)) || ((j == 10 && i > 2 && i < 8) || (j == 9 && i == 5)))
                    {
                        a_square.SquareType = Square.square_type.AttackerStart;
                        if (a_square.BareTileType == Square.bare_tile_type.tile1)
                            a_square.ImageName = "attacktile1.bmp";
                        if (a_square.BareTileType == Square.bare_tile_type.tile2)
                            a_square.ImageName = "attacktile2.bmp";
                        if (a_square.BareTileType == Square.bare_tile_type.tile3)
                            a_square.ImageName = "attacktile3.bmp";
                        if (a_square.BareTileType == Square.bare_tile_type.tile4)
                            a_square.ImageName = "attacktile4.bmp";
                        a_square.Occupation = Square.occupation_type.Attacker;
                    }
                    board.Add(a_square);
                }
            }
        }
    }
}
