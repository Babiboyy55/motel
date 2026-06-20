using System.Collections.Generic;

namespace baitaplon.Models
{
    public class NguoiDung
    {
        public int Id { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }

        public string TenNguoiDung { get; set; }

        // Vai trò: "Admin", "QuanLy", "ThuNgan"
        public string VaiTro { get; set; }
        public bool TrangThai { get; set; }

        public virtual ICollection<NhatKyHoatDong> NhatKyHoatDongs { get; set; }
    }
}