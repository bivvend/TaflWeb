using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaflWeb.Model.AI;
using TaflWeb.Model.Classes;
using static TaflWeb.Models.Classes.TurnDefinitions;

namespace TaflWeb.Model.AI
{
    /// <summary>
    /// Class look to weigh up piece advantages by likely hood of takes
    /// </summary>    

    public class General
    {

        public double desireToTakeWhenAttacker = 100.0;
        public double desireToTakeWhenDefender = 10.0;

        public double desireToAvoidTakeAttacker = 6.0;
        public double desireToAvoidTakeDefender = 2.0;

        public double desireToTakeWhenAttackerDepth2 = 10.0;
        public double desireToTakeWhenDefenderDepth2 = 1.0;

        public List<TaflWeb.Model.Classes.Move> EvaluateLowMem(List<List<Move>> inputMoveList, TurnState currentTurnState)
        {
            List<Move> suggestedMoves = new List<Move>();

            //Get some scaling constants

            double maxDefenderTakeDepth2 = 100;
            double maxDefenderTakeDepth1 = 10;
            double maxDefenderTakeDepth0 = 1;
            double maxAttackerTakeDepth2 = 100;
            double maxAttackerTakeDepth1 = 10;
            double maxAttackerTakeDepth0 = 1;

            //ensure no div by zero
            if (maxDefenderTakeDepth0 < 1.0)
                maxDefenderTakeDepth0 = 1.0;

            if (maxDefenderTakeDepth1 < 1.0)
                maxDefenderTakeDepth1 = 1.0;

            if (maxDefenderTakeDepth2 < 1.0)
                maxDefenderTakeDepth2 = 1.0;

            //ensure no div by zero
            if (maxAttackerTakeDepth0 < 1.0)
                maxAttackerTakeDepth0 = 1.0;

            if (maxAttackerTakeDepth1 < 1.0)
                maxAttackerTakeDepth1 = 1.0;

            if (maxAttackerTakeDepth2 < 1.0)
                maxAttackerTakeDepth2 = 1.0;

            //Pick the best
            if (currentTurnState == TurnState.Defender)
            {
                suggestedMoves.Add(inputMoveList[0].MaxObject((item) => (double)item.numberTakesDefenderAtDepth[0] * (desireToTakeWhenDefender / (maxDefenderTakeDepth0)) - (double)item.numberTakesAttackerAtDepth[1] * (desireToAvoidTakeDefender / maxAttackerTakeDepth1) + (double)item.numberTakesDefenderAtDepth[2] * (desireToTakeWhenDefenderDepth2 / maxDefenderTakeDepth2)));
                suggestedMoves.ForEach((item) =>
                {
                    item.scoreGeneral = (double)item.numberTakesDefenderAtDepth[0] * (desireToTakeWhenDefender / maxDefenderTakeDepth0) - (double)item.numberTakesAttackerAtDepth[1] * (desireToAvoidTakeDefender / maxAttackerTakeDepth1) + (double)item.numberTakesDefenderAtDepth[2] * (desireToTakeWhenDefenderDepth2 / maxDefenderTakeDepth2);
                });

            }
            else if (currentTurnState == TurnState.Attacker)
            {
                suggestedMoves.Add(inputMoveList[0].MaxObject((item) => (double)item.numberTakesAttackerAtDepth[0] * (desireToTakeWhenAttacker / maxAttackerTakeDepth0) - (double)item.numberTakesDefenderAtDepth[1] * (desireToAvoidTakeAttacker / maxDefenderTakeDepth1) + (double)item.numberTakesAttackerAtDepth[2] * (desireToTakeWhenAttackerDepth2 / maxAttackerTakeDepth2)));
                suggestedMoves.ForEach((item) =>
                {
                    item.scoreGeneral = (double)item.numberTakesAttackerAtDepth[0] * (desireToTakeWhenAttacker / maxAttackerTakeDepth0) - (double)item.numberTakesDefenderAtDepth[1] * (desireToAvoidTakeAttacker / maxDefenderTakeDepth1) + (double)item.numberTakesAttackerAtDepth[2] * (desireToTakeWhenAttackerDepth2 / maxAttackerTakeDepth2);
                    if (item.scoreGeneral > 1.0)
                    {
                        int i = 0;
                    }
                });
            }

            return suggestedMoves;

        }

