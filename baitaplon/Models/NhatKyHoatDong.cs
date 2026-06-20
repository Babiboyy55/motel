using System;

namespace baitaplon.Models
{
    public class NhatKyHoatDong
    {
        public int Id { get; set; }
        public string HanhDong { get; set; } // Ví dụ: "Thêm hợp đồng", "Xóa phòng"
        public DateTime ThoiGian { get; set; }
        public string ChiTiet { get; set; }

        // Khóa ngoại liên kết với Người dùng
        public int NguoiDungId { get; set; }
        public virtual NguoiDung NguoiDung { get; set; }
    }
}