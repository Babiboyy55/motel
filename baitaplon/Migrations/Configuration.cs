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
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(baitaplon.Models.NhaTroDbContext context)
        {
            // 1. DỮ LIỆU TEST: PHÂN HỆ 3.6 (NGƯỜI DÙNG & NHẬT KÝ)
            // - Bỏ HoTen, bổ sung MatKhau và TrangThai
            context.NguoiDungs.AddOrUpdate(
                n => n.TenDangNhap,
                new NguoiDung { TenDangNhap = "admin", TenNguoiDung = "Hoàng Phong", MatKhau = "123456", VaiTro = "Admin", TrangThai = true },
                new NguoiDung { TenDangNhap = "thungan1", TenNguoiDung = "Nguyễn Thị Thu", MatKhau = "123456", VaiTro = "ThuNgan", TrangThai = true }
             );

            // Lưu Database ở bước này trước để hệ thống tự cấp Id cho 2 người dùng vừa tạo
            context.SaveChanges();

            // Lấy Id của Admin và Thu Ngân để gắn vào Nhật Ký Hoạt Động
            var admin = context.NguoiDungs.FirstOrDefault(n => n.TenDangNhap == "admin");
            var thuNgan = context.NguoiDungs.FirstOrDefault(n => n.TenDangNhap == "thungan1");

            if (admin != null && thuNgan != null)
            {
                context.NhatKyHoatDongs.AddOrUpdate(
                    n => n.HanhDong,
                    // Sử dụng NguoiDungId thay vì gán tên trực tiếp
                    new NhatKyHoatDong { NguoiDungId = admin.Id, HanhDong = "Cập nhật hệ thống PCCC", ThoiGian = DateTime.Now.AddHours(-2), ChiTiet = "Khu A" },
                    new NhatKyHoatDong { NguoiDungId = thuNgan.Id, HanhDong = "Xuất báo cáo tháng trước", ThoiGian = DateTime.Now.AddMinutes(-30), ChiTiet = "File Excel" }
                );
            }

            // 2. DỮ LIỆU TEST: PHÂN HỆ 3.5 (HỢP ĐỒNG & CẢNH BÁO)
            /*
            context.HopDongs.AddOrUpdate(
                h => h.HopDongID, 
                new HopDong { HopDongID = 1, KhachID = 1, PhongID = 101, NgayBatDau = DateTime.Now.AddMonths(-5), NgayKetThuc = DateTime.Now.AddMonths(6), GiaThue = 3000000, TrangThai = "Hoạt động" },
                new HopDong { HopDongID = 2, KhachID = 2, PhongID = 102, NgayBatDau = DateTime.Now.AddMonths(-11), NgayKetThuc = DateTime.Now.AddDays(15), GiaThue = 2500000, TrangThai = "Hoạt động" }
            );
            */

            // 3. DỮ LIỆU TEST: PHÂN HỆ 3.4 (DASHBOARD)
            /*
            string thangNay = $"{DateTime.Now.Month:D2}/{DateTime.Now.Year}";

            context.HoaDons.AddOrUpdate(
                h => h.HoaDonID,
                new HoaDon { HoaDonID = 1, PhongID = 1, Thang = DateTime.Now.Month, Nam = DateTime.Now.Year, TienThue = 3500000, SoTienDaTra = 3500000, TrangThai = "Đã thanh toán" },
                new HoaDon { HoaDonID = 2, PhongID = 2, Thang = DateTime.Now.Month, Nam = DateTime.Now.Year, TienThue = 2800000, SoTienDaTra = 0, TrangThai = "Chưa thanh toán" }
            );
            */

            // 4. CHỐT LƯU TOÀN BỘ XUỐNG DATABASE
            context.SaveChanges();
        }
    }
}
