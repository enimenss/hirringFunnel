namespace HiringFunnel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uptade : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ContactTechnology", newName: "TechnologyContact");
            DropPrimaryKey("dbo.TechnologyContact");
            CreateTable(
                "dbo.Interviewer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Byte(nullable: false),
                        InterviewerId = c.Int(nullable: false),
                        PInsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProcessInstance", t => t.PInsId)
                .ForeignKey("dbo.User", t => t.InterviewerId)
                .Index(t => t.InterviewerId)
                .Index(t => t.PInsId);
            
            AddPrimaryKey("dbo.TechnologyContact", new[] { "Technology_Id", "Contact_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Interviewer", "InterviewerId", "dbo.User");
            DropForeignKey("dbo.Interviewer", "PInsId", "dbo.ProcessInstance");
            DropIndex("dbo.Interviewer", new[] { "PInsId" });
            DropIndex("dbo.Interviewer", new[] { "InterviewerId" });
            DropPrimaryKey("dbo.TechnologyContact");
            DropTable("dbo.Interviewer");
            AddPrimaryKey("dbo.TechnologyContact", new[] { "Contact_Id", "Technology_Id" });
            RenameTable(name: "dbo.TechnologyContact", newName: "ContactTechnology");
        }
    }
}
