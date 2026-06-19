using System;
using System.ComponentModel.DataAnnotations;

namespace Quanlynhatro.Models
{
    /// <summary>
    /// KhachThue - Model đại diện cho một khách thuê
    /// </summary>
    public class KhachThue
    {
        [Key]
        public int KhachID { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public DateTime NgayVaoO { get; set; }
        public string CCCD { get; set; }
        public string DiaChi { get; set; }
        public int PhongID { get; set; }
    }
}
