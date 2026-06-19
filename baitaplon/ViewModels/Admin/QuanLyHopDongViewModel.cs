using baitaplon.Models;
using Quanlynhatro.Models;
using Quanlynhatro.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows; // Dùng để gọi MessageBox

namespace baitaplon.ViewModels.Admin
{
    public class QuanLyHopDongViewModel : BaseViewModel
    {
        private NhaTroDbContext _context;

        // 1. Danh sách Hợp đồng để hiển thị lên bảng
        private ObservableCollection<HopDong> _danhSachHopDong;
        public ObservableCollection<HopDong> DanhSachHopDong
        {
            get => _danhSachHopDong;
            set { _danhSachHopDong = value; OnPropertyChanged(); }
        }

        // 2. Danh sách Lịch nhắc việc (Tự động sinh ra dựa trên logic)
        private ObservableCollection<ThongBaoNhacViec> _danhSachNhacViec;
        public ObservableCollection<ThongBaoNhacViec> DanhSachNhacViec
        {
            get => _danhSachNhacViec;
            set { _danhSachNhacViec = value; OnPropertyChanged(); }
        }

        // Thuộc tính lưu trữ hợp đồng mà Admin đang click chọn trên DataGrid
        private HopDong _selectedHopDong;
        public HopDong SelectedHopDong
        {
            get => _selectedHopDong;
            set { _selectedHopDong = value; OnPropertyChanged(); }
        }

        // Khai báo 2 Command
        public ICommand GiaHanCommand { get; set; }
        public ICommand ThanhLyCommand { get; set; }

        public QuanLyHopDongViewModel()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _context = new NhaTroDbContext();
            TaiDuLieu();

            // Command chỉ cho phép bấm nút nếu đã chọn 1 dòng trên lưới (SelectedHopDong != null)
            GiaHanCommand = new RelayCommand(p => ThucHienGiaHan(), p => SelectedHopDong != null);
            ThanhLyCommand = new RelayCommand(p => ThucHienThanhLy(), p => SelectedHopDong != null);
        }

        public void TaiDuLieu()
        {
            // Lấy toàn bộ hợp đồng
            DanhSachHopDong = new ObservableCollection<HopDong>(_context.HopDongs.ToList());

            // KHỞI TẠO LOGIC NHẮC VIỆC TỰ ĐỘNG
            DanhSachNhacViec = new ObservableCollection<ThongBaoNhacViec>();
            DateTime homNay = DateTime.Now;
            DateTime mienCanhBao = homNay.AddDays(30); // Cảnh báo trước 30 ngày

            // A. Quét Hợp đồng sắp hết hạn
            var hopDongSapHet = _context.HopDongs
                .Where(h => h.NgayKetThuc >= homNay && h.NgayKetThuc <= mienCanhBao)
                .ToList();

            foreach (var hd in hopDongSapHet)
            {
                int soNgayConLai = (hd.NgayKetThuc - homNay).Days;
                DanhSachNhacViec.Add(new ThongBaoNhacViec
                {
                    // Sửa MaHopDong thành HopDongID ở dòng này
                    TieuDe = $"Hợp đồng số {hd.HopDongID} sắp hết hạn",
                    NoiDung = $"Chỉ còn {soNgayConLai} ngày (đáo hạn vào {hd.NgayKetThuc:dd/MM/yyyy}). Cần liên hệ khách thuê.",
                    MauSac = "#E74C3C",
                    Icon = "⚠️"
                });
            }

            // B. Quét Hóa đơn chưa thanh toán (Nhắc thu tiền)
            var hoaDonChuaDong = _context.HoaDons
                .Where(h => h.TrangThai != "DaThanhToan")
                .ToList();

            foreach (var hd in hoaDonChuaDong)
            {
                DanhSachNhacViec.Add(new ThongBaoNhacViec
                {
                    TieuDe = $"Thu tiền tháng {hd.ThangNam}",
                    NoiDung = $"Hợp đồng {hd.HopDongId} còn nợ {hd.CongNo:N0} đ chưa thanh toán.",
                    MauSac = "#F39C12", // Màu Vàng cam: Nhắc nhở
                    Icon = "💰"
                });
            }

            // C. Nhắc nhở bảo trì (Có thể cài đặt cứng định kỳ hoặc tạo thêm bảng sau này)
            DanhSachNhacViec.Add(new ThongBaoNhacViec
            {
                TieuDe = "Lịch bảo trì định kỳ",
                NoiDung = "Đã đến hạn kiểm tra hệ thống PCCC và vệ sinh điều hòa các phòng khu A.",
                MauSac = "#3498DB", // Màu Xanh lam: Thông tin
                Icon = "🛠️"
            });
        }

        private void ThucHienGiaHan()
        {
            // Tìm bản ghi gốc trong Database dựa vào ID
            var hdDb = _context.HopDongs.Find(SelectedHopDong.HopDongID);
            if (hdDb != null)
            {
                // Cộng thêm 6 tháng vào ngày kết thúc hiện tại
                hdDb.NgayKetThuc = hdDb.NgayKetThuc.AddMonths(6);
                hdDb.TrangThai = "Hoạt động"; // Đảm bảo trạng thái đang mở

                _context.SaveChanges(); // Lưu xuống DB
                TaiDuLieu(); // Gọi hàm tải lại dữ liệu để làm mới DataGrid và Lịch nhắc việc

                MessageBox.Show($"Đã gia hạn hợp đồng {hdDb.HopDongID} thêm 6 tháng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ThucHienThanhLy()
        {
            // Hiển thị hộp thoại hỏi lại cho chắc chắn
            var xacNhan = MessageBox.Show($"Bạn có chắc chắn muốn thanh lý hợp đồng {SelectedHopDong.HopDongID} không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (xacNhan == MessageBoxResult.Yes)
            {
                var hdDb = _context.HopDongs.Find(SelectedHopDong.HopDongID);
                if (hdDb != null)
                {
                    // Chuyển trạng thái sang Hết hạn
                    hdDb.TrangThai = "Hết hạn";

                    _context.SaveChanges();
                    TaiDuLieu();

                    MessageBox.Show("Đã thanh lý hợp đồng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }

    // Lớp chứa dữ liệu cho Trung tâm thông báo (Notification)
    public class ThongBaoNhacViec
    {
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string MauSac { get; set; }
        public string Icon { get; set; }
    }
}