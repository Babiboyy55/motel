using Quanlynhatro.ViewModels;
using Quanlynhatro.Models;
using baitaplon.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.Admin
{
    /// <summary>
    /// QuanLyPhongViewModel - CRUD phòng trọ kết nối SQL Server thông qua Entity Framework
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

        // Thống kê (Lấy trực tiếp từ Database)
        public int SoPhongTong
        {
            get
            {
                using (var db = new NhaTroDbContext())
                    return db.PhongTros.Count();
            }
        }

        public int SoPhongTrong
        {
            get
            {
                using (var db = new NhaTroDbContext())
                    return db.PhongTros.Count(p => p.TrangThai == TrangThaiPhong.Trong);
            }
        }

        public int SoPhongDangThue
        {
            get
            {
                using (var db = new NhaTroDbContext())
                    return db.PhongTros.Count(p => p.TrangThai == TrangThaiPhong.DangThue);
            }
        }

        public int SoPhongSuaChua
        {
            get
            {
                using (var db = new NhaTroDbContext())
                    return db.PhongTros.Count(p => p.TrangThai == TrangThaiPhong.DangSuaChua);
            }
        }

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
            LocTrangThai = "Tất cả";

            ThemPhongCommand = new RelayCommand(o => MoFormThem());
            SuaPhongCommand = new RelayCommand(o => MoFormSua(), o => PhongDangChon != null);
            XoaPhongCommand = new RelayCommand(o => XoaPhong(), o => PhongDangChon != null);
            LuuPhongCommand = new RelayCommand(o => LuuPhong());
            HuyCommand = new RelayCommand(o => DongForm());
            DoiTrangThaiCommand = new RelayCommand(o => DoiTrangThai(o?.ToString()), o => PhongDangChon != null);

            TaiDuLieuTuDatabase();
        }

        private void TaiDuLieuTuDatabase()
        {
            try
            {
                using (var db = new NhaTroDbContext())
                {
                    var List = db.PhongTros.ToList();
                    _danhSachPhongGoc = new ObservableCollection<PhongTro>(List);
                    TimKiem();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu phòng trọ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TimKiem()
        {
            if (_danhSachPhongGoc == null) return;
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
            // Clone để tránh thay đổi trực tiếp trên UI trước khi lưu
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

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    if (IsEditMode)
                    {
                        var existing = db.PhongTros.Find(FormPhong.PhongID);
                        if (existing != null)
                        {
                            existing.TenPhong = FormPhong.TenPhong;
                            existing.LoaiPhong = FormPhong.LoaiPhong;
                            existing.Tang = FormPhong.Tang;
                            existing.DienTich = FormPhong.DienTich;
                            existing.GiaThue = FormPhong.GiaThue;
                            existing.GiaDien = FormPhong.GiaDien;
                            existing.GiaNuoc = FormPhong.GiaNuoc;
                            existing.GiaInternet = FormPhong.GiaInternet;
                            existing.GiaRac = FormPhong.GiaRac;
                            existing.GiaXeMay = FormPhong.GiaXeMay;
                            existing.TrangThai = FormPhong.TrangThai;
                            existing.TrangThaiNoiThat = FormPhong.TrangThaiNoiThat;
                            existing.MoTa = FormPhong.MoTa;
                            existing.CoWifi = FormPhong.CoWifi;
                            existing.CoMayGiat = FormPhong.CoMayGiat;
                            existing.CoBep = FormPhong.CoBep;
                            existing.CoGiuXeMay = FormPhong.CoGiuXeMay;
                            existing.CoDieuHoa = FormPhong.CoDieuHoa;
                            existing.CoNongLanh = FormPhong.CoNongLanh;
                            existing.CoTuLanh = FormPhong.CoTuLanh;
                            existing.CoTivi = FormPhong.CoTivi;
                        }
                    }
                    else
                    {
                        db.PhongTros.Add(FormPhong);
                    }
                    db.SaveChanges();
                }

                DongForm();
                TaiDuLieuTuDatabase();
                CapNhatThongKe();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu phòng trọ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                try
                {
                    using (var db = new NhaTroDbContext())
                    {
                        var existing = db.PhongTros.Find(PhongDangChon.PhongID);
                        if (existing != null)
                        {
                            db.PhongTros.Remove(existing);
                            db.SaveChanges();
                        }
                    }
                    TaiDuLieuTuDatabase();
                    CapNhatThongKe();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa phòng trọ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    var existing = db.PhongTros.Find(PhongDangChon.PhongID);
                    if (existing != null)
                    {
                        existing.TrangThai = newStatus;
                        db.SaveChanges();
                    }
                }
                TaiDuLieuTuDatabase();
                CapNhatThongKe();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đổi trạng thái: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DongForm() => IsFormVisible = false;

        private void CapNhatThongKe()
        {
            OnPropertyChanged(nameof(SoPhongTong));
            OnPropertyChanged(nameof(SoPhongTrong));
            OnPropertyChanged(nameof(SoPhongDangThue));
            OnPropertyChanged(nameof(SoPhongSuaChua));
        }
    }
}
