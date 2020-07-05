namespace PicManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConfirmPassWord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ConfirmPwd", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "ShowUserName", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "PassWord", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "PassWord", c => c.String());
            AlterColumn("dbo.Users", "ShowUserName", c => c.String());
            AlterColumn("dbo.Users", "UserName", c => c.String());
            DropColumn("dbo.Users", "ConfirmPwd");
        }
    }
}
