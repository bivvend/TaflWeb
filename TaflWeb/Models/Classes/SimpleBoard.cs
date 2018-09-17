using System;
using System.Collections.Generic;
using static TaflWeb.Models.Classes.TurnDefinitions;

namespace TaflWeb.Model.Classes
{
    public class SimpleBoard  // Simple representation of a board used by AI calculations
    {

        public Square.occupation_type[,] OccupationArray { get; set; }

        public Square.square_type[,] SquareTypeArray { get; set; }    // Will be used sparingly,  as all boards will take the same type array

        public SimpleSquare kingSquare = null;     //Used to store the location of the king to improve AI speed.

        public SimpleBoard()
        {
            
        }

        //Copy constructor
        public SimpleBoard(SimpleBoard input)
        {
            int SizeX = input.OccupationArray.GetLength(0);
            int SizeY = input.OccupationArray.GetLength(1);

            Square.occupation_type[,] newOcc = new Square.occupation_type[SizeX, SizeY];
            Square.square_type[,] newTypes = new Square.square_type[SizeX, SizeY];

            for (int i = 0; i < SizeY; i++) //Rows
            {
                for (int j = 0; j < SizeX; j++) //Columns
                {
                    newOcc[j, i] = input.OccupationArray[j, i];
                    if(input.OccupationArray[j,i] == Square.occupation_type.King)
                    {
                        this.kingSquare = new SimpleSquare(j, i, Square.occupation_type.King, input.SquareTypeArray[j, i]);
                    }
                    newTypes[j, i] = input.SquareTypeArray[j, i];
                }
            }

            this.OccupationArray = newOcc;
            this.SquareTypeArray = newTypes;
        }


        public List<Move> GetPossibleMoves(TurnState turnState, Move parent, int depth)
        {
            List<Move> moveList = new List<Move>();
            List<Piece> activePieces = new List<Piece>();

            for (int i = 0; i < this.OccupationArray.GetLength(1); i++) // Rows
            {
                for (int j = 0; j < this.OccupationArray.GetLength(0); j++) //Columns
                {
                    if (turnState == TurnState.Defender && (OccupationArray[j,i]== Square.occupation_type.Defender || OccupationArray[j, i] == Square.occupation_type.King))
                    {
                        if (OccupationArray[j, i] == Square.occupation_type.King)
                            activePieces.Add(new Piece(j, i, Piece.PieceType.King));
                        else
                            activePieces.Add(new Piece(j, i, Piece.PieceType.Defender));

                    }
                    if (turnState == TurnState.Attacker && OccupationArray[j, i] == Square.occupation_type.Attacker)
                    {
                        activePieces.Add(new Piece(j, i, Piece.PieceType.Attacker));
                    }

                }
            }
            activePieces.ForEach((p) =>
            {
                moveList.AddRange(GetMovesForPiece(p, parent, depth));
            });            

            return moveList;
        }

        

        public List<Move> GetMovesForPiece(Piece p, Move parent, int depth)
        {
            List<Move> moveList = new List<Move>();
            int startColumn = p.Column;
            int startRow = p.Row;

            //decrease column until zero. Stop when find occupied or zero

            for (int N = startColumn - 1; N >= 0; N--)
            {

                if (OccupationArray[N,startRow] == Square.occupation_type.Empty && (SquareTypeArray[N,startRow] != Square.square_type.Corner || p.Type==Piece.PieceType.King))
                {
                    if (SquareTypeArray[N, startRow] != Square.square_type.Throne)
                    {
                        moveList.Add(new Move(startColumn, startRow, N, startRow, parent, depth));
                    }
                    else
                    {
                        if (p.Type == Piece.PieceType.King)
                        {
                            moveList.Add(new Move(startColumn, startRow, N, startRow, parent, depth));
                        }
                    }
                }
                else
                {
                    //Don't break for Throne
                    if (SquareTypeArray[N, startRow] != Square.square_type.Throne || OccupationArray[N, startRow] != Square.occupation_type.Empty)
                        break;
                }
                
            }

            //increase column. Stop when find occupied or Full size

            for (int N = startColumn + 1; N < OccupationArray.GetLength(0); N++)
            {

                if (OccupationArray[N, startRow] == Square.occupation_type.Empty && (SquareTypeArray[N, startRow] != Square.square_type.Corner || p.Type == Piece.PieceType.King))
                {
                    if (SquareTypeArray[N, startRow] != Square.square_type.Throne)
                    {
                        moveList.Add(new Move(startColumn, startRow, N, startRow, parent, depth));
                    }
                    else
                    {
                        if (p.Type == Piece.PieceType.King)
                        {
                            moveList.Add(new Move(startColumn, startRow, N, startRow, parent, depth));
                        }
                    }
                }
                else
                {
                    //Don't break for Throne
                    if (SquareTypeArray[N, startRow] != Square.square_type.Throne || OccupationArray[N, startRow] != Square.occupation_type.Empty)
                        break;
                }
                
            }

            //increase row until zero. Stop when find occupied or Full size

            for (int N = startRow + 1; N < OccupationArray.GetLength(1); N++)
            {
                if (OccupationArray[startColumn,N] == Square.occupation_type.Empty && (SquareTypeArray[startColumn,N] != Square.square_type.Corner || p.Type == Piece.PieceType.King))
                {
                    if (SquareTypeArray[startColumn, N] != Square.square_type.Throne)
                    {
                        moveList.Add(new Move(startColumn, startRow, startColumn, N, parent, depth));
                    }
                    else
                    {
                        if (p.Type == Piece.PieceType.King)
                        {
                            moveList.Add(new Move(startColumn, startRow, startColumn, N, parent, depth));
                        }
                    }
                }
                else
                {
                    //Don't break for Throne
                    if (SquareTypeArray[startColumn, N] != Square.square_type.Throne || OccupationArray[N, startRow] != Square.occupation_type.Empty)
                        break;
                }
                
            }

            //decrease row until zero. Stop when find occupied or zero
            for (int N = startRow - 1; N >= 0; N--)
            {
                if (OccupationArray[startColumn, N] == Square.occupation_type.Empty && (SquareTypeArray[startColumn, N] != Square.square_type.Corner || p.Type == Piece.PieceType.King))
                {
                    if (SquareTypeArray[startColumn, N] != Square.square_type.Throne)
                    {
                        moveList.Add(new Move(startColumn, startRow, startColumn, N, parent, depth));
                    }
                    else
                    {
                        if (p.Type == Piece.PieceType.King)
                        {
                            moveList.Add(new Move(startColumn, startRow, startColumn, N, parent, depth));
                        }
                    }
                }
                else
                {
                    //Don't break for Throne
                    if (SquareTypeArray[startColumn, N] != Square.square_type.Throne || OccupationArray[N, startRow] != Square.occupation_type.Empty)
                        break;
                }
            }

            return moveList;
        }
    }
}
