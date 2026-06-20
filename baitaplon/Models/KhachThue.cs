using System;
using System.Collections.ObjectModel;

namespace Quanlynhatro.Models
{
    /// <summary>
    /// KhachThue - Model đại diện cho một khách thuê (mở rộng đầy đủ)
    /// </summary>
    public class KhachThue
    {
        public int KhachID { get; set; }
        public string HoTen { get; set; }               // Họ và tên đầy đủ
        public string CCCD { get; set; }                // Số CMND/CCCD
        public string SoDienThoai { get; set; }         // Số điện thoại
        public string Email { get; set; }               // Email
        public DateTime NgaySinh { get; set; }          // Ngày sinh
        public string GioiTinh { get; set; }            // Nam / Nữ
        public string QueQuan { get; set; }             // Quê quán / Nơi thường trú
        public string DiaChiHienTai { get; set; }       // Địa chỉ hiện tại
        public string AnhChanDung { get; set; }         // Đường dẫn ảnh chân dung
        public string AnhCCCD { get; set; }             // Đường dẫn ảnh CCCD

        // Người liên hệ khẩn cấp
        public string NguoiLienHeKhan { get; set; }     // Tên người liên hệ khẩn cấp
        public string QuanHeNguoiLienHe { get; set; }   // Quan hệ (Bố, Mẹ, Anh...)
        public string SoDTNguoiLienHe { get; set; }     // SĐT người liên hệ khẩn cấp

        // Thông tin thuê phòng
        public int PhongID { get; set; }                // Phòng đang ở (0 nếu chưa ở)
        public DateTime NgayVaoO { get; set; }          // Ngày bắt đầu vào ở
        public DateTime? NgayRaO { get; set; }          // Ngày ra (null nếu còn ở)
        public bool DangO { get; set; }                 // Đang ở hay không

        // Ghi chú
        public string GhiChu { get; set; }

        // Computed
        public string TuoiText
        {
            get
            {
                if (NgaySinh == default) return "--";
                int age = DateTime.Today.Year - NgaySinh.Year;
                if (NgaySinh.Date > DateTime.Today.AddYears(-age)) age--;
                return $"{age} tuổi";
            }
        }

        public string TrangThaiText => DangO ? "Đang ở" : "Đã rời";

        public KhachThue()
        {
            NgaySinh = new DateTime(1995, 1, 1);
            NgayVaoO = DateTime.Today;
            DangO = true;
            GioiTinh = "Nam";
        }
    }
}
