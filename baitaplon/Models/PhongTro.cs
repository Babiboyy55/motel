using System;
using System.Collections.ObjectModel;

namespace Quanlynhatro.Models
{
    /// <summary>
    /// Trạng thái phòng trọ
    /// </summary>
    public enum TrangThaiPhong
    {
        Trong,          // Trống - sẵn sàng cho thuê
        DangThue,       // Đang thuê - có khách ở
        DangSuaChua,    // Đang sửa chữa
        DangDon         // Đang dọn dẹp
    }

    /// <summary>
    /// Loại phòng
    /// </summary>
    public enum LoaiPhong
    {
        PhongDon,       // Phòng đơn
        PhongDoi,       // Phòng đôi (giường đôi)
        PhongGhep,      // Phòng ghép nhiều người
        CanHo           // Căn hộ mini
    }

    /// <summary>
    /// PhongTro - Model đại diện cho một phòng trọ (mở rộng đầy đủ)
    /// </summary>
    public class PhongTro
    {
        public int PhongID { get; set; }
        public string TenPhong { get; set; }          // Số/tên phòng (P101, P202...)
        public LoaiPhong LoaiPhong { get; set; }       // Loại phòng
        public int Tang { get; set; }                   // Tầng
        public double DienTich { get; set; }            // Diện tích (m²)
        public decimal GiaThue { get; set; }            // Giá thuê/tháng
        public decimal GiaDien { get; set; }            // Giá điện/kWh (đồng)
        public decimal GiaNuoc { get; set; }            // Giá nước/m³ (đồng)
        public decimal GiaInternet { get; set; }        // Phí internet/tháng
        public decimal GiaRac { get; set; }             // Phí rác/tháng
        public decimal GiaXeMay { get; set; }           // Phí gửi xe máy/tháng
        public TrangThaiPhong TrangThai { get; set; }   // Trạng thái phòng
        public string TrangThaiNoiThat { get; set; }    // Nội thất: Đầy đủ / Cơ bản / Không có
        public string MoTa { get; set; }                // Mô tả phòng
        public string HinhAnh { get; set; }             // Đường dẫn ảnh phòng

        // Tiện ích kèm theo
        public bool CoWifi { get; set; }
        public bool CoMayGiat { get; set; }
        public bool CoBep { get; set; }
        public bool CoGiuXeMay { get; set; }
        public bool CoDieuHoa { get; set; }
        public bool CoNongLanh { get; set; }
        public bool CoTuLanh { get; set; }
        public bool CoTivi { get; set; }

        // Navigation properties
        public ObservableCollection<KhachThue> DanhSachKhach { get; set; }

        // Computed properties
        public string TrangThaiText => TrangThai switch
        {
            TrangThaiPhong.Trong => "Trống",
            TrangThaiPhong.DangThue => "Đang thuê",
            TrangThaiPhong.DangSuaChua => "Đang sửa chữa",
            TrangThaiPhong.DangDon => "Đang dọn",
            _ => "Không xác định"
        };

        public string LoaiPhongText => LoaiPhong switch
        {
            LoaiPhong.PhongDon => "Phòng đơn",
            LoaiPhong.PhongDoi => "Phòng đôi",
            LoaiPhong.PhongGhep => "Phòng ghép",
            LoaiPhong.CanHo => "Căn hộ mini",
            _ => "Khác"
        };

        public string TienIchText
        {
            get
            {
                var tienIch = new System.Collections.Generic.List<string>();
                if (CoWifi) tienIch.Add("WiFi");
                if (CoMayGiat) tienIch.Add("Máy giặt");
                if (CoBep) tienIch.Add("Bếp");
                if (CoGiuXeMay) tienIch.Add("Gửi xe");
                if (CoDieuHoa) tienIch.Add("Điều hòa");
                if (CoNongLanh) tienIch.Add("Nóng lạnh");
                if (CoTuLanh) tienIch.Add("Tủ lạnh");
                if (CoTivi) tienIch.Add("TV");
                return tienIch.Count > 0 ? string.Join(", ", tienIch) : "Không có";
            }
        }

        public PhongTro()
        {
            DanhSachKhach = new ObservableCollection<KhachThue>();
            TrangThai = TrangThaiPhong.Trong;
            GiaDien = 3500;   // 3,500 đ/kWh mặc định
            GiaNuoc = 15000;  // 15,000 đ/m³ mặc định
            GiaInternet = 100000;
            GiaRac = 20000;
            TrangThaiNoiThat = "Cơ bản";
        }
    }
}
