using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Net.Mime;
using System.Web;

namespace TipTournament.Models
{
    public class EstimatedModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Match")]
        public int MatchId { get; set; }
        public virtual MatchModel Match { get; set; }

        public string UserId { get; set; }

        public int One { get; set; }
        public int Two { get; set; }

        public override string ToString()
        {
            return One + " : " + Two;
        }
    }

    public class EstimatedDbContext : EFDbContext
    {
        public DbSet<EstimatedModel> EstimatedResults { get; set; }
    }
}