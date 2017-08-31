namespace GoogleCalenderSyncUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFieldInTable1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "IsGoogleConnected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "IsGoogleConnected", c => c.String());
        }
    }
}
