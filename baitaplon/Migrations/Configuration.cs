using baitaplon.Services;

namespace baitaplon.Migrations
{
    using baitaplon.Models;
    using Quanlynhatro.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<baitaplon.Models.NhaTroDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(baitaplon.Models.NhaTroDbContext context)
        {
            // 1. DỮ LIỆU TEST: PHÂN HỆ 3.6 (NGƯỜI DÙNG & NHẬT KÝ)
            context.NguoiDungs.AddOrUpdate(
                n => n.TenDangNhap,
                new NguoiDung { TenDangNhap = "admin", TenNguoiDung = "Hoàng Phong", MatKhau = SecurityHelper.HashPassword("123456"), VaiTro = "Admin", TrangThai = true, SoDienThoai = "0901234567" },
                new NguoiDung { TenDangNhap = "thungan1", TenNguoiDung = "Nguyễn Thị Thu", MatKhau = SecurityHelper.HashPassword("123456"), VaiTro = "ThuNgan", TrangThai = true, SoDienThoai = "0912345678" }
            );

            context.SaveChanges();

            var admin = context.NguoiDungs.FirstOrDefault(n => n.TenDangNhap == "admin");
            var thuNgan = context.NguoiDungs.FirstOrDefault(n => n.TenDangNhap == "thungan1");

            if (admin != null && thuNgan != null)
            {
                context.NhatKyHoatDongs.AddOrUpdate(
                    n => n.HanhDong,
                    new NhatKyHoatDong { NguoiDungId = admin.Id, HanhDong = "Cập nhật hệ thống PCCC", ThoiGian = DateTime.Now.AddHours(-2), ChiTiet = "Khu A" },
                    new NhatKyHoatDong { NguoiDungId = thuNgan.Id, HanhDong = "Xuất báo cáo tháng trước", ThoiGian = DateTime.Now.AddMinutes(-30), ChiTiet = "File Excel" }
                );
            }

            // 2. DỮ LIỆU TEST: PHÒNG TRỌ
            context.PhongTros.AddOrUpdate(
                p => p.PhongID,
                new PhongTro
                {
                    PhongID = 1, TenPhong = "P101", LoaiPhong = LoaiPhong.PhongDon,
                    Tang = 1, DienTich = 18, GiaThue = 2500000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 100000,
                    TrangThai = TrangThaiPhong.DangThue, TrangThaiNoiThat = "Đầy đủ",
                    CoWifi = true, CoDieuHoa = true, CoNongLanh = true,
                    MoTa = "Phòng đơn tầng 1, view sân, nội thất đầy đủ"
                },
                new PhongTro
                {
                    PhongID = 2, TenPhong = "P102", LoaiPhong = LoaiPhong.PhongDoi,
                    Tang = 1, DienTich = 25, GiaThue = 3500000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 200000,
                    TrangThai = TrangThaiPhong.DangThue, TrangThaiNoiThat = "Đầy đủ",
                    CoWifi = true, CoDieuHoa = true, CoNongLanh = true, CoTuLanh = true, CoTivi = true,
                    MoTa = "Phòng đôi rộng rãi, nội thất cao cấp"
                },
                new PhongTro
                {
                    PhongID = 3, TenPhong = "P201", LoaiPhong = LoaiPhong.PhongDon,
                    Tang = 2, DienTich = 20, GiaThue = 2800000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 100000,
                    TrangThai = TrangThaiPhong.Trong, TrangThaiNoiThat = "Cơ bản",
                    CoWifi = true, CoNongLanh = true, CoDieuHoa = false,
                    MoTa = "Phòng đơn tầng 2, thoáng mát"
                }
            );

            // 3. DỮ LIỆU TEST: KHÁCH THUÊ
            context.KhachThues.AddOrUpdate(
                k => k.KhachID,
                new KhachThue
                {
                    KhachID = 1, HoTen = "Nguyễn Văn An", CCCD = "012345678901",
                    SoDienThoai = "0901234567", Email = "an.nguyen@gmail.com",
                    NgaySinh = new DateTime(1998, 5, 15), GioiTinh = "Nam",
                    QueQuan = "Hà Nội", DiaChiHienTai = "123 Đường ABC, Q.1, TP.HCM",
                    PhongID = 1, NgayVaoO = new DateTime(2025, 1, 1), DangO = true
                },
                new KhachThue
                {
                    KhachID = 2, HoTen = "Trần Thị Bình", CCCD = "098765432100",
                    SoDienThoai = "0912345678", Email = "binh.tran@gmail.com",
                    NgaySinh = new DateTime(2000, 8, 20), GioiTinh = "Nữ",
                    QueQuan = "Đà Nẵng", DiaChiHienTai = "456 Đường DEF, Q.3, TP.HCM",
                    PhongID = 2, NgayVaoO = new DateTime(2025, 3, 1), DangO = true
                }
            );

            context.SaveChanges();

            // 4. DỮ LIỆU TEST: HỢP ĐỒNG
            context.HopDongs.AddOrUpdate(
                h => h.HopDongID,
                new HopDong
                {
                    HopDongID = 1, KhachID = 1, PhongID = 1, TenKhach = "Nguyễn Văn An", TenPhong = "P101",
                    NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2026, 12, 1),
                    GiaThue = 2500000, TienCoc = 2500000,
                    TrangThai = "Hoạt động"
                },
                new HopDong
                {
                    HopDongID = 2, KhachID = 2, PhongID = 2, TenKhach = "Trần Thị Bình", TenPhong = "P102",
                    NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2026, 12, 1),
                    GiaThue = 3500000, TienCoc = 3500000,
                    TrangThai = "Hoạt động"
                }
            );

            // 5. DỮ LIỆU TEST: HÓA ĐƠN
            context.HoaDons.AddOrUpdate(
                h => h.HoaDonID,
                new HoaDon
                {
                    HoaDonID = 1, PhongID = 1, TenPhong = "P101", TenKhach = "Nguyễn Văn An",
                    Thang = DateTime.Now.Month, Nam = DateTime.Now.Year, TienThue = 2500000,
                    ChiSoDienDau = 1200, ChiSoDienCuoi = 1285, GiaDien = 3500,
                    ChiSoNuocDau = 45, ChiSoNuocCuoi = 51, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 100000,
                    TrangThai = "Đã thanh toán", SoTienDaTra = 2500000 + 85*3500 + 6*15000 + 100000 + 20000 + 100000,
                    NgayThanhToan = DateTime.Now.AddDays(-2), HinhThucThanhToan = "Chuyển khoản",
                    NgayLap = DateTime.Now.AddDays(-5), HopDongId = 1
                },
                new HoaDon
                {
                    HoaDonID = 2, PhongID = 2, TenPhong = "P102", TenKhach = "Trần Thị Bình",
                    Thang = DateTime.Now.Month, Nam = DateTime.Now.Year, TienThue = 3500000,
                    ChiSoDienDau = 890, ChiSoDienCuoi = 1002, GiaDien = 3500,
                    ChiSoNuocDau = 30, ChiSoNuocCuoi = 38, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 200000,
                    TrangThai = "Chưa thanh toán", SoTienDaTra = 0,
                    NgayLap = DateTime.Now.AddDays(-5), HopDongId = 2
                }
            );

            context.SaveChanges();
        }
    }
}
