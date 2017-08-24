namespace HiringFunnel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hehess : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Annotation", new[] { "PInsId" });
            DropIndex("dbo.Annotation", new[] { "ContactId" });
            AlterColumn("dbo.Annotation", "PInsId", c => c.Int());
            AlterColumn("dbo.Annotation", "ContactId", c => c.Int());
            CreateIndex("dbo.Annotation", "PInsId");
            CreateIndex("dbo.Annotation", "ContactId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Annotation", new[] { "ContactId" });
            DropIndex("dbo.Annotation", new[] { "PInsId" });
            AlterColumn("dbo.Annotation", "ContactId", c => c.Int(nullable: false));
            AlterColumn("dbo.Annotation", "PInsId", c => c.Int(nullable: false));
            CreateIndex("dbo.Annotation", "ContactId");
            CreateIndex("dbo.Annotation", "PInsId");
        }
    }
}
