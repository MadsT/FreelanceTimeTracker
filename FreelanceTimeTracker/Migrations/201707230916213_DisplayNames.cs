namespace FreelanceTimeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DisplayNames : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clients", "ClientName", c => c.String(nullable: false));
            AlterColumn("dbo.Clients", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.Projects", "ProjectName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Projects", "ProjectName", c => c.String());
            AlterColumn("dbo.Clients", "Address", c => c.String());
            AlterColumn("dbo.Clients", "ClientName", c => c.String());
        }
    }
}
