namespace Spa_Resort_bali.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class test : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "ClientNumber");
        }

        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "ClientNumber", c => c.Int());
        }
    }
}