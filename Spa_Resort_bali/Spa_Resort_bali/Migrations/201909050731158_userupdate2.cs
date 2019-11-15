namespace Spa_Resort_bali.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class userupdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ClientNumber", c => c.Int());
            DropColumn("dbo.AspNetUsers", "Klantnr");
            DropColumn("dbo.AspNetUsers", "Phonenr");
        }

        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Phonenr", c => c.String());
            AddColumn("dbo.AspNetUsers", "Klantnr", c => c.Int());
            DropColumn("dbo.AspNetUsers", "ClientNumber");
        }
    }
}