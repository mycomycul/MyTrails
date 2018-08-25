namespace MyTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Conditions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PercentSnowCover = c.Int(nullable: false),
                        Description = c.String(),
                        Date = c.DateTime(),
                        TrailId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Trails", t => t.TrailId)
                .Index(t => t.TrailId);
            
            CreateTable(
                "dbo.Trails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TrailName = c.String(),
                        Zone = c.String(),
                        Description = c.String(),
                        TotalMiles = c.Single(),
                        Status = c.String(),
                        Agency = c.String(),
                        InfoHTMLLink = c.String(),
                        Elevation = c.String(),
                        ShortDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        Id = c.String(nullable: false, maxLength: 128),
                        ShortDescription = c.String(),
                        Geography = c.Geography(),
                        Status = c.String(),
                        TrailID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Trails", t => t.TrailID)
                .Index(t => t.TrailID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.TrailSections", "TrailID", "dbo.Trails");
            DropForeignKey("dbo.PostTrails", "Trail_Id", "dbo.Trails");
            DropForeignKey("dbo.PostTrails", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.Conditions", "TrailId", "dbo.Trails");
            DropIndex("dbo.PostTrails", new[] { "Trail_Id" });
            DropIndex("dbo.PostTrails", new[] { "Post_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TrailSections", new[] { "TrailID" });
            DropIndex("dbo.Conditions", new[] { "TrailId" });
            DropTable("dbo.PostTrails");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.TrailSections");
            DropTable("dbo.Posts");
            DropTable("dbo.Trails");
            DropTable("dbo.Conditions");
        }
    }
}
