namespace HiringFunnel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Annotation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Type = c.Byte(nullable: false),
                        Hidden = c.Boolean(nullable: false),
                        PInsId = c.Int(nullable: false),
                        ContactId = c.Int(nullable: false),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.AuthorId)
                .ForeignKey("dbo.ProcessInstance", t => t.PInsId)
                .ForeignKey("dbo.Contact", t => t.ContactId)
                .Index(t => t.PInsId)
                .Index(t => t.ContactId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        PasswordHash = c.String(nullable: false),
                        Seniority = c.Byte(nullable: false),
                        Role = c.Byte(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Technology",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        Location = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        LinkedIn = c.String(),
                        CV = c.Binary(),
                        Seniority = c.Byte(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProcessInstance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        ContactDate = c.DateTime(nullable: false),
                        InterviewScheduled = c.Boolean(nullable: false),
                        InterviewNoticed = c.Boolean(nullable: false),
                        InterviewHeld = c.Boolean(nullable: false),
                        InterviewDate = c.DateTime(nullable: false),
                        TestScheduled = c.Boolean(nullable: false),
                        TestNoticed = c.Boolean(nullable: false),
                        TestHeld = c.Boolean(nullable: false),
                        TestDate = c.DateTime(nullable: false),
                        TestDefenseScheduled = c.Boolean(nullable: false),
                        TestDefenseNoticed = c.Boolean(nullable: false),
                        TestDefenseHeld = c.Boolean(nullable: false),
                        TestDefenseDate = c.DateTime(nullable: false),
                        OfferSent = c.Boolean(nullable: false),
                        OfferDate = c.DateTime(nullable: false),
                        Accepted = c.Boolean(nullable: false),
                        AcceptanceDate = c.DateTime(nullable: false),
                        Rejected = c.Boolean(nullable: false),
                        RejectionDate = c.DateTime(nullable: false),
                        Quit = c.Boolean(nullable: false),
                        QuitDate = c.DateTime(nullable: false),
                        ContactId = c.Int(nullable: false),
                        ProcessId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contact", t => t.ContactId)
                .ForeignKey("dbo.Process", t => t.ProcessId)
                .Index(t => t.ContactId)
                .Index(t => t.ProcessId);
            
            CreateTable(
                "dbo.Process",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Technologies = c.String(nullable: false),
                        Seniority = c.Byte(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ToDoItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(nullable: false),
                        Completed = c.Boolean(nullable: false),
                        Time = c.DateTime(nullable: false),
                        PInsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProcessInstance", t => t.PInsId)
                .Index(t => t.PInsId);
            
            CreateTable(
                "dbo.ContactTechnology",
                c => new
                    {
                        Contact_Id = c.Int(nullable: false),
                        Technology_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Contact_Id, t.Technology_Id })
                .ForeignKey("dbo.Contact", t => t.Contact_Id, cascadeDelete: true)
                .ForeignKey("dbo.Technology", t => t.Technology_Id, cascadeDelete: true)
                .Index(t => t.Contact_Id)
                .Index(t => t.Technology_Id);
            
            CreateTable(
                "dbo.TechnologyUser",
                c => new
                    {
                        Technology_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Technology_Id, t.User_Id })
                .ForeignKey("dbo.Technology", t => t.Technology_Id, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Technology_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Annotation", "ContactId", "dbo.Contact");
            DropForeignKey("dbo.Annotation", "PInsId", "dbo.ProcessInstance");
            DropForeignKey("dbo.Annotation", "AuthorId", "dbo.User");
            DropForeignKey("dbo.TechnologyUser", "User_Id", "dbo.User");
            DropForeignKey("dbo.TechnologyUser", "Technology_Id", "dbo.Technology");
            DropForeignKey("dbo.ContactTechnology", "Technology_Id", "dbo.Technology");
            DropForeignKey("dbo.ContactTechnology", "Contact_Id", "dbo.Contact");
            DropForeignKey("dbo.ToDoItem", "PInsId", "dbo.ProcessInstance");
            DropForeignKey("dbo.ProcessInstance", "ProcessId", "dbo.Process");
            DropForeignKey("dbo.ProcessInstance", "ContactId", "dbo.Contact");
            DropIndex("dbo.TechnologyUser", new[] { "User_Id" });
            DropIndex("dbo.TechnologyUser", new[] { "Technology_Id" });
            DropIndex("dbo.ContactTechnology", new[] { "Technology_Id" });
            DropIndex("dbo.ContactTechnology", new[] { "Contact_Id" });
            DropIndex("dbo.ToDoItem", new[] { "PInsId" });
            DropIndex("dbo.ProcessInstance", new[] { "ProcessId" });
            DropIndex("dbo.ProcessInstance", new[] { "ContactId" });
            DropIndex("dbo.Annotation", new[] { "AuthorId" });
            DropIndex("dbo.Annotation", new[] { "ContactId" });
            DropIndex("dbo.Annotation", new[] { "PInsId" });
            DropTable("dbo.TechnologyUser");
            DropTable("dbo.ContactTechnology");
            DropTable("dbo.ToDoItem");
            DropTable("dbo.Process");
            DropTable("dbo.ProcessInstance");
            DropTable("dbo.Contact");
            DropTable("dbo.Technology");
            DropTable("dbo.User");
            DropTable("dbo.Annotation");
        }
    }
}
