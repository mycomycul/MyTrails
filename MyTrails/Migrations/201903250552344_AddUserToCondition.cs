namespace MyTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserToCondition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Conditions", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Conditions", "UserId");
            AddForeignKey("dbo.Conditions", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Conditions", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Conditions", new[] { "UserId" });
            DropColumn("dbo.Conditions", "UserId");
        }
    }
}
