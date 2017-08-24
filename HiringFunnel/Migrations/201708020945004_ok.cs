namespace HiringFunnel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ok : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Process", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Process", "EndDate", c => c.DateTime(nullable: false));
        }
    }
}
