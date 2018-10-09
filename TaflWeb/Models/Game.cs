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

        private Square _selectedSquare;
        public Square selectedSquare
        {
            get
            {
                return _selectedSquare;
            }
            set
            {
                _selectedSquare = value;
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
            
            board = new BoardModel(11, 11);
        }

        public string GetString()
        {
            return "TaflWeb";
        }

        public string GetBoardAsJson()
        {
            string response = JsonConvert.SerializeObject(this);
            return response;
        }

        async public Task<Move> RunAITurn(SimpleBoard startBoard)
        {
            Move move = new Move();
            await Task.Delay(200);
            return move;
        }


        public string SquareClickResponse(int column, int row)
        {
            Square clickedSquare = board.GetSquare(row, column);
            ClickResponseTransferObject responseObj = new ClickResponseTransferObject() { responseText = ClickResponseTransferObject.NO_PIECE_FOUND, requestReDraw = false };
            if(clickedSquare != null)
            {
                //Check to see if square is occupied
                //Game already won
                if(currentTurnState == TurnState.VictoryAttacker || currentTurnState == TurnState.VictoryDefender)
                {
                    responseObj.requestReDraw = false;
                    responseObj.responseText = ClickResponseTransferObject.NOT_A_VALID_SQUARE;
                    return JsonConvert.SerializeObject(responseObj);
                }
                //Clicked attacker and attacker is not ai 
                if(clickedSquare.AttackerPresent && currentTurnState == TurnState.Attacker && !attackerIsAI)
                {
                    ApplySelection(clickedSquare);
                    responseObj.responseText = ClickResponseTransferObject.PIECE_FOUND_SELECTING;
                    responseObj.requestReDraw = true;
                    return JsonConvert.SerializeObject(responseObj);

                }
                //clicked defender and defender not ai
                if ((clickedSquare.DefenderPresent || clickedSquare.KingPresent) && currentTurnState == TurnState.Defender && !defenderIsAI)
                {
                    ApplySelection(clickedSquare);
                    responseObj.requestReDraw = true;
                    responseObj.responseText = ClickResponseTransferObject.PIECE_FOUND_SELECTING;
                    return JsonConvert.SerializeObject(responseObj);
                }
                //clicked empty square
                if (selectedSquare != null  && clickedSquare.Highlighted)
                {
                    if (!clickedSquare.AttackerPresent && !clickedSquare.DefenderPresent)
                    {
                        if (currentTurnState == TurnState.Attacker && !attackerIsAI)
                        {
                            board.MovePiece(selectedSquare.Row, selectedSquare.Column, clickedSquare.Row, clickedSquare.Column);
                            AdvanceTurn();
                            responseObj.requestReDraw = true;
                            responseObj.responseText = ClickResponseTransferObject.VALID_MOVE_FOUND_EXECUTING;
                            return JsonConvert.SerializeObject(responseObj);
                        }
                        if (currentTurnState == TurnState.Defender && !defenderIsAI)
                        {
                            board.MovePiece(selectedSquare.Row, selectedSquare.Column, clickedSquare.Row, clickedSquare.Column);
                            AdvanceTurn();
                            responseObj.requestReDraw = true;
                            responseObj.responseText = ClickResponseTransferObject.VALID_MOVE_FOUND_EXECUTING;
                            return JsonConvert.SerializeObject(responseObj);
                        }
                    }
                }
            }
            else
            {
                responseObj.requestReDraw = false;
                responseObj.responseText= ClickResponseTransferObject.NOT_A_VALID_SQUARE;
                return  JsonConvert.SerializeObject(responseObj);
            }

            
            return JsonConvert.SerializeObject(responseObj); //  catch

        }

        private void AdvanceTurn()
        {
            //See if anyone has won

            //Check for Attacker victory
            if (currentTurnState == TurnState.Attacker)
            {
                if (board.CheckForAttackerVictory())
                {
                    currentTurnState = TurnState.VictoryAttacker;
                }
            }
            if(currentTurnState == TurnState.Defender)
            {
                if(board.CheckForDefenderVictory())
                {
                    currentTurnState = TurnState.VictoryDefender;
                }
            }

            TurnState newState = TurnState.Resetting;

            if(currentTurnState == TurnState.Attacker)
            {
                newState = TurnState.Defender;
            }
            if(currentTurnState == TurnState.Defender)
            {
                newState = TurnState.Attacker;
            }

            currentTurnState = newState;
            ClearSelections();

            
            
        }

        private void HighlightPossibleMoves(Square squareSelected)
        {
            int startColumn = squareSelected.Column;
            int startRow = squareSelected.Row;
            //Clear All Current Highlighting
            board.board.ToList().ForEach((item) => item.Highlighted = false);

            //decrease column until zero. Stop when find occupied or zero
            Square aSquare = null;

            for (int N = startColumn - 1; N >= 0; N--)
            {
                aSquare = GetSquare(startRow, N);
                if (aSquare != null)
                {
                    if (aSquare.Occupation == Square.occupation_type.Empty && (aSquare.SquareType != Square.square_type.Corner || squareSelected.KingPresent))
                    {
                        if (aSquare.SquareType != Square.square_type.Throne)
                        {
                            aSquare.Highlighted = true;
                        }
                        else
                        {
                            if (squareSelected.KingPresent)
                            {
                                aSquare.Highlighted = true;
                            }
                        }
                    }
                    else
                    {
                        //Don't break for Throne unless it is occupied
                        if (aSquare.SquareType != Square.square_type.Throne || aSquare.Occupation != Square.occupation_type.Empty)
                            break;
                    }
                }
            }

            //increase column. Stop when find occupied or Full size

            for (int N = startColumn + 1; N < board.SizeX; N++)
            {
                aSquare = GetSquare(startRow, N);
                if (aSquare != null)
                {
                    if (aSquare.Occupation == Square.occupation_type.Empty && (aSquare.SquareType != Square.square_type.Corner || squareSelected.KingPresent))
                    {
                        if (aSquare.SquareType != Square.square_type.Throne)
                        {
                            aSquare.Highlighted = true;
                        }
                        else
                        {
                            if (squareSelected.KingPresent)
                            {
                                aSquare.Highlighted = true;
                            }
                        }
                    }
                    else
                    {
                        //Don't break for Throne
                        if (aSquare.SquareType != Square.square_type.Throne || aSquare.Occupation != Square.occupation_type.Empty)
                            break;
                    }
                }
            }

            //increase row until zero. Stop when find occupied or Full size

            for (int N = startRow + 1; N < board.SizeY; N++)
            {
                aSquare = GetSquare(N, startColumn);
                if (aSquare != null)
                {
                    if (aSquare.Occupation == Square.occupation_type.Empty && (aSquare.SquareType != Square.square_type.Corner || squareSelected.KingPresent))
                    {
                        if (aSquare.SquareType != Square.square_type.Throne)
                        {
                            aSquare.Highlighted = true;
                        }
                        else
                        {
                            if (squareSelected.KingPresent)
                            {
                                aSquare.Highlighted = true;
                            }
                        }
                    }
                    else
                    {
                        //Don't break for Throne
                        if (aSquare.SquareType != Square.square_type.Throne || aSquare.Occupation != Square.occupation_type.Empty)
                            break;
                    }
                }
            }

            //decrease row until zero. Stop when find occupied or zero
            for (int N = startRow - 1; N >= 0; N--)
            {
                aSquare = GetSquare(N, startColumn);
                if (aSquare != null)
                {
                    if (aSquare.Occupation == Square.occupation_type.Empty && (aSquare.SquareType != Square.square_type.Corner || squareSelected.KingPresent))
                    {
                        if (aSquare.SquareType != Square.square_type.Throne)
                        {
                            aSquare.Highlighted = true;
                        }
                        else
                        {
                            if (squareSelected.KingPresent)
                            {
                                aSquare.Highlighted = true;
                            }
                        }
                    }
                    else
                    {
                        //Don't break for Throne
                        if (aSquare.SquareType != Square.square_type.Throne || aSquare.Occupation != Square.occupation_type.Empty)
                            break;
                    }
                }
            }
        }

        private Square GetSquare(int Row, int Column)
        {
            Square squareFound = null;

            List<Square> squaresFound = board.board.Where((item) => item.Column == Column && item.Row == Row).ToList();
            if (squaresFound.Count > 0)
            {
                squareFound = squaresFound[0];
            }
            return squareFound;
        }

        private void ApplySelection(Square square)
        {
            SelectSquare(square);
            if (square.Selected)
            {
                HighlightPossibleMoves(square);
                selectedSquare = square;
            }
            else
            {
                board.board.ToList().ForEach((item) => item.Highlighted = false);
                selectedSquare = null;
            }
        }

        private void ClearSelections()
        {
            foreach (Square square in board.board)
            {
                square.Selected = false;
                square.Highlighted = false;
            }
            selectedSquare = null;
        }

        private void SelectSquare(Square squareToSelect)
        {
            foreach (Square square in board.board)
            {
                if (square.Row != squareToSelect.Row || square.Column != squareToSelect.Column)
                {
                    square.Selected = false;
                    square.Highlighted = false;
                }
            }

            squareToSelect.Selected = !squareToSelect.Selected;
            if(squareToSelect.Selected)
            {
                selectedSquare = squareToSelect;
            }
            else
            {
                selectedSquare = null;
            }
        }
    }
}