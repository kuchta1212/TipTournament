using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TipTournament.Models
{
    public class RankingModel
    {

        public int Id { get; set;}

        public string UserId { get; set; }

        public int Points { get; set; }

    }

    public class RankingDbContext : EFDbContext
    {
        public DbSet<RankingModel> Ranking { get; set; }
    }
}