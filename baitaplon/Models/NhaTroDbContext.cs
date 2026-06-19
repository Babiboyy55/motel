using Quanlynhatro.Models;
using System.Data.Entity;

namespace baitaplon.Models
{
    // Bắt buộc phải kế thừa từ DbContext của Entity Framework
    public class NhaTroDbContext : DbContext
    {
        // Chỉ định EF đọc chuỗi kết nối "DefaultConnection" từ file App.config
        public NhaTroDbContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<NhatKyHoatDong> NhatKyHoatDongs { get; set; }
        public DbSet<HopDong> HopDongs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<PhongTro> PhongTros { get; set; }
        public DbSet<KhachThue> KhachThues { get; set; }
    }
}