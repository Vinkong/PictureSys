namespace PicManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPicClass1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PictureInfoes",
                c => new
                    {
                        PicId = c.Int(nullable: false, identity: true),
                        PicName = c.String(),
                        Latitude = c.String(),
                        Longitude = c.String(),
                        TackDate = c.DateTime(nullable: false),
                        TackPalce = c.String(),
                    })
                .PrimaryKey(t => t.PicId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PictureInfoes");
        }
    }
}
