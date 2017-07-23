namespace FreelanceTimeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectSelectedClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "SelectedClient", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "SelectedClient");
        }
    }
}
