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
            // 1. DỮ LIỆU TEST: NGƯỜI DÙNG & NHẬT KÝ
            context.NguoiDungs.AddOrUpdate(
                n => n.TenDangNhap,
                new NguoiDung { TenDangNhap = "admin", TenNguoiDung = "Hoàng Phong", MatKhau = SecurityHelper.HashPassword("123456"), VaiTro = "Admin", TrangThai = true, SoDienThoai = "0901234567" },
                new NguoiDung { TenDangNhap = "thungan1", TenNguoiDung = "Nguyễn Thị Thu", MatKhau = SecurityHelper.HashPassword("123456"), VaiTro = "ThuNgan", TrangThai = true, SoDienThoai = "0912345678" },
                new NguoiDung { TenDangNhap = "thungan2", TenNguoiDung = "Phạm Hoàng Long", MatKhau = SecurityHelper.HashPassword("123456"), VaiTro = "ThuNgan", TrangThai = true, SoDienThoai = "0987654321" },
                new NguoiDung { TenDangNhap = "quanly1", TenNguoiDung = "Lê Thị Mai", MatKhau = SecurityHelper.HashPassword("123456"), VaiTro = "QuanLy", TrangThai = true, SoDienThoai = "0934567890" },
                new NguoiDung { TenDangNhap = "quanly2", TenNguoiDung = "Trần Minh Quân", MatKhau = SecurityHelper.HashPassword("123456"), VaiTro = "QuanLy", TrangThai = false, SoDienThoai = "0905556667" }
            );

            context.SaveChanges();

            var admin = context.NguoiDungs.FirstOrDefault(n => n.TenDangNhap == "admin");
            var thuNgan1 = context.NguoiDungs.FirstOrDefault(n => n.TenDangNhap == "thungan1");
            var quanLy = context.NguoiDungs.FirstOrDefault(n => n.TenDangNhap == "quanly1");

            if (admin != null && thuNgan1 != null && !context.NhatKyHoatDongs.Any())
            {
                context.NhatKyHoatDongs.AddRange(new[] {
                    new NhatKyHoatDong { NguoiDungId = admin.Id, HanhDong = "Đăng nhập", ThoiGian = DateTime.Now.AddDays(-3), ChiTiet = "Đăng nhập thành công với vai trò Admin" },
                    new NhatKyHoatDong { NguoiDungId = admin.Id, HanhDong = "Cập nhật hệ thống PCCC", ThoiGian = DateTime.Now.AddHours(-2), ChiTiet = "Khu A" },
                    new NhatKyHoatDong { NguoiDungId = thuNgan1.Id, HanhDong = "Xuất báo cáo tháng trước", ThoiGian = DateTime.Now.AddMinutes(-30), ChiTiet = "File Excel" },
                    new NhatKyHoatDong { NguoiDungId = thuNgan1.Id, HanhDong = "Đăng nhập", ThoiGian = DateTime.Now.AddHours(-1), ChiTiet = "Đăng nhập thành công với vai trò ThuNgan" },
                    new NhatKyHoatDong { NguoiDungId = quanLy != null ? quanLy.Id : admin.Id, HanhDong = "Cập nhật thông tin phòng", ThoiGian = DateTime.Now.AddDays(-1), ChiTiet = "Cập nhật giá thuê phòng P202" },
                    new NhatKyHoatDong { NguoiDungId = admin.Id, HanhDong = "Khóa tài khoản", ThoiGian = DateTime.Now.AddDays(-2), ChiTiet = "Khóa tài khoản quanly2 do vi phạm" }
                });
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
                },
                new PhongTro
                {
                    PhongID = 4, TenPhong = "P202", LoaiPhong = LoaiPhong.PhongDoi,
                    Tang = 2, DienTich = 30, GiaThue = 4000000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 200000,
                    TrangThai = TrangThaiPhong.DangThue, TrangThaiNoiThat = "Đầy đủ",
                    CoWifi = true, CoDieuHoa = true, CoNongLanh = true, CoTuLanh = true, CoMayGiat = true,
                    MoTa = "Phòng VIP ban công rộng, đầy đủ đồ gia dụng"
                },
                new PhongTro
                {
                    PhongID = 5, TenPhong = "P203", LoaiPhong = LoaiPhong.PhongDon,
                    Tang = 2, DienTich = 18, GiaThue = 2600000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 100000,
                    TrangThai = TrangThaiPhong.DangThue, TrangThaiNoiThat = "Cơ bản",
                    CoWifi = true, CoDieuHoa = true, CoNongLanh = true,
                    MoTa = "Phòng đơn cơ bản thoáng sạch"
                },
                new PhongTro
                {
                    PhongID = 6, TenPhong = "P301", LoaiPhong = LoaiPhong.PhongDon,
                    Tang = 3, DienTich = 18, GiaThue = 2400000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 100000,
                    TrangThai = TrangThaiPhong.DangSuaChua, TrangThaiNoiThat = "Không có",
                    CoWifi = false, CoDieuHoa = false, CoNongLanh = false,
                    MoTa = "Phòng đang sơn lại và sửa điện nước"
                },
                new PhongTro
                {
                    PhongID = 7, TenPhong = "P302", LoaiPhong = LoaiPhong.PhongDoi,
                    Tang = 3, DienTich = 28, GiaThue = 3600000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 200000,
                    TrangThai = TrangThaiPhong.DangThue, TrangThaiNoiThat = "Đầy đủ",
                    CoWifi = true, CoDieuHoa = true, CoNongLanh = true,
                    MoTa = "Phòng đôi rộng rãi tầng cao yên tĩnh"
                },
                new PhongTro
                {
                    PhongID = 8, TenPhong = "P303", LoaiPhong = LoaiPhong.PhongDon,
                    Tang = 3, DienTich = 20, GiaThue = 2700000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 100000,
                    TrangThai = TrangThaiPhong.Trong, TrangThaiNoiThat = "Cơ bản",
                    CoWifi = true, CoDieuHoa = false, CoNongLanh = true,
                    MoTa = "Phòng tầng 3 trống sẵn ở ngay"
                },
                new PhongTro
                {
                    PhongID = 9, TenPhong = "P401", LoaiPhong = LoaiPhong.PhongDoi,
                    Tang = 4, DienTich = 32, GiaThue = 4200000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 200000,
                    TrangThai = TrangThaiPhong.Trong, TrangThaiNoiThat = "Đầy đủ",
                    CoWifi = true, CoDieuHoa = true, CoNongLanh = true, CoTuLanh = true,
                    MoTa = "Căn hộ Penthouse mini tầng thượng"
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
                },
                new KhachThue
                {
                    KhachID = 3, HoTen = "Lê Văn Cường", CCCD = "011223344556",
                    SoDienThoai = "0988776655", Email = "cuong.le@gmail.com",
                    NgaySinh = new DateTime(1996, 12, 10), GioiTinh = "Nam",
                    QueQuan = "Thanh Hóa", DiaChiHienTai = "789 Đường GHI, Q.Bình Thạnh, TP.HCM",
                    PhongID = 4, NgayVaoO = new DateTime(2025, 2, 15), DangO = true
                },
                new KhachThue
                {
                    KhachID = 4, HoTen = "Phạm Thị Dung", CCCD = "022334455667",
                    SoDienThoai = "0966554433", Email = "dung.pham@gmail.com",
                    NgaySinh = new DateTime(1999, 3, 4), GioiTinh = "Nữ",
                    QueQuan = "Nam Định", DiaChiHienTai = "Khu phố 3, Q.Thủ Đức, TP.HCM",
                    PhongID = 5, NgayVaoO = new DateTime(2025, 5, 1), DangO = true
                },
                new KhachThue
                {
                    KhachID = 5, HoTen = "Hoàng Văn Em", CCCD = "033445566778",
                    SoDienThoai = "0977665544", Email = "em.hoang@gmail.com",
                    NgaySinh = new DateTime(1997, 7, 25), GioiTinh = "Nam",
                    QueQuan = "Nghệ An", DiaChiHienTai = "Phường 5, Q.Gò Vấp, TP.HCM",
                    PhongID = 7, NgayVaoO = new DateTime(2025, 6, 1), DangO = true
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
                },
                new HopDong
                {
                    HopDongID = 3, KhachID = 3, PhongID = 4, TenKhach = "Lê Văn Cường", TenPhong = "P202",
                    NgayBatDau = new DateTime(2025, 2, 15), NgayKetThuc = new DateTime(2026, 2, 15),
                    GiaThue = 4000000, TienCoc = 4000000,
                    TrangThai = "Hoạt động"
                },
                new HopDong
                {
                    HopDongID = 4, KhachID = 4, PhongID = 5, TenKhach = "Phạm Thị Dung", TenPhong = "P203",
                    NgayBatDau = new DateTime(2025, 5, 1), NgayKetThuc = new DateTime(2026, 5, 1),
                    GiaThue = 2600000, TienCoc = 2600000,
                    TrangThai = "Hoạt động"
                },
                new HopDong
                {
                    HopDongID = 5, KhachID = 5, PhongID = 7, TenKhach = "Hoàng Văn Em", TenPhong = "P302",
                    NgayBatDau = new DateTime(2025, 6, 1), NgayKetThuc = new DateTime(2026, 6, 1),
                    GiaThue = 3600000, TienCoc = 3600000,
                    TrangThai = "Hoạt động"
                }
            );

            // 5. DỮ LIỆU TEST: HÓA ĐƠN
            int thisMonth = DateTime.Now.Month;
            int thisYear = DateTime.Now.Year;
            int prevMonth = thisMonth == 1 ? 12 : thisMonth - 1;
            int prevYear = thisMonth == 1 ? thisYear - 1 : thisYear;

            context.HoaDons.AddOrUpdate(
                h => h.HoaDonID,
                // Hóa đơn tháng trước - Phòng 1 (Đã thanh toán)
                new HoaDon
                {
                    HoaDonID = 1, PhongID = 1, TenPhong = "P101", TenKhach = "Nguyễn Văn An",
                    Thang = prevMonth, Nam = prevYear, TienThue = 2500000,
                    ChiSoDienDau = 1050, ChiSoDienCuoi = 1200, GiaDien = 3500,
                    ChiSoNuocDau = 38, ChiSoNuocCuoi = 45, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 100000,
                    TrangThai = "Đã thanh toán", SoTienDaTra = 2500000 + 150 * 3500 + 7 * 15000 + 100000 + 20000 + 100000,
                    NgayThanhToan = new DateTime(prevYear, prevMonth, 10), HinhThucThanhToan = "Chuyển khoản",
                    NgayLap = new DateTime(prevYear, prevMonth, 5), HopDongId = 1
                },
                // Hóa đơn tháng này - Phòng 1 (Chưa thanh toán)
                new HoaDon
                {
                    HoaDonID = 2, PhongID = 1, TenPhong = "P101", TenKhach = "Nguyễn Văn An",
                    Thang = thisMonth, Nam = thisYear, TienThue = 2500000,
                    ChiSoDienDau = 1200, ChiSoDienCuoi = 1310, GiaDien = 3500,
                    ChiSoNuocDau = 45, ChiSoNuocCuoi = 52, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 100000,
                    TrangThai = "Chưa thanh toán", SoTienDaTra = 0,
                    NgayLap = DateTime.Now.AddDays(-5), HopDongId = 1
                },
                // Hóa đơn tháng trước - Phòng 2 (Đã thanh toán)
                new HoaDon
                {
                    HoaDonID = 3, PhongID = 2, TenPhong = "P102", TenKhach = "Trần Thị Bình",
                    Thang = prevMonth, Nam = prevYear, TienThue = 3500000,
                    ChiSoDienDau = 780, ChiSoDienCuoi = 890, GiaDien = 3500,
                    ChiSoNuocDau = 24, ChiSoNuocCuoi = 30, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 200000,
                    TrangThai = "Đã thanh toán", SoTienDaTra = 3500000 + 110 * 3500 + 6 * 15000 + 100000 + 20000 + 200000,
                    NgayThanhToan = new DateTime(prevYear, prevMonth, 8), HinhThucThanhToan = "Chuyển khoản",
                    NgayLap = new DateTime(prevYear, prevMonth, 5), HopDongId = 2
                },
                // Hóa đơn tháng này - Phòng 2 (Trả một phần)
                new HoaDon
                {
                    HoaDonID = 4, PhongID = 2, TenPhong = "P102", TenKhach = "Trần Thị Bình",
                    Thang = thisMonth, Nam = thisYear, TienThue = 3500000,
                    ChiSoDienDau = 890, ChiSoDienCuoi = 1020, GiaDien = 3500,
                    ChiSoNuocDau = 30, ChiSoNuocCuoi = 38, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 200000,
                    TrangThai = "Trả một phần", SoTienDaTra = 2500000,
                    NgayThanhToan = DateTime.Now.AddDays(-1), HinhThucThanhToan = "Tiền mặt",
                    NgayLap = DateTime.Now.AddDays(-5), HopDongId = 2
                },
                // Hóa đơn tháng này - Phòng 4 (Đã thanh toán)
                new HoaDon
                {
                    HoaDonID = 5, PhongID = 4, TenPhong = "P202", TenKhach = "Lê Văn Cường",
                    Thang = thisMonth, Nam = thisYear, TienThue = 4000000,
                    ChiSoDienDau = 350, ChiSoDienCuoi = 490, GiaDien = 3500,
                    ChiSoNuocDau = 12, ChiSoNuocCuoi = 20, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 200000,
                    TrangThai = "Đã thanh toán", SoTienDaTra = 4000000 + 140 * 3500 + 8 * 15000 + 100000 + 20000 + 200000,
                    NgayThanhToan = DateTime.Now.AddDays(-3), HinhThucThanhToan = "Chuyển khoản",
                    NgayLap = DateTime.Now.AddDays(-5), HopDongId = 3
                },
                // Hóa đơn tháng này - Phòng 5 (Chưa thanh toán)
                new HoaDon
                {
                    HoaDonID = 6, PhongID = 5, TenPhong = "P203", TenKhach = "Phạm Thị Dung",
                    Thang = thisMonth, Nam = thisYear, TienThue = 2600000,
                    ChiSoDienDau = 150, ChiSoDienCuoi = 220, GiaDien = 3500,
                    ChiSoNuocDau = 5, ChiSoNuocCuoi = 9, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 100000,
                    TrangThai = "Chưa thanh toán", SoTienDaTra = 0,
                    NgayLap = DateTime.Now.AddDays(-5), HopDongId = 4
                },
                // Hóa đơn tháng này - Phòng 7 (Đã thanh toán)
                new HoaDon
                {
                    HoaDonID = 7, PhongID = 7, TenPhong = "P302", TenKhach = "Hoàng Văn Em",
                    Thang = thisMonth, Nam = thisYear, TienThue = 3600000,
                    ChiSoDienDau = 120, ChiSoDienCuoi = 260, GiaDien = 3500,
                    ChiSoNuocDau = 8, ChiSoNuocCuoi = 15, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 200000,
                    TrangThai = "Đã thanh toán", SoTienDaTra = 3600000 + 140 * 3500 + 7 * 15000 + 100000 + 20000 + 200000,
                    NgayThanhToan = DateTime.Now.AddDays(-2), HinhThucThanhToan = "Chuyển khoản",
                    NgayLap = DateTime.Now.AddDays(-5), HopDongId = 5
                }
            );

            context.SaveChanges();
        }
    }
}
