namespace FreelanceTimeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientOnProject : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Projects", "Client_ClientID", "dbo.Clients");
            DropIndex("dbo.Projects", new[] { "Client_ClientID" });
            RenameColumn(table: "dbo.Projects", name: "Client_ClientID", newName: "ClientID");
            AlterColumn("dbo.Projects", "ClientID", c => c.Int(nullable: false));
            CreateIndex("dbo.Projects", "ClientID");
            AddForeignKey("dbo.Projects", "ClientID", "dbo.Clients", "ClientID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "ClientID", "dbo.Clients");
            DropIndex("dbo.Projects", new[] { "ClientID" });
            AlterColumn("dbo.Projects", "ClientID", c => c.Int());
            RenameColumn(table: "dbo.Projects", name: "ClientID", newName: "Client_ClientID");
            CreateIndex("dbo.Projects", "Client_ClientID");
            AddForeignKey("dbo.Projects", "Client_ClientID", "dbo.Clients", "ClientID");
        }
    }
}
