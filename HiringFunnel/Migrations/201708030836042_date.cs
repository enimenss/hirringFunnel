namespace HiringFunnel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class date : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProcessInstance", "ContactDate", c => c.DateTime());
            AlterColumn("dbo.ProcessInstance", "InterviewDate", c => c.DateTime());
            AlterColumn("dbo.ProcessInstance", "TestDate", c => c.DateTime());
            AlterColumn("dbo.ProcessInstance", "TestDefenseDate", c => c.DateTime());
            AlterColumn("dbo.ProcessInstance", "OfferDate", c => c.DateTime());
            AlterColumn("dbo.ProcessInstance", "AcceptanceDate", c => c.DateTime());
            AlterColumn("dbo.ProcessInstance", "RejectionDate", c => c.DateTime());
            AlterColumn("dbo.ProcessInstance", "QuitDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProcessInstance", "QuitDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProcessInstance", "RejectionDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProcessInstance", "AcceptanceDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProcessInstance", "OfferDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProcessInstance", "TestDefenseDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProcessInstance", "TestDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProcessInstance", "InterviewDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProcessInstance", "ContactDate", c => c.DateTime(nullable: false));
        }
    }
}
