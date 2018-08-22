namespace MyTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    
    public partial class AddGeography : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trails", "Status", c => c.String());
            AddColumn("dbo.Trails", "Geography", c => c.Geography());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trails", "Geography");
            DropColumn("dbo.Trails", "Status");
        }
    }
}
