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
    /// QuanLyHoaDonViewModel - Quản lý thu chi, hóa đơn, công nợ
    /// </summary>
    public class QuanLyHoaDonViewModel : BaseViewModel
    {
        private ObservableCollection<HoaDon> _danhSachHoaDon;
        private HoaDon _hoaDonDangChon;
        private int _thangLoc;
        private int _namLoc;
        private string _locTrangThai;

        // Form hóa đơn
        private HoaDon _formHoaDon;
        private bool _isFormHoaDonVisible;
        private bool _isEditHoaDon;
        private string _formHoaDonTitle;

        // Form thanh toán
        private decimal _soTienThanhToan;
        private string _hinhThucThanhToan;
        private bool _isFormThanhToanVisible;

        public ObservableCollection<HoaDon> DanhSachHoaDon
        {
            get => _danhSachHoaDon;
            set => SetProperty(ref _danhSachHoaDon, value);
        }

        public HoaDon HoaDonDangChon
        {
            get => _hoaDonDangChon;
            set { SetProperty(ref _hoaDonDangChon, value); OnPropertyChanged(nameof(CoHoaDonDangChon)); }
        }

        public int ThangLoc
        {
            get => _thangLoc;
            set { SetProperty(ref _thangLoc, value); TimKiem(); }
        }

        public int NamLoc
        {
            get => _namLoc;
            set { SetProperty(ref _namLoc, value); TimKiem(); }
        }

        public string LocTrangThai
        {
            get => _locTrangThai;
            set { SetProperty(ref _locTrangThai, value); TimKiem(); }
        }

        // Form HoaDon
        public HoaDon FormHoaDon
        {
            get => _formHoaDon;
            set => SetProperty(ref _formHoaDon, value);
        }

        public bool IsFormHoaDonVisible
        {
            get => _isFormHoaDonVisible;
            set => SetProperty(ref _isFormHoaDonVisible, value);
        }

        public string FormHoaDonTitle
        {
            get => _formHoaDonTitle;
            set => SetProperty(ref _formHoaDonTitle, value);
        }

        // Form Thanh Toán
        public decimal SoTienThanhToan
        {
            get => _soTienThanhToan;
            set => SetProperty(ref _soTienThanhToan, value);
        }

        public string HinhThucThanhToan
        {
            get => _hinhThucThanhToan;
            set => SetProperty(ref _hinhThucThanhToan, value);
        }

        public bool IsFormThanhToanVisible
        {
            get => _isFormThanhToanVisible;
            set => SetProperty(ref _isFormThanhToanVisible, value);
        }

        public bool CoHoaDonDangChon => HoaDonDangChon != null;

        // Thống kê
        public decimal TongThuThang => DataService.TongThuThang(ThangLoc, NamLoc);
        public decimal TongConNo => DataService.TongConNo();
        public int SoHoaDonChuaThanhToan => DataService.DanhSachHoaDon.Count(h => h.TrangThai == "Chưa thanh toán");
        public int SoHoaDonDaThanhToan => DataService.DanhSachHoaDon.Count(h => h.TrangThai == "Đã thanh toán" && h.Thang == ThangLoc && h.Nam == NamLoc);

        // Danh sách phòng cho ComboBox
        public ObservableCollection<PhongTro> DanhSachPhong => DataService.DanhSachPhong;
        public string[] DanhSachTrangThaiHD => new[] { "Tất cả", "Chưa thanh toán", "Trả một phần", "Đã thanh toán" };
        public string[] DanhSachHinhThucTT => new[] { "Tiền mặt", "Chuyển khoản", "Ví điện tử" };
        public int[] DanhSachThang => Enumerable.Range(1, 12).ToArray();
        public int[] DanhSachNam => Enumerable.Range(2020, 10).ToArray();

        // Commands
        public ICommand TaoHoaDonTuDongCommand { get; }
        public ICommand ThemHoaDonThuCongCommand { get; }
        public ICommand SuaHoaDonCommand { get; }
        public ICommand XoaHoaDonCommand { get; }
        public ICommand LuuHoaDonCommand { get; }
        public ICommand HuyHoaDonCommand { get; }
        public ICommand GhiNhanThanhToanCommand { get; }
        public ICommand MoThanhToanCommand { get; }
        public ICommand HuyThanhToanCommand { get; }

        public QuanLyHoaDonViewModel()
        {
            ThangLoc = DateTime.Today.Month;
            NamLoc = DateTime.Today.Year;
            LocTrangThai = "Tất cả";
            HinhThucThanhToan = "Tiền mặt";

            TaoHoaDonTuDongCommand = new RelayCommand(o => TaoHoaDonTuDong());
            ThemHoaDonThuCongCommand = new RelayCommand(o => MoFormThemHoaDon());
            SuaHoaDonCommand = new RelayCommand(o => MoFormSuaHoaDon(), o => HoaDonDangChon != null);
            XoaHoaDonCommand = new RelayCommand(o => XoaHoaDon(), o => HoaDonDangChon != null);
            LuuHoaDonCommand = new RelayCommand(o => LuuHoaDon());
            HuyHoaDonCommand = new RelayCommand(o => IsFormHoaDonVisible = false);
            GhiNhanThanhToanCommand = new RelayCommand(o => GhiNhanThanhToan());
            MoThanhToanCommand = new RelayCommand(o => MoFormThanhToan(), o => HoaDonDangChon != null && HoaDonDangChon.TrangThai != "Đã thanh toán");
            HuyThanhToanCommand = new RelayCommand(o => IsFormThanhToanVisible = false);

            LamMoiDanhSach();
        }

        private void TimKiem()
        {
            var ds = DataService.DanhSachHoaDon.AsEnumerable();

            if (ThangLoc > 0)
                ds = ds.Where(h => h.Thang == ThangLoc && h.Nam == NamLoc);

            if (LocTrangThai != "Tất cả" && !string.IsNullOrEmpty(LocTrangThai))
                ds = ds.Where(h => h.TrangThai == LocTrangThai);

            DanhSachHoaDon = new ObservableCollection<HoaDon>(ds.OrderByDescending(h => h.HoaDonID));
            CapNhatThongKe();
        }

        private void TaoHoaDonTuDong()
        {
            var res = MessageBox.Show($"Tạo hóa đơn tự động cho tháng {ThangLoc}/{NamLoc}?\nHệ thống sẽ tạo hóa đơn cho tất cả phòng đang thuê chưa có hóa đơn tháng này.",
                "Xác nhận tạo hóa đơn", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                var dsHD = DataService.TaoHoaDonTuDong(ThangLoc, NamLoc);
                if (dsHD.Count == 0)
                    MessageBox.Show("Tất cả phòng đang thuê đã có hóa đơn tháng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show($"Đã tạo {dsHD.Count} hóa đơn thành công!\nVui lòng nhập chỉ số điện/nước cho từng phòng.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                LamMoiDanhSach();
            }
        }

        private void MoFormThemHoaDon()
        {
            FormHoaDon = new HoaDon { Thang = ThangLoc, Nam = NamLoc };
            _isEditHoaDon = false;
            FormHoaDonTitle = "TẠO HÓA ĐƠN THỦ CÔNG";
            IsFormHoaDonVisible = true;
        }

        private void MoFormSuaHoaDon()
        {
            if (HoaDonDangChon == null) return;
            FormHoaDon = new HoaDon
            {
                HoaDonID = HoaDonDangChon.HoaDonID,
                PhongID = HoaDonDangChon.PhongID,
                TenPhong = HoaDonDangChon.TenPhong,
                TenKhach = HoaDonDangChon.TenKhach,
                Thang = HoaDonDangChon.Thang,
                Nam = HoaDonDangChon.Nam,
                TienThue = HoaDonDangChon.TienThue,
                ChiSoDienDau = HoaDonDangChon.ChiSoDienDau,
                ChiSoDienCuoi = HoaDonDangChon.ChiSoDienCuoi,
                GiaDien = HoaDonDangChon.GiaDien,
                ChiSoNuocDau = HoaDonDangChon.ChiSoNuocDau,
                ChiSoNuocCuoi = HoaDonDangChon.ChiSoNuocCuoi,
                GiaNuoc = HoaDonDangChon.GiaNuoc,
                TienInternet = HoaDonDangChon.TienInternet,
                TienRac = HoaDonDangChon.TienRac,
                TienXe = HoaDonDangChon.TienXe,
                PhiKhac = HoaDonDangChon.PhiKhac,
                GhiChuPhiKhac = HoaDonDangChon.GhiChuPhiKhac,
                TrangThai = HoaDonDangChon.TrangThai,
                SoTienDaTra = HoaDonDangChon.SoTienDaTra,
                GhiChu = HoaDonDangChon.GhiChu
            };
            _isEditHoaDon = true;
            FormHoaDonTitle = $"SỬA HÓA ĐƠN - {HoaDonDangChon.TenPhong} ({HoaDonDangChon.ThangNamText})";
            IsFormHoaDonVisible = true;
        }

        private void LuuHoaDon()
        {
            if (FormHoaDon == null) return;
            if (FormHoaDon.PhongID == 0)
            {
                MessageBox.Show("Vui lòng chọn phòng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gán tên phòng và khách
            var phong = DataService.DanhSachPhong.FirstOrDefault(p => p.PhongID == FormHoaDon.PhongID);
            FormHoaDon.TenPhong = phong?.TenPhong;
            var khach = DataService.DanhSachKhach.FirstOrDefault(k => k.PhongID == FormHoaDon.PhongID && k.DangO);
            FormHoaDon.TenKhach = khach?.HoTen ?? "(Chưa rõ)";

            if (_isEditHoaDon)
                DataService.SuaHoaDon(FormHoaDon);
            else
                DataService.ThemHoaDon(FormHoaDon);

            IsFormHoaDonVisible = false;
            LamMoiDanhSach();
        }

        private void XoaHoaDon()
        {
            if (HoaDonDangChon == null) return;
            if (HoaDonDangChon.TrangThai == "Đã thanh toán")
            {
                MessageBox.Show("Không thể xóa hóa đơn đã thanh toán!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var res = MessageBox.Show($"Xóa hóa đơn phòng {HoaDonDangChon.TenPhong} tháng {HoaDonDangChon.Thang}/{HoaDonDangChon.Nam}?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                DataService.XoaHoaDon(HoaDonDangChon.HoaDonID);
                LamMoiDanhSach();
            }
        }

        private void MoFormThanhToan()
        {
            if (HoaDonDangChon == null) return;
            SoTienThanhToan = HoaDonDangChon.ConNo;
            HinhThucThanhToan = "Tiền mặt";
            IsFormThanhToanVisible = true;
        }

        private void GhiNhanThanhToan()
        {
            if (HoaDonDangChon == null) return;
            if (SoTienThanhToan <= 0)
            {
                MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataService.GhiNhanThanhToan(HoaDonDangChon.HoaDonID, SoTienThanhToan, HinhThucThanhToan);
            IsFormThanhToanVisible = false;
            MessageBox.Show("Đã ghi nhận thanh toán thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            LamMoiDanhSach();
        }

        private void LamMoiDanhSach()
        {
            TimKiem();
        }

        private void CapNhatThongKe()
        {
            OnPropertyChanged(nameof(TongThuThang));
            OnPropertyChanged(nameof(TongConNo));
            OnPropertyChanged(nameof(SoHoaDonChuaThanhToan));
            OnPropertyChanged(nameof(SoHoaDonDaThanhToan));
        }
    }
}
