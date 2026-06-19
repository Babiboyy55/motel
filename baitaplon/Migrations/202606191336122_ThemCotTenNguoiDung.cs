namespace baitaplon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThemCotTenNguoiDung : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NguoiDungs", "TenNguoiDung", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.NguoiDungs", "TenNguoiDung");
        }
    }
}
