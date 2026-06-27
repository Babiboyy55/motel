using baitaplon.Models;
using Quanlynhatro.Models;
using Quanlynhatro.ViewModels;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Win32;
using MiniExcelLibs;

namespace baitaplon.ViewModels.Admin
{
    // Kế thừa BaseViewModel để giao diện tự động cập nhật số liệu
    public class DashboardViewModel : BaseViewModel
    {
        private NhaTroDbContext _context;

        // --- BACKING FIELDS & PROPERTIES ---
        private int _tongSoPhong;
        public int TongSoPhong
        {
            get => _tongSoPhong;
            set { _tongSoPhong = value; OnPropertyChanged(); }
        }

        private int _soPhongTrong;
        public int SoPhongTrong
        {
            get => _soPhongTrong;
            set { _soPhongTrong = value; OnPropertyChanged(); }
        }

        private double _tyLeLapDay;
        public double TyLeLapDay
        {
            get => _tyLeLapDay;
            set { _tyLeLapDay = value; OnPropertyChanged(); }
        }

        private decimal _doanhThuThangNay;
        public decimal DoanhThuThangNay
        {
            get => _doanhThuThangNay;
            set { _doanhThuThangNay = value; OnPropertyChanged(); }
        }

        private decimal _tongCongNo;
        public decimal TongCongNo
        {
            get => _tongCongNo;
            set { _tongCongNo = value; OnPropertyChanged(); }
        }

        private ObservableCollection<HoaDon> _danhSachCongNo;
        public ObservableCollection<HoaDon> DanhSachCongNo
        {
            get => _danhSachCongNo;
            set { _danhSachCongNo = value; OnPropertyChanged(); }
        }

        private ObservableCollection<HoaDon> _lichSuThanhToan;
        public ObservableCollection<HoaDon> LichSuThanhToan
        {
            get => _lichSuThanhToan;
            set { _lichSuThanhToan = value; OnPropertyChanged(); }
        }

        // --- CÁC THUỘC TÍNH BỘ LỌC ---
        public List<int> DanhSachThang { get; } = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        public List<int> DanhSachNam { get; } = new List<int> { 2024, 2025, 2026, 2027, 2028 };

        private int _thangLoc;
        public int ThangLoc
        {
            get => _thangLoc;
            set { _thangLoc = value; OnPropertyChanged(); }
        }

        private int _namLoc;
        public int NamLoc
        {
            get => _namLoc;
            set { _namLoc = value; OnPropertyChanged(); }
        }

        // --- COMMAND CHO NÚT BẤM ---
        public ICommand LocDuLieuCommand { get; set; }
        public ICommand XuatExcelCommand { get; set; }

        // --- CONSTRUCTOR ---
        public DashboardViewModel()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _context = new NhaTroDbContext();

            // Đặt mặc định là tháng/năm hiện tại
            ThangLoc = DateTime.Now.Month;
            NamLoc = DateTime.Now.Year;

            // Khởi tạo các Command sử dụng RelayCommand của nhóm bạn
            LocDuLieuCommand = new RelayCommand(p => ThongKeDuLieu(), p => true);
            XuatExcelCommand = new RelayCommand(p => XuatBaoCao(), p => true);

            ThongKeDuLieu();
        }

        // --- LOGIC TÍNH TOÁN ---
        public void ThongKeDuLieu()
        {
            // 1. Thống kê Phòng (Giữ nguyên)
            TongSoPhong = _context.PhongTros.Count();
            SoPhongTrong = _context.PhongTros.Count(p => p.TrangThai == TrangThaiPhong.Trong);
            if (TongSoPhong > 0) TyLeLapDay = Math.Round((double)(TongSoPhong - SoPhongTrong) / TongSoPhong * 100, 2);

            // 2. Định dạng lại chuỗi tháng năm được chọn từ ComboBox (Ví dụ: "06/2026")
            string thangDuocChon = $"{ThangLoc:D2}/{NamLoc}";

            // 3. Truy vấn lấy theo tháng được lọc
            DoanhThuThangNay = _context.HoaDons
                .Where(h => h.Thang == ThangLoc && h.Nam == NamLoc && h.TrangThai == "Đã thanh toán")
                .ToList()
                .Sum(h => h.TongTien);

            // 4. Kéo danh sách nợ và lịch sử thanh toán (Cập nhật logic lấy theo tháng luôn nếu bạn muốn)
            TongCongNo = _context.HoaDons.Where(h => h.TrangThai != "Đã thanh toán").ToList().Sum(h => h.CongNo);

            DanhSachCongNo = new ObservableCollection<HoaDon>(_context.HoaDons
                .Where(h => h.TrangThai != "Đã thanh toán")
                .ToList()
                .OrderByDescending(h => h.TongTien));

            LichSuThanhToan = new ObservableCollection<HoaDon>(_context.HoaDons
                .Where(h => h.TrangThai == "Đã thanh toán" && h.Thang == ThangLoc && h.Nam == NamLoc)
                .OrderByDescending(h => h.NgayThanhToan).ToList());
        }

        // Hàm xuất báo cáo sử dụng MiniExcel
        private void XuatBaoCao()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"BaoCao_DoanhThu_Thang_{ThangLoc}_{NamLoc}.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                // Format lại dữ liệu cho đẹp trước khi đẩy ra Excel
                var dataToExport = LichSuThanhToan.Select(h => new {
                    MaHopDong = h.HopDongId,
                    KyThu = h.ThangNam,
                    TongTien = h.TongTien,
                    DaThu = h.SoTienDaTra,
                    ConNo = h.CongNo,
                    NgayThanhToan = h.NgayThanhToan?.ToString("dd/MM/yyyy") ?? "Chưa thanh toán",
                    TrangThai = h.TrangThai
                });

                MiniExcel.SaveAs(dialog.FileName, dataToExport);
                System.Windows.MessageBox.Show("Đã xuất báo cáo ra Excel thành công!", "Hoàn tất", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }
}