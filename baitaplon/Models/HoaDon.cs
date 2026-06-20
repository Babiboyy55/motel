using System;

namespace Quanlynhatro.Models
{
    /// <summary>
    /// HoaDon - Hóa đơn hàng tháng của phòng
    /// </summary>
    public class HoaDon
    {
        public int HoaDonID { get; set; }
        public int PhongID { get; set; }
        public string TenPhong { get; set; }           // Tên phòng (hiển thị)
        public string TenKhach { get; set; }           // Tên khách chính (hiển thị)
        public int Thang { get; set; }                 // Tháng
        public int Nam { get; set; }                   // Năm
        public string ThangNamText => $"Tháng {Thang:D2}/{Nam}";

        // Tiền thuê
        public decimal TienThue { get; set; }

        // Điện
        public int ChiSoDienDau { get; set; }          // Chỉ số điện đầu kỳ (kWh)
        public int ChiSoDienCuoi { get; set; }         // Chỉ số điện cuối kỳ (kWh)
        public int SoDienTieuThu => ChiSoDienCuoi - ChiSoDienDau;
        public decimal GiaDien { get; set; }            // Giá điện/kWh
        public decimal TienDien => SoDienTieuThu * GiaDien;

        // Nước
        public int ChiSoNuocDau { get; set; }          // Chỉ số nước đầu kỳ (m³)
        public int ChiSoNuocCuoi { get; set; }         // Chỉ số nước cuối kỳ (m³)
        public int SoNuocTieuThu => ChiSoNuocCuoi - ChiSoNuocDau;
        public decimal GiaNuoc { get; set; }            // Giá nước/m³
        public decimal TienNuoc => SoNuocTieuThu * GiaNuoc;

        // Phí dịch vụ
        public decimal TienInternet { get; set; }       // Phí internet
        public decimal TienRac { get; set; }            // Phí rác
        public decimal TienXe { get; set; }             // Phí gửi xe
        public decimal PhiKhac { get; set; }            // Phí khác
        public string GhiChuPhiKhac { get; set; }

        // Tổng hợp
        public decimal TongTien => TienThue + TienDien + TienNuoc + TienInternet + TienRac + TienXe + PhiKhac;
        public decimal SoTienPhaiTra { get; set; }     // Có thể điều chỉnh

        // Thanh toán
        public string TrangThai { get; set; }           // "Chưa thanh toán" / "Đã thanh toán" / "Trả một phần"
        public decimal SoTienDaTra { get; set; }        // Số tiền đã thanh toán
        public decimal ConNo => TongTien - SoTienDaTra; // Còn nợ
        public DateTime? NgayThanhToan { get; set; }    // Ngày thanh toán
        public string HinhThucThanhToan { get; set; }   // Tiền mặt / Chuyển khoản
        public string MaGiaoDich { get; set; }          // Mã giao dịch (chuyển khoản)

        // Meta
        public DateTime NgayLap { get; set; }           // Ngày lập hóa đơn
        public string GhiChu { get; set; }

        // Computed display
        public string TrangThaiColor => TrangThai switch
        {
            "Đã thanh toán" => "#27ae60",
            "Trả một phần" => "#f39c12",
            _ => "#e74c3c"
        };

        public HoaDon()
        {
            NgayLap = DateTime.Today;
            TrangThai = "Chưa thanh toán";
            Thang = DateTime.Today.Month;
            Nam = DateTime.Today.Year;
        }
    }
}
