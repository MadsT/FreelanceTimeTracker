namespace FreelanceTimeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOwnerToClient : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Clients", name: "ApplicationUser_Id", newName: "ClientOwner_Id");
            RenameIndex(table: "dbo.Clients", name: "IX_ApplicationUser_Id", newName: "IX_ClientOwner_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Clients", name: "IX_ClientOwner_Id", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.Clients", name: "ClientOwner_Id", newName: "ApplicationUser_Id");
        }
    }
}
