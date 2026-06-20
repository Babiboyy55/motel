using Quanlynhatro.ViewModels;
using Quanlynhatro.Models;
using Quanlynhatro.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.Admin
{
    /// <summary>
    /// QuanLyKhachViewModel - Quản lý khách thuê và hợp đồng
    /// </summary>
    public class QuanLyKhachViewModel : BaseViewModel
    {
        private ObservableCollection<KhachThue> _danhSachKhach;
        private ObservableCollection<HopDong> _danhSachHopDong;
        private KhachThue _khachDangChon;
        private HopDong _hopDongDangChon;
        private string _tuKhoaTimKiem;
        private int _tabIndex; // 0 = Khách thuê, 1 = Hợp đồng

        // Form khách
        private KhachThue _formKhach;
        private bool _isFormKhachVisible;
        private bool _isEditKhach;
        private string _formKhachTitle;

        // Form hợp đồng
        private HopDong _formHopDong;
        private bool _isFormHopDongVisible;
        private bool _isEditHopDong;
        private string _formHopDongTitle;

        public ObservableCollection<KhachThue> DanhSachKhach
        {
            get => _danhSachKhach;
            set => SetProperty(ref _danhSachKhach, value);
        }

        public ObservableCollection<HopDong> DanhSachHopDong
        {
            get => _danhSachHopDong;
            set => SetProperty(ref _danhSachHopDong, value);
        }

        public KhachThue KhachDangChon
        {
            get => _khachDangChon;
            set { SetProperty(ref _khachDangChon, value); OnPropertyChanged(nameof(CoKhachDangChon)); }
        }

        public HopDong HopDongDangChon
        {
            get => _hopDongDangChon;
            set { SetProperty(ref _hopDongDangChon, value); OnPropertyChanged(nameof(CoHopDongDangChon)); }
        }

        public string TuKhoaTimKiem
        {
            get => _tuKhoaTimKiem;
            set { SetProperty(ref _tuKhoaTimKiem, value); TimKiem(); }
        }

        public int TabIndex
        {
            get => _tabIndex;
            set => SetProperty(ref _tabIndex, value);
        }

        // Form Khách
        public KhachThue FormKhach
        {
            get => _formKhach;
            set => SetProperty(ref _formKhach, value);
        }

        public bool IsFormKhachVisible
        {
            get => _isFormKhachVisible;
            set => SetProperty(ref _isFormKhachVisible, value);
        }

        public string FormKhachTitle
        {
            get => _formKhachTitle;
            set => SetProperty(ref _formKhachTitle, value);
        }

        // Form HopDong
        public HopDong FormHopDong
        {
            get => _formHopDong;
            set => SetProperty(ref _formHopDong, value);
        }

        public bool IsFormHopDongVisible
        {
            get => _isFormHopDongVisible;
            set => SetProperty(ref _isFormHopDongVisible, value);
        }

        public string FormHopDongTitle
        {
            get => _formHopDongTitle;
            set => SetProperty(ref _formHopDongTitle, value);
        }

        public bool CoKhachDangChon => KhachDangChon != null;
        public bool CoHopDongDangChon => HopDongDangChon != null;

        // Thống kê
        public int TongSoKhach => DataService.DanhSachKhach.Count(k => k.DangO);
        public int SoHopDongHoatDong => DataService.DanhSachHopDong.Count(h => h.TrangThai == "Hoạt động");
        public int SoHopDongSapHetHan => DataService.DanhSachHopDong.Count(h => h.TrangThai == "Hoạt động" && h.NgayKetThuc <= DateTime.Today.AddDays(30));

        // Danh sách phòng cho ComboBox
        public ObservableCollection<PhongTro> DanhSachPhong => DataService.DanhSachPhong;
        public string[] DanhSachGioiTinh => new[] { "Nam", "Nữ", "Khác" };
        public string[] DanhSachQuanHe => new[] { "Bố", "Mẹ", "Anh", "Chị", "Em", "Vợ", "Chồng", "Bạn bè", "Khác" };

        // Commands - Khách
        public ICommand ThemKhachCommand { get; }
        public ICommand SuaKhachCommand { get; }
        public ICommand XoaKhachCommand { get; }
        public ICommand LuuKhachCommand { get; }
        public ICommand HuyKhachCommand { get; }

        // Commands - HopDong
        public ICommand ThemHopDongCommand { get; }
        public ICommand SuaHopDongCommand { get; }
        public ICommand ChamDutHopDongCommand { get; }
        public ICommand LuuHopDongCommand { get; }
        public ICommand HuyHopDongCommand { get; }

        public QuanLyKhachViewModel()
        {
            DanhSachKhach = new ObservableCollection<KhachThue>(DataService.DanhSachKhach);
            DanhSachHopDong = new ObservableCollection<HopDong>(DataService.DanhSachHopDong);

            ThemKhachCommand = new RelayCommand(o => MoFormThemKhach());
            SuaKhachCommand = new RelayCommand(o => MoFormSuaKhach(), o => KhachDangChon != null);
            XoaKhachCommand = new RelayCommand(o => XoaKhach(), o => KhachDangChon != null);
            LuuKhachCommand = new RelayCommand(o => LuuKhach());
            HuyKhachCommand = new RelayCommand(o => IsFormKhachVisible = false);

            ThemHopDongCommand = new RelayCommand(o => MoFormThemHopDong());
            SuaHopDongCommand = new RelayCommand(o => MoFormSuaHopDong(), o => HopDongDangChon != null);
            ChamDutHopDongCommand = new RelayCommand(o => ChamDutHopDong(), o => HopDongDangChon?.TrangThai == "Hoạt động");
            LuuHopDongCommand = new RelayCommand(o => LuuHopDong());
            HuyHopDongCommand = new RelayCommand(o => IsFormHopDongVisible = false);
        }

        private void TimKiem()
        {
            if (string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                DanhSachKhach = new ObservableCollection<KhachThue>(DataService.DanhSachKhach);
                DanhSachHopDong = new ObservableCollection<HopDong>(DataService.DanhSachHopDong);
            }
            else
            {
                string kw = TuKhoaTimKiem.ToLower();
                DanhSachKhach = new ObservableCollection<KhachThue>(
                    DataService.DanhSachKhach.Where(k =>
                        (k.HoTen?.ToLower().Contains(kw) ?? false) ||
                        (k.CCCD?.Contains(kw) ?? false) ||
                        (k.SoDienThoai?.Contains(kw) ?? false)));

                DanhSachHopDong = new ObservableCollection<HopDong>(
                    DataService.DanhSachHopDong.Where(h =>
                        (h.TenKhach?.ToLower().Contains(kw) ?? false) ||
                        (h.TenPhong?.Contains(kw) ?? false)));
            }
        }

        // ---- Khách ----
        private void MoFormThemKhach()
        {
            FormKhach = new KhachThue();
            _isEditKhach = false;
            FormKhachTitle = "THÊM KHÁCH THUÊ MỚI";
            IsFormKhachVisible = true;
        }

        private void MoFormSuaKhach()
        {
            if (KhachDangChon == null) return;
            FormKhach = new KhachThue
            {
                KhachID = KhachDangChon.KhachID,
                HoTen = KhachDangChon.HoTen,
                CCCD = KhachDangChon.CCCD,
                SoDienThoai = KhachDangChon.SoDienThoai,
                Email = KhachDangChon.Email,
                NgaySinh = KhachDangChon.NgaySinh,
                GioiTinh = KhachDangChon.GioiTinh,
                QueQuan = KhachDangChon.QueQuan,
                DiaChiHienTai = KhachDangChon.DiaChiHienTai,
                NguoiLienHeKhan = KhachDangChon.NguoiLienHeKhan,
                QuanHeNguoiLienHe = KhachDangChon.QuanHeNguoiLienHe,
                SoDTNguoiLienHe = KhachDangChon.SoDTNguoiLienHe,
                PhongID = KhachDangChon.PhongID,
                NgayVaoO = KhachDangChon.NgayVaoO,
                DangO = KhachDangChon.DangO,
                GhiChu = KhachDangChon.GhiChu
            };
            _isEditKhach = true;
            FormKhachTitle = $"SỬA THÔNG TIN - {KhachDangChon.HoTen}";
            IsFormKhachVisible = true;
        }

        private void LuuKhach()
        {
            if (FormKhach == null) return;
            if (string.IsNullOrWhiteSpace(FormKhach.HoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên khách!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditKhach)
                DataService.SuaKhach(FormKhach);
            else
                DataService.ThemKhach(FormKhach);

            IsFormKhachVisible = false;
            LamMoiDanhSach();
        }

        private void XoaKhach()
        {
            if (KhachDangChon == null) return;
            if (KhachDangChon.DangO)
            {
                MessageBox.Show("Không thể xóa khách đang ở! Hãy chấm dứt hợp đồng trước.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var res = MessageBox.Show($"Xóa khách '{KhachDangChon.HoTen}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                DataService.XoaKhach(KhachDangChon.KhachID);
                LamMoiDanhSach();
            }
        }

        // ---- Hợp Đồng ----
        private void MoFormThemHopDong()
        {
            FormHopDong = new HopDong();
            _isEditHopDong = false;
            FormHopDongTitle = "TẠO HỢP ĐỒNG MỚI";
            IsFormHopDongVisible = true;
        }

        private void MoFormSuaHopDong()
        {
            if (HopDongDangChon == null) return;
            FormHopDong = new HopDong
            {
                HopDongID = HopDongDangChon.HopDongID,
                KhachID = HopDongDangChon.KhachID,
                PhongID = HopDongDangChon.PhongID,
                TenKhach = HopDongDangChon.TenKhach,
                TenPhong = HopDongDangChon.TenPhong,
                NgayBatDau = HopDongDangChon.NgayBatDau,
                NgayKetThuc = HopDongDangChon.NgayKetThuc,
                GiaThue = HopDongDangChon.GiaThue,
                TienCoc = HopDongDangChon.TienCoc,
                DaHoanCoc = HopDongDangChon.DaHoanCoc,
                DieuKhoan = HopDongDangChon.DieuKhoan,
                TrangThai = HopDongDangChon.TrangThai,
                GhiChu = HopDongDangChon.GhiChu
            };
            _isEditHopDong = true;
            FormHopDongTitle = $"SỬA HỢP ĐỒNG #{HopDongDangChon.HopDongID}";
            IsFormHopDongVisible = true;
        }

        private void LuuHopDong()
        {
            if (FormHopDong == null) return;
            if (FormHopDong.KhachID == 0 || FormHopDong.PhongID == 0)
            {
                MessageBox.Show("Vui lòng chọn khách và phòng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gán tên khách, tên phòng
            var khach = DataService.DanhSachKhach.FirstOrDefault(k => k.KhachID == FormHopDong.KhachID);
            var phong = DataService.DanhSachPhong.FirstOrDefault(p => p.PhongID == FormHopDong.PhongID);
            FormHopDong.TenKhach = khach?.HoTen;
            FormHopDong.TenPhong = phong?.TenPhong;
            FormHopDong.GiaThue = phong?.GiaThue ?? FormHopDong.GiaThue;

            if (_isEditHopDong)
                DataService.SuaHopDong(FormHopDong);
            else
                DataService.ThemHopDong(FormHopDong);

            IsFormHopDongVisible = false;
            LamMoiDanhSach();
        }

        private void ChamDutHopDong()
        {
            if (HopDongDangChon == null) return;
            var res = MessageBox.Show($"Chấm dứt hợp đồng #{HopDongDangChon.HopDongID} với phòng {HopDongDangChon.TenPhong}?\nPhòng sẽ được chuyển về trạng thái Trống.",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                DataService.ChamDutHopDong(HopDongDangChon.HopDongID);
                LamMoiDanhSach();
            }
        }

        private void LamMoiDanhSach()
        {
            DanhSachKhach = new ObservableCollection<KhachThue>(DataService.DanhSachKhach);
            DanhSachHopDong = new ObservableCollection<HopDong>(DataService.DanhSachHopDong);
            CapNhatThongKe();
        }

        private void CapNhatThongKe()
        {
            OnPropertyChanged(nameof(TongSoKhach));
            OnPropertyChanged(nameof(SoHopDongHoatDong));
            OnPropertyChanged(nameof(SoHopDongSapHetHan));
        }
    }
}
