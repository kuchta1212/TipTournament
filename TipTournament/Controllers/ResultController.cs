using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    public class ResultController
    {

        private static ResultsDbContext resultContext = new ResultsDbContext();

        public static List<ResultModel> GeResults()
        {
            return resultContext.Results.ToList();
        }

        public static void AddResult(ResultModel model)
        {
            resultContext.Results.Add(model);
        }

        public static ResultModel GetResult(int id)
        {
            return resultContext.Results.FirstOrDefault(item => item.Id == id);
        }

        public static int SaveChanges()
        {
            return resultContext.SaveChanges();
        }

    }
}