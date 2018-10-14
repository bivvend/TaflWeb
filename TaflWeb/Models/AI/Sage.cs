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
    /// Evaluator used to compare the scores of all the other evaluators
    /// </summary>
    public class Sage
    {
        General general { get; set; }
        Assassin assassin { get; set; }
        KingsCouncil kingsCouncil { get; set; }

        public double weightGeneral { get; set; } = 0.0d;
        public double weightAssassin { get; set; } = 0.0d;
        public double weightKingsCouncil { get; set; } = 0.0d;

        public List<List<Move>>  inputMoveList { get; set; }
        public List<Move> bestList { get; set; }  //Used for High mem calcs
        public List<Move> longTermBestList { get; set; } // Used for lowMem version of calcs
        public Move suggestedMove { get; set; } //Used for High mem calcs
        public Move longTermSuggestedMove { get; set; } // Used for lowMem version of calcs

        public TurnState currentTurnState { get; set; } = TurnState.Attacker;

        private readonly Object locker = new Object();

        public Sage()
        {
            general = new General();
            assassin = new Assassin();
            kingsCouncil = new KingsCouncil();

            weightAssassin = 1.0;
            weightGeneral = 1.0;
            weightKingsCouncil = 1.0;
            longTermBestList = new List<Move>();
        }

        
        public void ProcessMovesLowerMem(List<List<Move>> input, TurnState turnState)
        {
            try
            {
                this.inputMoveList = input;
                this.currentTurnState = turnState;


                lock (locker)
                {
                    //Ask general to process moves            
                    longTermBestList.AddRange(general.EvaluateLowMem(inputMoveList, turnState));

                    if (turnState == TurnState.Attacker)
                    {
                        longTermBestList.AddRange(assassin.Evaluate(inputMoveList, turnState));
                    }

                    if (turnState == TurnState.Defender)
                    {
                        longTermBestList.AddRange(kingsCouncil.Evaluate(inputMoveList, turnState));
                    }
                }
            }
            catch(Exception ex)
            {
                int i = 0; 
            }

        }

        public Move PickBestLowerMem()
        {
            longTermBestList.ForEach((item) =>
            {

                item.scoreSage = item.scoreGeneral * weightGeneral + item.scoreKingsCouncil * weightKingsCouncil + item.scoreAssassin * weightAssassin;

            });

            //pick best to return
            Move suggestedMove = longTermBestList.MaxObject((item) => item.scoreSage);
            return suggestedMove;

        }



    }
}
