namespace baitaplon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CapNhatKhoaChinhPhongVaKhach : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KhachThues",
                c => new
                    {
                        KhachID = c.Int(nullable: false, identity: true),
                        HoTen = c.String(),
                        SoDienThoai = c.String(),
                        Email = c.String(),
                        NgayVaoO = c.DateTime(nullable: false),
                        CCCD = c.String(),
                        DiaChi = c.String(),
                        PhongID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.KhachID)
                .ForeignKey("dbo.PhongTroes", t => t.PhongID, cascadeDelete: true)
                .Index(t => t.PhongID);
            
            CreateTable(
                "dbo.PhongTroes",
                c => new
                    {
                        PhongID = c.Int(nullable: false, identity: true),
                        TenPhong = c.String(),
                        Dia = c.String(),
                        GiaThue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DienTich = c.Int(nullable: false),
                        TrangThai = c.String(),
                        MoTa = c.String(),
                    })
                .PrimaryKey(t => t.PhongID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KhachThues", "PhongID", "dbo.PhongTroes");
            DropIndex("dbo.KhachThues", new[] { "PhongID" });
            DropTable("dbo.PhongTroes");
            DropTable("dbo.KhachThues");
        }
    }
}
