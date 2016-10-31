namespace FeedbackSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Votes", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Votes", "UserId");
            AddForeignKey("dbo.Votes", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Votes", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Votes", new[] { "UserId" });
            DropColumn("dbo.Votes", "UserId");
        }
    }
}
