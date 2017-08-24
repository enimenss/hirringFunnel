namespace HiringFunnel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ee1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contact", "CVContent", c => c.Binary());
            AddColumn("dbo.Contact", "CVContentType", c => c.String());
            DropColumn("dbo.Contact", "CV");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contact", "CV", c => c.Binary());
            DropColumn("dbo.Contact", "CVContentType");
            DropColumn("dbo.Contact", "CVContent");
        }
    }
}
