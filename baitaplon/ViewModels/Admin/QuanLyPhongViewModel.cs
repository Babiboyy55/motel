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
    /// QuanLyPhongViewModel - Hoàn thiện CRUD phòng trọ
    /// </summary>
    public class QuanLyPhongViewModel : BaseViewModel
    {
        private ObservableCollection<PhongTro> _danhSachPhong;
        private ObservableCollection<PhongTro> _danhSachPhongGoc;
        private PhongTro _phongDangChon;
        private string _tuKhomTimKiem;
        private string _locTrangThai;

        // ---- Form thêm/sửa phòng ----
        private PhongTro _formPhong;
        private bool _isFormVisible;
        private bool _isEditMode;
        private string _formTitle;

        public ObservableCollection<PhongTro> DanhSachPhong
        {
            get => _danhSachPhong;
            set => SetProperty(ref _danhSachPhong, value);
        }

        public PhongTro PhongDangChon
        {
            get => _phongDangChon;
            set
            {
                SetProperty(ref _phongDangChon, value);
                OnPropertyChanged(nameof(CoPhongDangChon));
            }
        }

        public string TuKhoaTimKiem
        {
            get => _tuKhomTimKiem;
            set { SetProperty(ref _tuKhomTimKiem, value); TimKiem(); }
        }

        public string LocTrangThai
        {
            get => _locTrangThai;
            set { SetProperty(ref _locTrangThai, value); TimKiem(); }
        }

        // Form binding
        public PhongTro FormPhong
        {
            get => _formPhong;
            set => SetProperty(ref _formPhong, value);
        }

        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string FormTitle
        {
            get => _formTitle;
            set => SetProperty(ref _formTitle, value);
        }

        public bool CoPhongDangChon => PhongDangChon != null;

        // Thống kê
        public int SoPhongTong => DataService.DanhSachPhong.Count;
        public int SoPhongTrong => DataService.SoPhongTrong();
        public int SoPhongDangThue => DataService.SoPhongDangThue();
        public int SoPhongSuaChua => DataService.DanhSachPhong.Count(p => p.TrangThai == TrangThaiPhong.DangSuaChua);

        // ComboBox items
        public string[] DanhSachTrangThai => new[] { "Tất cả", "Trống", "Đang thuê", "Đang sửa chữa", "Đang dọn" };
        public LoaiPhong[] DanhSachLoaiPhong => (LoaiPhong[])Enum.GetValues(typeof(LoaiPhong));
        public TrangThaiPhong[] DanhSachTrangThaiPhong => (TrangThaiPhong[])Enum.GetValues(typeof(TrangThaiPhong));
        public string[] DanhSachNoiThat => new[] { "Đầy đủ", "Cơ bản", "Không có" };

        // Commands
        public ICommand ThemPhongCommand { get; }
        public ICommand SuaPhongCommand { get; }
        public ICommand XoaPhongCommand { get; }
        public ICommand LuuPhongCommand { get; }
        public ICommand HuyCommand { get; }
        public ICommand DoiTrangThaiCommand { get; }

        public QuanLyPhongViewModel()
        {
            _danhSachPhongGoc = DataService.DanhSachPhong;
            DanhSachPhong = new ObservableCollection<PhongTro>(_danhSachPhongGoc);
            LocTrangThai = "Tất cả";

            ThemPhongCommand = new RelayCommand(o => MoFormThem());
            SuaPhongCommand = new RelayCommand(o => MoFormSua(), o => PhongDangChon != null);
            XoaPhongCommand = new RelayCommand(o => XoaPhong(), o => PhongDangChon != null);
            LuuPhongCommand = new RelayCommand(o => LuuPhong());
            HuyCommand = new RelayCommand(o => DongForm());
            DoiTrangThaiCommand = new RelayCommand(o => DoiTrangThai(o?.ToString()), o => PhongDangChon != null);
        }

        private void TimKiem()
        {
            var ds = _danhSachPhongGoc.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
                ds = ds.Where(p => p.TenPhong.IndexOf(TuKhoaTimKiem, StringComparison.OrdinalIgnoreCase) >= 0
                                || (p.MoTa?.IndexOf(TuKhoaTimKiem, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0);

            if (LocTrangThai != "Tất cả" && !string.IsNullOrEmpty(LocTrangThai))
                ds = ds.Where(p => p.TrangThaiText == LocTrangThai);

            DanhSachPhong = new ObservableCollection<PhongTro>(ds);
        }

        private void MoFormThem()
        {
            FormPhong = new PhongTro();
            IsEditMode = false;
            FormTitle = "THÊM PHÒNG MỚI";
            IsFormVisible = true;
        }

        private void MoFormSua()
        {
            if (PhongDangChon == null) return;
            // Clone để tránh thay đổi trực tiếp
            FormPhong = new PhongTro
            {
                PhongID = PhongDangChon.PhongID,
                TenPhong = PhongDangChon.TenPhong,
                LoaiPhong = PhongDangChon.LoaiPhong,
                Tang = PhongDangChon.Tang,
                DienTich = PhongDangChon.DienTich,
                GiaThue = PhongDangChon.GiaThue,
                GiaDien = PhongDangChon.GiaDien,
                GiaNuoc = PhongDangChon.GiaNuoc,
                GiaInternet = PhongDangChon.GiaInternet,
                GiaRac = PhongDangChon.GiaRac,
                GiaXeMay = PhongDangChon.GiaXeMay,
                TrangThai = PhongDangChon.TrangThai,
                TrangThaiNoiThat = PhongDangChon.TrangThaiNoiThat,
                MoTa = PhongDangChon.MoTa,
                HinhAnh = PhongDangChon.HinhAnh,
                CoWifi = PhongDangChon.CoWifi,
                CoMayGiat = PhongDangChon.CoMayGiat,
                CoBep = PhongDangChon.CoBep,
                CoGiuXeMay = PhongDangChon.CoGiuXeMay,
                CoDieuHoa = PhongDangChon.CoDieuHoa,
                CoNongLanh = PhongDangChon.CoNongLanh,
                CoTuLanh = PhongDangChon.CoTuLanh,
                CoTivi = PhongDangChon.CoTivi,
            };
            IsEditMode = true;
            FormTitle = $"SỬA PHÒNG - {PhongDangChon.TenPhong}";
            IsFormVisible = true;
        }

        private void LuuPhong()
        {
            if (FormPhong == null) return;
            if (string.IsNullOrWhiteSpace(FormPhong.TenPhong))
            {
                MessageBox.Show("Vui lòng nhập tên/số phòng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsEditMode)
                DataService.SuaPhong(FormPhong);
            else
                DataService.ThemPhong(FormPhong);

            DongForm();
            LamMoiDanhSach();
            CapNhatThongKe();
        }

        private void XoaPhong()
        {
            if (PhongDangChon == null) return;
            if (PhongDangChon.TrangThai == TrangThaiPhong.DangThue)
            {
                MessageBox.Show("Không thể xóa phòng đang có khách thuê!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Bạn có chắc muốn xóa phòng {PhongDangChon.TenPhong}?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataService.XoaPhong(PhongDangChon.PhongID);
                LamMoiDanhSach();
                CapNhatThongKe();
            }
        }

        private void DoiTrangThai(string trangThai)
        {
            if (PhongDangChon == null || string.IsNullOrEmpty(trangThai)) return;
            TrangThaiPhong newStatus = trangThai switch
            {
                "Trống" => TrangThaiPhong.Trong,
                "Đang thuê" => TrangThaiPhong.DangThue,
                "Đang sửa chữa" => TrangThaiPhong.DangSuaChua,
                "Đang dọn" => TrangThaiPhong.DangDon,
                _ => PhongDangChon.TrangThai
            };
            PhongDangChon.TrangThai = newStatus;
            DataService.SuaPhong(PhongDangChon);
            LamMoiDanhSach();
            CapNhatThongKe();
        }

        private void DongForm() => IsFormVisible = false;

        private void LamMoiDanhSach()
        {
            _danhSachPhongGoc = DataService.DanhSachPhong;
            TimKiem();
        }

        private void CapNhatThongKe()
        {
            OnPropertyChanged(nameof(SoPhongTong));
            OnPropertyChanged(nameof(SoPhongTrong));
            OnPropertyChanged(nameof(SoPhongDangThue));
            OnPropertyChanged(nameof(SoPhongSuaChua));
        }
    }
}
