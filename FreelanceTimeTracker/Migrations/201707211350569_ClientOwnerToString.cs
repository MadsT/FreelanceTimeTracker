namespace FreelanceTimeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientOwnerToString : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Clients", name: "ClientOwner_Id", newName: "ApplicationUser_Id");
            RenameIndex(table: "dbo.Clients", name: "IX_ClientOwner_Id", newName: "IX_ApplicationUser_Id");
            AddColumn("dbo.Clients", "ClientOwner", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "ClientOwner");
            RenameIndex(table: "dbo.Clients", name: "IX_ApplicationUser_Id", newName: "IX_ClientOwner_Id");
            RenameColumn(table: "dbo.Clients", name: "ApplicationUser_Id", newName: "ClientOwner_Id");
        }
    }
}
