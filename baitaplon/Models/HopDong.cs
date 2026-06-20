using System;
using System.Collections.ObjectModel;

namespace Quanlynhatro.Models
{
    /// <summary>
    /// HopDong - Model hợp đồng thuê phòng (mở rộng đầy đủ)
    /// </summary>
    public class HopDong
    {
        public int HopDongID { get; set; }
        public int KhachID { get; set; }                // Khách thuê chính
        public int PhongID { get; set; }
        public DateTime NgayBatDau { get; set; }        // Ngày bắt đầu thuê
        public DateTime NgayKetThuc { get; set; }       // Ngày kết thúc hợp đồng
        public decimal GiaThue { get; set; }            // Giá thuê cố định trong HĐ
        public decimal TienCoc { get; set; }            // Tiền đặt cọc
        public bool DaHoanCoc { get; set; }             // Đã hoàn cọc chưa
        public DateTime? NgayHoanCoc { get; set; }      // Ngày hoàn cọc
        public string DieuKhoan { get; set; }           // Điều khoản hợp đồng
        public string TrangThai { get; set; }           // Hoạt động / Hết hạn / Đã chấm dứt
        public string GhiChu { get; set; }

        // Danh sách người ở chung (ghép phòng)
        public ObservableCollection<KhachThue> DanhSachNguoiOChung { get; set; }

        // Navigation
        public string TenKhach { get; set; }           // Tên khách (để hiển thị)
        public string TenPhong { get; set; }           // Tên phòng (để hiển thị)

        // Computed
        public int SoThueConLai
        {
            get
            {
                var diff = NgayKetThuc - DateTime.Today;
                return Math.Max(0, (int)diff.TotalDays);
            }
        }

        public string TrangThaiDisplay
        {
            get
            {
                if (TrangThai == "Hoạt động" && NgayKetThuc < DateTime.Today)
                    return "Sắp hết hạn";
                return TrangThai ?? "Hoạt động";
            }
        }

        public HopDong()
        {
            NgayBatDau = DateTime.Today;
            NgayKetThuc = DateTime.Today.AddMonths(12);
            TrangThai = "Hoạt động";
            DanhSachNguoiOChung = new ObservableCollection<KhachThue>();
        }
    }
}
