using Quanlynhatro.Models;
using System;

namespace baitaplon.Models
{
    public class HoaDon
    {
        public int Id { get; set; }
        public string ThangNam { get; set; } // Ví dụ: "06/2026"

        public decimal TongTien { get; set; }
        public decimal SoTienDaTra { get; set; }

        // Công nợ = TongTien - SoTienDaTra. Thuộc tính này có thể tính toán trực tiếp trên C#
        public decimal CongNo => TongTien - SoTienDaTra;

        public DateTime? NgayThanhToan { get; set; }
        public string TrangThai { get; set; } // "ChuaThanhToan", "DaThanhToan", "NoXau"

        public int HopDongId { get; set; }
        public virtual HopDong HopDong { get; set; }
    }
}