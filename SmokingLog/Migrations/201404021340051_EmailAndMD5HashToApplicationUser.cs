namespace SmokingLog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailAndMD5HashToApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "EmailAddress", c => c.String());
            AddColumn("dbo.AspNetUsers", "EmailMD5", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "EmailMD5");
            DropColumn("dbo.AspNetUsers", "EmailAddress");
        }
    }
}
