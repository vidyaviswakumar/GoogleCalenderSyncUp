namespace GoogleCalenderSyncUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFieldInTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsGoogleConnected", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsGoogleConnected");
        }
    }
}
