namespace PicManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPicUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PictureInfoes", "UserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PictureInfoes", "UserId");
        }
    }
}
