namespace MyTrails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTrailConditionLink : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Conditions", name: "TrailId_Id", newName: "TrailId");
            RenameIndex(table: "dbo.Conditions", name: "IX_TrailId_Id", newName: "IX_TrailId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Conditions", name: "IX_TrailId", newName: "IX_TrailId_Id");
            RenameColumn(table: "dbo.Conditions", name: "TrailId", newName: "TrailId_Id");
        }
    }
}
