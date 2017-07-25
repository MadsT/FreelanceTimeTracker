namespace FreelanceTimeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredSelectedClient : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Projects", "SelectedClient", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Projects", "SelectedClient", c => c.String());
        }
    }
}
