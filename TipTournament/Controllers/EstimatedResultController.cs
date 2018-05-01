using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    public class EstimatedResultController
    {

        private static EstimatedDbContext _estimatedContext = new EstimatedDbContext();

        public static void AddEstimatedResult(string userId, int matchId, int one, int two)
        {
            _estimatedContext.EstimatedResults.Add(
                new EstimatedModel()
                {
                    UserId = userId,
                    MatchId = matchId,
                    One = one,
                    Two = two
                });
            SaveChanges();
        }

        public static List<EstimatedModel> GetEstimatedResultModels()
        {
            return _estimatedContext.EstimatedResults.ToList();
        }

        public static List<EstimatedModel> GetEstimatedResultModelsForUser(string userId)
        {
            return GetEstimatedResultModels().Where(item => item.UserId == userId).ToList();
        }

        public static EstimatedModel GetEstimatedResultModelForUserAndMatch(string userId, int matchId)
        {
            return GetEstimatedResultModelsForUser(userId).FirstOrDefault(item => item.MatchId == matchId);
        }

        public static void SaveChanges()
        {
            _estimatedContext.SaveChanges();
        }
    }
}