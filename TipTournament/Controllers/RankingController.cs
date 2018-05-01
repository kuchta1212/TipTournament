using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    public class RankingController
    {

        private static RankingDbContext rankingDbContext = new RankingDbContext();

        public static List<RankingModel> GetRanking()
        {
            return rankingDbContext.Ranking.OrderByDescending(item => item.Points).ToList();
        }

        public static RankingModel GetRankingForUser(string userId)
        {
            return GetRanking().FirstOrDefault(item => item.UserId == userId);
        }

        public static void UpdatePoints(string userId, int addedPoints)
        {
            RankingModel ranking = GetRankingForUser(userId);
            ranking.Points += addedPoints;
            SaveChanges();
        }

        public static void AddUser(string userId)
        {
            rankingDbContext.Ranking.Add(new RankingModel() {UserId = userId, Points = 0});
            SaveChanges();
        }

        public static void SaveChanges()
        {
            rankingDbContext.SaveChanges();
        }

        public static bool IsUserSaved(string userId)
        {
            return GetRankingForUser(userId) != null;
        }
    }
}