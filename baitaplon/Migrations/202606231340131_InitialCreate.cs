namespace baitaplon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HoaDons",
                c => new
                    {
                        HoaDonID = c.Int(nullable: false, identity: true),
                        PhongID = c.Int(nullable: false),
                        TenPhong = c.String(),
                        TenKhach = c.String(),
                        Thang = c.Int(nullable: false),
                        Nam = c.Int(nullable: false),
                        TienThue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChiSoDienDau = c.Int(nullable: false),
                        ChiSoDienCuoi = c.Int(nullable: false),
                        GiaDien = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChiSoNuocDau = c.Int(nullable: false),
                        ChiSoNuocCuoi = c.Int(nullable: false),
                        GiaNuoc = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TienInternet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TienRac = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TienXe = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PhiKhac = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GhiChuPhiKhac = c.String(),
                        SoTienPhaiTra = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrangThai = c.String(),
                        SoTienDaTra = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HopDongId = c.Int(),
                        NgayThanhToan = c.DateTime(),
                        HinhThucThanhToan = c.String(),
                        MaGiaoDich = c.String(),
                        NgayLap = c.DateTime(nullable: false),
                        GhiChu = c.String(),
                    })
                .PrimaryKey(t => t.HoaDonID);
            
            CreateTable(
                "dbo.HopDongs",
                c => new
                    {
                        HopDongID = c.Int(nullable: false, identity: true),
                        KhachID = c.Int(nullable: false),
                        PhongID = c.Int(nullable: false),
                        NgayBatDau = c.DateTime(nullable: false),
                        NgayKetThuc = c.DateTime(nullable: false),
                        GiaThue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TienCoc = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DaHoanCoc = c.Boolean(nullable: false),
                        NgayHoanCoc = c.DateTime(),
                        DieuKhoan = c.String(),
                        TrangThai = c.String(),
                        GhiChu = c.String(),
                        TenKhach = c.String(),
                        TenPhong = c.String(),
                    })
                .PrimaryKey(t => t.HopDongID);
            
            CreateTable(
                "dbo.KhachThues",
                c => new
                    {
                        KhachID = c.Int(nullable: false, identity: true),
                        HoTen = c.String(),
                        CCCD = c.String(),
                        SoDienThoai = c.String(),
                        Email = c.String(),
                        NgaySinh = c.DateTime(nullable: false),
                        GioiTinh = c.String(),
                        QueQuan = c.String(),
                        DiaChiHienTai = c.String(),
                        AnhChanDung = c.String(),
                        AnhCCCD = c.String(),
                        NguoiLienHeKhan = c.String(),
                        QuanHeNguoiLienHe = c.String(),
                        SoDTNguoiLienHe = c.String(),
                        PhongID = c.Int(nullable: false),
                        NgayVaoO = c.DateTime(nullable: false),
                        NgayRaO = c.DateTime(),
                        DangO = c.Boolean(nullable: false),
                        GhiChu = c.String(),
                        HopDong_HopDongID = c.Int(),
                    })
                .PrimaryKey(t => t.KhachID)
                .ForeignKey("dbo.HopDongs", t => t.HopDong_HopDongID)
                .ForeignKey("dbo.PhongTroes", t => t.PhongID, cascadeDelete: true)
                .Index(t => t.PhongID)
                .Index(t => t.HopDong_HopDongID);
            
            CreateTable(
                "dbo.NguoiDungs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenDangNhap = c.String(),
                        MatKhau = c.String(),
                        TenNguoiDung = c.String(),
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
            
            CreateTable(
                "dbo.PhongTroes",
                c => new
                    {
                        PhongID = c.Int(nullable: false, identity: true),
                        TenPhong = c.String(),
                        LoaiPhong = c.Int(nullable: false),
                        Tang = c.Int(nullable: false),
                        DienTich = c.Double(nullable: false),
                        GiaThue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GiaDien = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GiaNuoc = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GiaInternet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GiaRac = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GiaXeMay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrangThai = c.Int(nullable: false),
                        TrangThaiNoiThat = c.String(),
                        MoTa = c.String(),
                        HinhAnh = c.String(),
                        CoWifi = c.Boolean(nullable: false),
                        CoMayGiat = c.Boolean(nullable: false),
                        CoBep = c.Boolean(nullable: false),
                        CoGiuXeMay = c.Boolean(nullable: false),
                        CoDieuHoa = c.Boolean(nullable: false),
                        CoNongLanh = c.Boolean(nullable: false),
                        CoTuLanh = c.Boolean(nullable: false),
                        CoTivi = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PhongID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KhachThues", "PhongID", "dbo.PhongTroes");
            DropForeignKey("dbo.NhatKyHoatDongs", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.KhachThues", "HopDong_HopDongID", "dbo.HopDongs");
            DropIndex("dbo.NhatKyHoatDongs", new[] { "NguoiDungId" });
            DropIndex("dbo.KhachThues", new[] { "HopDong_HopDongID" });
            DropIndex("dbo.KhachThues", new[] { "PhongID" });
            DropTable("dbo.PhongTroes");
            DropTable("dbo.NhatKyHoatDongs");
            DropTable("dbo.NguoiDungs");
            DropTable("dbo.KhachThues");
            DropTable("dbo.HopDongs");
            DropTable("dbo.HoaDons");
        }
    }
}
