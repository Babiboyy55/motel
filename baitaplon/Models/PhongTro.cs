using System;
using System.Collections.ObjectModel;

namespace Quanlynhatro.Models
{
    /// <summary>
    /// PhongTro - Model đại diện cho một phòng trọ
    /// </summary>
    public class PhongTro
    {
        public int PhongID { get; set; }
        public string TenPhong { get; set; }
        public string Dia { get; set; }
        public decimal GiaThue { get; set; }
        public int DienTich { get; set; }
        public string TrangThai { get; set; } // "Trống" hoặc "Đã cho thuê"
        public string MoTa { get; set; }
        public ObservableCollection<KhachThue> DanhSachKhach { get; set; }

        public PhongTro()
        {
            DanhSachKhach = new ObservableCollection<KhachThue>();
        }
    }
}
