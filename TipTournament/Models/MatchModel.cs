using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;

namespace TipTournament.Models
{
    public class MatchModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Result")]
        public int ResultId { get; set; }
        public virtual ResultModel Result { get; set; }

        public string TeamOne { get; set; }

        public string TeamTwo { get; set; }


    }

    public class MatchcesDbContext : EFDbContext
    {
        public DbSet<MatchModel> Matches { get; set; }
    }

}