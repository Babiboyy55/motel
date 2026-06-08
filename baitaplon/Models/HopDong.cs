using System;
using System;

namespace Quanlynhatro.Models
{
    /// <summary>
    /// HopDong - Model đại diện cho một hợp đồng thuê
    /// </summary>
    public class HopDong
    {
        public int HopDongID { get; set; }
        public int KhachID { get; set; }
        public int PhongID { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal GiaCoDinh { get; set; }
        public string TrangThai { get; set; } // "Hoạt động" hoặc "Hết hạn"
        public string GhiChu { get; set; }
    }
}

