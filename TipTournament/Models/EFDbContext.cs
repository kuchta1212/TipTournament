using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TipTournament.Models
{
    public class EFDbContext : DbContext
    {
        public EFDbContext(): base("name=Tournament")
        {}
    }
}