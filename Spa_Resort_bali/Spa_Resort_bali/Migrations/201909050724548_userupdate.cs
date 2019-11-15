namespace Spa_Resort_bali.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class userupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AdressId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "Firstname", c => c.String());
            AddColumn("dbo.AspNetUsers", "Lastname", c => c.String());
            AddColumn("dbo.AspNetUsers", "DateOfBirth", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "Klantnr", c => c.Int());
            AddColumn("dbo.AspNetUsers", "Phonenr", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Phonenr");
            DropColumn("dbo.AspNetUsers", "Klantnr");
            DropColumn("dbo.AspNetUsers", "DateOfBirth");
            DropColumn("dbo.AspNetUsers", "Lastname");
            DropColumn("dbo.AspNetUsers", "Firstname");
            DropColumn("dbo.AspNetUsers", "AdressId");
        }
    }
}