namespace HiringFunnel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProcessInstance", "CurrentPhase", c => c.Byte());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProcessInstance", "CurrentPhase");
        }
    }
}
