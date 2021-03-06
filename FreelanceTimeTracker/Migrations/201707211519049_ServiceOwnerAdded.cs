namespace FreelanceTimeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceOwnerAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "ServiceOwner", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "ServiceOwner");
        }
    }
}
