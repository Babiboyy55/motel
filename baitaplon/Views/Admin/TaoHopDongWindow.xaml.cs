using baitaplon.Models;
using Quanlynhatro.Models;
using System;
using System.Linq;
using System.Windows;

namespace baitaplon.Views.Admin
{
    public partial class TaoHopDongWindow : Window
    {
        public TaoHopDongWindow()
        {
            InitializeComponent();
            LoadDuLieu(); // Gọi hàm tải dữ liệu ngay khi mở form
        }

        private void LoadDuLieu()
        {
            using (var db = new NhaTroDbContext())
            {
                // Kéo danh sách phòng (chỉ lấy phòng trống) và khách thuê lên ComboBox
                // (Ghi chú: Nếu bảng PhongTro chưa có cột TrangThai, bạn có thể xóa đoạn .Where(...) đi)
                cmbPhong.ItemsSource = db.PhongTros.ToList();
                cmbKhach.ItemsSource = db.KhachThues.ToList();
            }
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            // 1. Kiểm tra xem Admin đã chọn/nhập đủ thông tin chưa
            if (cmbPhong.SelectedValue == null || cmbKhach.SelectedValue == null || string.IsNullOrWhiteSpace(txtGiaCoDinh.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Lưu xuống Database
            using (var db = new NhaTroDbContext())
            {
                var selectedPhongId = (int)cmbPhong.SelectedValue;
                var selectedKhachId = (int)cmbKhach.SelectedValue;

                var phong = db.PhongTros.FirstOrDefault(p => p.PhongID == selectedPhongId);
                var khach = db.KhachThues.FirstOrDefault(k => k.KhachID == selectedKhachId);

                var hopDongMoi = new HopDong
                {
                    PhongID = selectedPhongId,
                    KhachID = selectedKhachId,
                    TenPhong = phong?.TenPhong ?? "",
                    TenKhach = khach?.HoTen ?? "",
                    NgayBatDau = dpNgayBatDau.SelectedDate ?? DateTime.Now,
                    NgayKetThuc = dpNgayKetThuc.SelectedDate ?? DateTime.Now.AddMonths(6),
                    GiaThue = decimal.Parse(txtGiaCoDinh.Text),
                    TienCoc = decimal.Parse(txtGiaCoDinh.Text), // Mặc định cọc = 1 tháng tiền nhà
                    TrangThai = "Hoạt động"
                };

                db.HopDongs.Add(hopDongMoi);
                db.SaveChanges(); // Lệnh chốt hạ đẩy dữ liệu xuống SQL Server
            }

            MessageBox.Show("Tạo hợp đồng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close(); // Lưu xong thì tự động đóng cửa sổ
        }
    }
}