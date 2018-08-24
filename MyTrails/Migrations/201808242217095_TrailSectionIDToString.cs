namespace MyTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrailSectionIDToString : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TrailSections");
            AlterColumn("dbo.TrailSections", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.TrailSections", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TrailSections");
            AlterColumn("dbo.TrailSections", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TrailSections", "Id");
        }
    }
}
