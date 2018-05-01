using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    public class PointsCounter
    {

        public void CountPoints(Dictionary<int, ResultModel> newResults)
        {
            var rankingModels = RankingController.GetRanking();
            foreach (RankingModel rankingModel in rankingModels)
            {
                var userId = rankingModel.UserId;
                foreach (KeyValuePair<int, ResultModel> results in newResults)
                {
                    var estimatedResult = EstimatedResultController.GetEstimatedResultModelForUserAndMatch(userId, results.Key);
                    if (estimatedResult != null)
                    {
                        Evaulation evaulation = this.CompareResultAndEstimatedResult(results.Value, estimatedResult);
                        RankingController.UpdatePoints(userId, (int)evaulation);
                    }
                }
            }
        }

        private Evaulation CompareResultAndEstimatedResult(ResultModel realResult, EstimatedModel estimatedResult)
        {
            int realOne = realResult.ValueOne;
            int realTwo = realResult.ValueTwo;

            int estimatedOne = estimatedResult.One;
            int estimatedTwo = estimatedResult.Two;

            return Compare(realOne, realTwo, estimatedOne, estimatedTwo);
        }

        private static Evaulation Compare(int realOne, int realTwo, int estimatedOne, int estimatedTwo)
        { 
            if ((realOne == estimatedOne) && (realTwo == estimatedTwo))
                return Evaulation.VICTORY;

            int realDifference = realOne - realTwo;
            int estimatedDifferenec = estimatedOne - estimatedTwo;

            if (realDifference == estimatedDifferenec)
                return Evaulation.DIFFERENCE;

            if(estimatedDifferenec == 0) // tip was tie but there was no tie
                return Evaulation.LOST;

            if (realDifference != 0)// if real was no tie then there was a winner
            {
                bool realWinnerFirst = realDifference > 0;
                bool estimatedWinnerFirst = estimatedDifferenec > 0;

                if (realWinnerFirst == estimatedWinnerFirst)
                    return Evaulation.WINNER;
            }
            

            return Evaulation.LOST;
        }


        public static string CountForView(string realResult, string estimatedResult)
        {
            string[] realStr = realResult.Split(':');
            int realOne = Int32.Parse(realStr[0]);
            int realTwo = Int32.Parse(realStr[1]);

            string[] estimStr = estimatedResult.Split(':');
            int estOne = Int32.Parse(estimStr[0]);
            int estTwo = Int32.Parse(estimStr[1]);

            return ((int) Compare(realOne, realTwo, estOne, estTwo)).ToString();
        }
    }


    public enum Evaulation
    {
        //CORRECT RESULT AND CORRECT WINNER
        VICTORY     = 4,
        //CORRECT WINNER AND CORRECT DIFFERNECE IN GOALS
        DIFFERENCE	= 2,
        //CORRECT WINNER
        WINNER      = 1,
        //WRONG
        LOST	    = 0
        
    }
}