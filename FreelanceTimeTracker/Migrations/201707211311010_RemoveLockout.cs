namespace FreelanceTimeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLockout : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ClientID = c.Int(nullable: false, identity: true),
                        ClientName = c.String(),
                        Address = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ClientID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectID = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(),
                        Client_ClientID = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectID)
                .ForeignKey("dbo.Clients", t => t.Client_ClientID)
                .Index(t => t.Client_ClientID);
            
            CreateTable(
                "dbo.ProjectServices",
                c => new
                    {
                        ProjectId = c.Int(nullable: false),
                        ServiceID = c.Int(nullable: false),
                        HoursWorked = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.ServiceID })
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceID, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.ServiceID);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        ServiceiD = c.Int(nullable: false, identity: true),
                        ServiceName = c.String(),
                        Price = c.Double(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ServiceiD)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Services", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Clients", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Projects", "Client_ClientID", "dbo.Clients");
            DropForeignKey("dbo.ProjectServices", "ServiceID", "dbo.Services");
            DropForeignKey("dbo.ProjectServices", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Services", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ProjectServices", new[] { "ServiceID" });
            DropIndex("dbo.ProjectServices", new[] { "ProjectId" });
            DropIndex("dbo.Projects", new[] { "Client_ClientID" });
            DropIndex("dbo.Clients", new[] { "ApplicationUser_Id" });
            DropTable("dbo.Services");
            DropTable("dbo.ProjectServices");
            DropTable("dbo.Projects");
            DropTable("dbo.Clients");
        }
    }
}
