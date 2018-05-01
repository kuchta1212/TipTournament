namespace TipTournament.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableEstimated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EstimatedModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatchId = c.Int(nullable: false),
                        UserId = c.String(),
                        One = c.Int(nullable: false),
                        Two = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MatchModels", t => t.MatchId, cascadeDelete: true)
                .Index(t => t.MatchId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EstimatedModels", "MatchId", "dbo.MatchModels");
            DropIndex("dbo.EstimatedModels", new[] { "MatchId" });
            DropTable("dbo.EstimatedModels");
        }
    }
}
