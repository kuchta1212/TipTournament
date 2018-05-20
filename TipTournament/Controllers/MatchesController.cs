using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    public class MatchesController
    {
        private static MatchcesDbContext matchesContext = new MatchcesDbContext();


        public static List<MatchModel> GetMatches()
        {
            return matchesContext.Matches.ToList();
        }

        public static void AddMatch(MatchModel model)
        {
            matchesContext.Matches.Add(model);
        }

        public static MatchModel GetMatch(string one, string two)
        {
            return matchesContext.Matches
                .FirstOrDefault(item => item.TeamOne.Equals(one) && item.TeamTwo.Equals(two));
        }

        public static int SaveChanges()
        {
            return matchesContext.SaveChanges();
        }

        public static int Count()
        {
            return matchesContext.Matches.Count();
        }


        public static bool MatchExists(string one, string two)
        {
            return GetMatch(one, two) != null;
        }

        public static void DeleteMatch(int matchId)
        {
            var matchModels = matchesContext.Matches.First(x => x.Id == matchId);
            matchesContext.Matches.Remove(matchModels);
            matchesContext.SaveChanges();

        }


    }
}