        public List<Move> Evaluate(List<List<Move>> inputMoveList, TurnState currentTurnState)
        {
            List<Move> suggestedMoves = new List<Move>();
            
            //Get some scaling constants

            Move TestMove = inputMoveList[0].MaxObject((item) => item.numberTakesDefenderAtDepth[2]);
            double maxDefenderTakeDepth2 = (double)TestMove.numberTakesDefenderAtDepth[2];

            TestMove = inputMoveList[0].MaxObject((item) => item.numberTakesDefenderAtDepth[1]);
            double maxDefenderTakeDepth1 = (double)TestMove.numberTakesDefenderAtDepth[1];

            TestMove = inputMoveList[0].MaxObject((item) => item.numberTakesDefenderAtDepth[0]);
            double maxDefenderTakeDepth0 = (double)TestMove.numberTakesDefenderAtDepth[0];

            TestMove = inputMoveList[0].MaxObject((item) => item.numberTakesAttackerAtDepth[2]);
            double maxAttackerTakeDepth2 = (double)TestMove.numberTakesAttackerAtDepth[2];

            TestMove = inputMoveList[0].MaxObject((item) => item.numberTakesAttackerAtDepth[1]);
            double maxAttackerTakeDepth1 = (double)TestMove.numberTakesAttackerAtDepth[1];

            TestMove = inputMoveList[0].MaxObject((item) => item.numberTakesAttackerAtDepth[0]);
            double maxAttackerTakeDepth0 = (double)TestMove.numberTakesAttackerAtDepth[0];

            //ensure no div by zero
            if (maxDefenderTakeDepth0 <1.0)
                maxDefenderTakeDepth0 = 1.0;

            if (maxDefenderTakeDepth1 < 1.0)
                maxDefenderTakeDepth1 = 1.0;

            if (maxDefenderTakeDepth2 < 1.0)
                maxDefenderTakeDepth2 = 1.0;

            //ensure no div by zero
            if (maxAttackerTakeDepth0 < 1.0)
                maxAttackerTakeDepth0 = 1.0;

            if (maxAttackerTakeDepth1 < 1.0)
                maxAttackerTakeDepth1 = 1.0;

            if (maxAttackerTakeDepth2 < 1.0)
                maxAttackerTakeDepth2 = 1.0;

            //Pick the best
            if (currentTurnState == TurnState.Defender)
            {
                suggestedMoves.Add(inputMoveList[0].MaxObject((item) => (double)item.numberTakesDefenderAtDepth[0] * (desireToTakeWhenDefender/(maxDefenderTakeDepth0)) - (double)item.numberTakesAttackerAtDepth[1] * (desireToAvoidTakeDefender/maxAttackerTakeDepth1) + (double)item.numberTakesDefenderAtDepth[2]* (desireToTakeWhenDefenderDepth2/maxDefenderTakeDepth2)));
                suggestedMoves.ForEach((item) =>
                {
                    item.scoreGeneral = (double)item.numberTakesDefenderAtDepth[0] * (desireToTakeWhenDefender / maxDefenderTakeDepth0) - (double)item.numberTakesAttackerAtDepth[1] * (desireToAvoidTakeDefender/ maxAttackerTakeDepth1) + (double)item.numberTakesDefenderAtDepth[2] * (desireToTakeWhenDefenderDepth2/ maxDefenderTakeDepth2);
                });

            }
            else if (currentTurnState == TurnState.Attacker)
            {
                suggestedMoves.Add(inputMoveList[0].MaxObject((item) => (double)item.numberTakesAttackerAtDepth[0] * (desireToTakeWhenAttacker/maxAttackerTakeDepth0) - (double)item.numberTakesDefenderAtDepth[1] *(desireToAvoidTakeAttacker/maxDefenderTakeDepth1) + (double)item.numberTakesAttackerAtDepth[2]*(desireToTakeWhenAttackerDepth2/maxAttackerTakeDepth2)));
                suggestedMoves.ForEach((item) =>
                {
                    item.scoreGeneral = (double)item.numberTakesAttackerAtDepth[0] * (desireToTakeWhenAttacker / maxAttackerTakeDepth0) - (double)item.numberTakesDefenderAtDepth[1] * (desireToAvoidTakeAttacker / maxDefenderTakeDepth1) + (double)item.numberTakesAttackerAtDepth[2] * (desireToTakeWhenAttackerDepth2 / maxAttackerTakeDepth2);
                    if (item.scoreGeneral > 1.0)
                    {
                        int i = 0;
                    }
                });
            }

            return suggestedMoves;
            
        }
    }
}
