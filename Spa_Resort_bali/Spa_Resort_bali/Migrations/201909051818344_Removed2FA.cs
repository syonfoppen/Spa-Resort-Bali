namespace Spa_Resort_bali.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Removed2FA : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "IsGoogleAuthenticatorEnabled");
            DropColumn("dbo.AspNetUsers", "GoogleAuthenticatorSecretKey");
        }

        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "GoogleAuthenticatorSecretKey", c => c.String());
            AddColumn("dbo.AspNetUsers", "IsGoogleAuthenticatorEnabled", c => c.Boolean(nullable: false));
        }
    }
}