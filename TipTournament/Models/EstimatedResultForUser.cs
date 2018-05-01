using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TipTournament.Models
{
    public class EstimatedResultForUser
    {
        private List<EstimatedModel> results;

        public EstimatedResultForUser(List<EstimatedModel> results)
        {
            this.results = results;
        }

        public bool HasEstimatedResultForMatch(int matchId)
        {
            return GetByMatchId(matchId) != null;
        }

        public EstimatedModel GetByMatchId(int matchId)
        {
            return results.Count > 0 ? this.results.FirstOrDefault(item => item.MatchId == matchId) : null;
        }

        public string GetEstimatedResultByUserAndMatchId(string userId, int matchId)
        {
            var model = this.results.FirstOrDefault(item => item.MatchId == matchId && item.UserId.Equals(userId));
            return model?.ToString() ?? string.Empty;
        }
    }
}