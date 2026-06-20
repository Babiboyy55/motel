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
                var hopDongMoi = new HopDong
                {
                    PhongID = (int)cmbPhong.SelectedValue,
                    KhachID = (int)cmbKhach.SelectedValue,
                    NgayBatDau = dpNgayBatDau.SelectedDate ?? DateTime.Now,
                    NgayKetThuc = dpNgayKetThuc.SelectedDate ?? DateTime.Now.AddMonths(6),
                    GiaCoDinh = decimal.Parse(txtGiaCoDinh.Text),
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