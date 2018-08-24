namespace MyTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDBGeography : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Points", "Trail_Id", "dbo.Trails");
            DropIndex("dbo.Points", new[] { "Trail_Id" });
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrailSections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShortDescription = c.String(),
                        Geography = c.Geography(),
                        Status = c.String(),
                        TrailID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Trails", t => t.TrailID)
                .Index(t => t.TrailID);
            
            CreateTable(
                "dbo.PostTrails",
                c => new
                    {
                        Post_Id = c.String(nullable: false, maxLength: 128),
                        Trail_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Post_Id, t.Trail_Id })
                .ForeignKey("dbo.Posts", t => t.Post_Id, cascadeDelete: true)
                .ForeignKey("dbo.Trails", t => t.Trail_Id, cascadeDelete: true)
                .Index(t => t.Post_Id)
                .Index(t => t.Trail_Id);
            
            AddColumn("dbo.Trails", "TotalMiles", c => c.Single());
            DropColumn("dbo.Trails", "Miles");
            DropColumn("dbo.Trails", "Geography");
            DropTable("dbo.Points");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Points",
                c => new
                    {
                        Latitude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Longitude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Elevation = c.Int(nullable: false),
                        Trail_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Latitude, t.Longitude });
            
            AddColumn("dbo.Trails", "Geography", c => c.Geography());
            AddColumn("dbo.Trails", "Miles", c => c.Single());
            DropForeignKey("dbo.TrailSections", "TrailID", "dbo.Trails");
            DropForeignKey("dbo.PostTrails", "Trail_Id", "dbo.Trails");
            DropForeignKey("dbo.PostTrails", "Post_Id", "dbo.Posts");
            DropIndex("dbo.PostTrails", new[] { "Trail_Id" });
            DropIndex("dbo.PostTrails", new[] { "Post_Id" });
            DropIndex("dbo.TrailSections", new[] { "TrailID" });
            DropColumn("dbo.Trails", "TotalMiles");
            DropTable("dbo.PostTrails");
            DropTable("dbo.TrailSections");
            DropTable("dbo.Posts");
            CreateIndex("dbo.Points", "Trail_Id");
            AddForeignKey("dbo.Points", "Trail_Id", "dbo.Trails", "Id");
        }
    }
}
