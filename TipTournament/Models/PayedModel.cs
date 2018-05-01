using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TipTournament.Models
{
    public class PayedModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public bool IsPayed { get; set; }
    }

    public class PayedDbContext : EFDbContext
    {
        public DbSet<PayedModel> Payed { get; set; }
    }
}