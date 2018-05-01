using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;

namespace TipTournament.Models
{
    public class ResultModel
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ValueOne { get; set; }
        public int ValueTwo { get; set; }
       
        public int IsImported { get; set; }

        public string Date { get; set; }

        public override string ToString()
        {
            return ValueOne + " : " + ValueTwo;
        }

    }

    public class ResultsDbContext : EFDbContext
    {
        public DbSet<ResultModel> Results { get; set; }
//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<ResultModel>().ToTable("RESULT");
//        }
    }
}