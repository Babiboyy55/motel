namespace baitaplon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaoBangAdminVaDoanhThu : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HoaDons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ThangNam = c.String(),
                        TongTien = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SoTienDaTra = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NgayThanhToan = c.DateTime(),
                        TrangThai = c.String(),
                        HopDongId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HopDongs", t => t.HopDongId, cascadeDelete: true)
                .Index(t => t.HopDongId);
            
            CreateTable(
                "dbo.HopDongs",
                c => new
                    {
                        HopDongID = c.Int(nullable: false, identity: true),
                        KhachID = c.Int(nullable: false),
                        PhongID = c.Int(nullable: false),
                        NgayBatDau = c.DateTime(nullable: false),
                        NgayKetThuc = c.DateTime(nullable: false),
                        GiaCoDinh = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrangThai = c.String(),
                        GhiChu = c.String(),
                    })
                .PrimaryKey(t => t.HopDongID);
            
            CreateTable(
                "dbo.NguoiDungs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenDangNhap = c.String(),
                        MatKhau = c.String(),
                        VaiTro = c.String(),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NhatKyHoatDongs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HanhDong = c.String(),
                        ThoiGian = c.DateTime(nullable: false),
                        ChiTiet = c.String(),
                        NguoiDungId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId, cascadeDelete: true)
                .Index(t => t.NguoiDungId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NhatKyHoatDongs", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.HoaDons", "HopDongId", "dbo.HopDongs");
            DropIndex("dbo.NhatKyHoatDongs", new[] { "NguoiDungId" });
            DropIndex("dbo.HoaDons", new[] { "HopDongId" });
            DropTable("dbo.NhatKyHoatDongs");
            DropTable("dbo.NguoiDungs");
            DropTable("dbo.HopDongs");
            DropTable("dbo.HoaDons");
        }
    }
}
