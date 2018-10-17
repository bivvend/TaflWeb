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

        private string _responseText;
        public string responseText
        {
            get
            {
                return _responseText;
            }
            set
            {
                _responseText = value;
            }
        }

        [JsonIgnore]
        private string NOT_A_VALID_SQUARE = "NOT_A_VALID_SQUARE";
        [JsonIgnore]
        private string NO_PIECE_FOUND = "NO_PIECE_FOUND";
        [JsonIgnore]
        private string PIECE_FOUND_SELECTING = "PIECE_FOUND_SELECTING";
        [JsonIgnore]
        private string VALID_MOVE_FOUND_EXECUTING = "VALID_MOVE_FOUND_EXECUTING";
        [JsonIgnore]
        private string AI_MOVE_COMPLETED = "AI_MOVE_COMPLETED";


        private bool _requestReDraw;
        public bool requestReDraw
        {
            get
            {
                return _requestReDraw;
            }
            set
            {
                _requestReDraw = value;
            }
        }


        [JsonIgnore]
        private Sage _sage;
        [JsonIgnore]
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
            moveHistory = new List<Move>();
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

        public async Task<string> RunAI(int turnState)
        {
            string response = "";
            sage = new Sage();
            SimpleBoard boardToRun = board.GetSimpleBoard();
            Move moveToMake = await Task<Move>.Factory.StartNew(() => RunAITurn(boardToRun));
            ApplyAIMove(moveToMake);
            moveToMake.ToString();  // ensure string representation is upto date;
            moveHistory.Add(moveToMake);
            requestReDraw = true;
            responseText = AI_MOVE_COMPLETED;
            response = JsonConvert.SerializeObject(this);
            return response;
        }

        private void ApplyAIMove (Move aMove)
        {
            //Overlay the simple board onto the real board
            int SizeX = aMove.board.OccupationArray.GetLength(0);
            int SizeY = aMove.board.OccupationArray.GetLength(1);
            Square aSquare = new Square();
            for (int i = 0; i < SizeY; i++) //Rows
            {
                for (int j = 0; j < SizeX; j++) //Columns
                {
                    aSquare = GetSquare(i, j);
                    if (aSquare != null)
                    {
                        aSquare.Occupation = aMove.board.OccupationArray[j, i];
                    }
                }
            }

            if (currentTurnState == TurnState.Attacker)
            {
                if(CheckForRepeatMoves(aMove))
                {
                    currentTurnState = TurnState.VictoryDefender;
                }
                if (board.CheckForAttackerVictory())
                {
                    currentTurnState = TurnState.VictoryAttacker;
                }
            }

            if (currentTurnState == TurnState.Defender)
            {
                if (CheckForRepeatMoves(aMove))
                {
                    currentTurnState = TurnState.VictoryAttacker;
                }
                if (board.CheckForDefenderVictory())
                {
                    currentTurnState = TurnState.VictoryDefender;
                }

            }

            if (currentTurnState == TurnState.Defender)
            {
                currentTurnState = TurnState.Attacker;
            }
            else if (currentTurnState == TurnState.Attacker)
            {

                currentTurnState = TurnState.Defender;
            }
        }

        private bool CheckForRepeatMoves(Move aMove)
        {
            bool isRepeat = false;
            //Don't check if
            //Current (not added yet) yours*//enemy 0 //yours-1 //enemy-2 //yours-3 *//enemy-4 //yours-5//enemy-6 //yours-7 *//enemy-8
            //If current  -3  and -7 are the same
            //and 
            //enemy's  0  -4  and -8   are the same 
            //Then the board position has repeated 3 times so it is a loss for the current player as was forced to make an illegal move.

            //For list indicies 
            //Current  with Count-4 and Count-8
            //Count-1 with Count-5 and Count-9
            if(moveHistory.Count > 8)   //Needs to be at least 9 moves in history.
            {

            }

            return isRepeat;
        }

        private Move RunAITurn(SimpleBoard startBoard)
        {
            SimpleBoard BaseBoard = startBoard;

            List<Move> moveList_0 = new List<Move>();
            DateTime start = DateTime.Now;
            sage.bestList = new List<Move>();
            sage.longTermBestList = new List<Move>();

            //Fill list with all possible moves of depth 0

            moveList_0 = BaseBoard.GetPossibleMoves(this.currentTurnState, null, 0);

            Object _lock = new object();

            //Create Depth 1 moves
            Parallel.ForEach(moveList_0, (m) =>
            {
                //Make the moves
                m.MakeMove(m, BaseBoard);

                //Look at all the depth 1 moves for the opposing side

                List<Move> moveList_1 = new List<Move>();
                List<Move> moveList_2 = new List<Move>();

                if (currentTurnState == TurnState.Defender)
                {

                    moveList_1 = m.board.GetPossibleMoves(TurnState.Attacker, m, 1);
                }
                if (currentTurnState == TurnState.Attacker)
                {
                    moveList_1 = m.board.GetPossibleMoves(TurnState.Defender, m, 1);

                }

                moveList_1.ForEach((m2) =>
                {
                    m2.MakeMove(m2, m2.parent.board);
                });

                moveList_1.ForEach((m2) =>
                {
                    //Look at all the depth 1 moves for the initial side                    

                    if (currentTurnState == TurnState.Defender)
                    {
                        moveList_2 = m2.board.GetPossibleMoves(TurnState.Defender, m2, 2);
                    }
                    if (currentTurnState == TurnState.Attacker)
                    {
                        moveList_2 = m2.board.GetPossibleMoves(TurnState.Attacker, m2, 2);

                    }

                    moveList_2.ForEach((m3) =>
                    {
                        //Make the moves
                        m3.MakeMove(m3, m3.parent.board);
                    });


                    //build List<List<Move>>
                    List<List<Move>> moveList = new List<List<Move>>();
                    moveList.Add(new List<Move>());
                    moveList[0].Add(m);
                    moveList.Add(moveList_1);
                    moveList.Add(moveList_2);

                    //Propagate scores
                    for (int i = moveList.Count - 1; i >= 0; i--)
                    {

                        //Push all the data to the depth 0 moves
                        moveList[i].ForEach((item) =>
                        {
                            if (i != 0)
                            {
                                //Total number of takes in the pipeline downwards
                                if (item.numberTakesAttacker > 0 || item.numberTakesDefender > 0)
                                {
                                    if (i == 1)
                                    {
                                        item.parent.numberTakesAttackerAtDepth[i] += item.numberTakesAttacker;
                                        item.parent.numberTakesDefenderAtDepth[i] += item.numberTakesDefender;
                                    }
                                    if (i == 2)
                                    {
                                        item.parent.parent.numberTakesAttackerAtDepth[i] += item.numberTakesAttacker;
                                        item.parent.parent.numberTakesDefenderAtDepth[i] += item.numberTakesDefender;
                                    }
                                }
                            }
                            else
                            {
                                item.numberTakesAttackerAtDepth[0] = item.numberTakesAttacker;
                                item.numberTakesDefenderAtDepth[0] = item.numberTakesDefender;
                            }


                        });

                    }

                    sage.ProcessMovesLowerMem(moveList, currentTurnState);


                });

            });

            TimeSpan depth2duration = (DateTime.Now - start);
            double runtimetodepth2 = depth2duration.TotalSeconds;

            TimeSpan duration = (DateTime.Now - start);
            double runtime = duration.TotalSeconds;
            Move bestMove = sage.PickBestLowerMem();
            bestMove.runTime = runtime;
            return bestMove;
        }

        private Square GetSelectedSquare()
        {
            List<Square> foundSquares = board.board.Where(item => item.Selected == true).ToList();
            if(foundSquares.Count < 1)
            {
                return null;
            }
            else
            {
                return foundSquares[0];
            }

        }

        public string SquareClickResponse(int column, int row)
        {

            Square clickedSquare = board.GetSquare(row, column);
            selectedSquare = GetSelectedSquare();

            if (clickedSquare != null)
            {
                //Check to see if square is occupied
                //Game already won
                if(currentTurnState == TurnState.VictoryAttacker || currentTurnState == TurnState.VictoryDefender)
                {
                    this.requestReDraw = false;
                    this.responseText = NOT_A_VALID_SQUARE;
                    return JsonConvert.SerializeObject(this);
                }
                //Clicked attacker and attacker is not ai 
                if(clickedSquare.AttackerPresent && currentTurnState == TurnState.Attacker && !attackerIsAI)
                {
                    ApplySelection(clickedSquare);
                    this.requestReDraw = true;
                    this.responseText  = "PIECE_FOUND_SELECTING";
                    return JsonConvert.SerializeObject(this);

                }
                //clicked defender and defender not ai
                if ((clickedSquare.DefenderPresent || clickedSquare.KingPresent) && currentTurnState == TurnState.Defender && !defenderIsAI)
                {
                    ApplySelection(clickedSquare);
                    this.requestReDraw = true;
                    this.responseText = "PIECE_FOUND_SELECTING";
                    return JsonConvert.SerializeObject(this);
                }
                //clicked empty square
                if (selectedSquare != null  && clickedSquare.Highlighted)
                {
                    if (!clickedSquare.AttackerPresent && !clickedSquare.DefenderPresent)
                    {
                        if (currentTurnState == TurnState.Attacker && !attackerIsAI)
                        {
                            board.MovePiece(selectedSquare.Row, selectedSquare.Column, clickedSquare.Row, clickedSquare.Column);
                            Move moveMade = new Move(selectedSquare.Column, selectedSquare.Row, clickedSquare.Column, clickedSquare.Row, null, 0);
                            moveMade.ToString();  // ensure string representation is upto date;
                            moveHistory.Add(moveMade);
                            AdvanceTurn();
                            this.requestReDraw = true;
                            this.responseText = VALID_MOVE_FOUND_EXECUTING;
                            return JsonConvert.SerializeObject(this);
                        }
                        if (currentTurnState == TurnState.Defender && !defenderIsAI)
                        {
                            board.MovePiece(selectedSquare.Row, selectedSquare.Column, clickedSquare.Row, clickedSquare.Column);
                            Move moveMade = new Move(selectedSquare.Column, selectedSquare.Row, clickedSquare.Column, clickedSquare.Row, null, 0);
                            moveMade.ToString();  // ensure string representation is upto date;
                            moveHistory.Add(moveMade);
                            AdvanceTurn();
                            this.requestReDraw = true;
                            this.responseText = VALID_MOVE_FOUND_EXECUTING;
                            return JsonConvert.SerializeObject(this);
                        }
                    }
                }
            }
            else
            {
                this.requestReDraw = false;
                this.responseText= NOT_A_VALID_SQUARE;
                return  JsonConvert.SerializeObject(this);
            }

            this.requestReDraw = false;
            this.responseText = NOT_A_VALID_SQUARE;
            return JsonConvert.SerializeObject(this); //  catch

        }

        private async void AdvanceTurn()
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