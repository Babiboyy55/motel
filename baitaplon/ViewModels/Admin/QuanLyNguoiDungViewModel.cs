using baitaplon.Models;
using Quanlynhatro.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace baitaplon.ViewModels.Admin
{
    public class QuanLyNguoiDungViewModel : BaseViewModel
    {
        private NhaTroDbContext _context;
        private NguoiDung _nguoiThucHien;

        // 1. Danh sách Tài khoản
        private ObservableCollection<NguoiDung> _danhSachNguoiDung;
        public ObservableCollection<NguoiDung> DanhSachNguoiDung
        {
            get => _danhSachNguoiDung;
            set { _danhSachNguoiDung = value; OnPropertyChanged(); }
        }

        // 2. Danh sách Nhật ký hoạt động (Audit Log)
        private ObservableCollection<NhatKyHoatDong> _danhSachNhatKy;
        public ObservableCollection<NhatKyHoatDong> DanhSachNhatKy
        {
            get => _danhSachNhatKy;
            set { _danhSachNhatKy = value; OnPropertyChanged(); }
        }

        // 3. Tài khoản đang chọn trong DataGrid
        private NguoiDung _nguoiDungDangChon;
        public NguoiDung NguoiDungDangChon
        {
            get => _nguoiDungDangChon;
            set 
            { 
                _nguoiDungDangChon = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(NutKhoaText));
                OnPropertyChanged(nameof(NutKhoaMau));
            }
        }

        // 4. Trạng thái hiển thị Form cấp tài khoản mới
        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set { _isFormVisible = value; OnPropertyChanged(); }
        }

        // 5. Các Input fields của Form
        private string _tenDangNhapInput;
        public string TenDangNhapInput
        {
            get => _tenDangNhapInput;
            set { _tenDangNhapInput = value; OnPropertyChanged(); }
        }

        private string _tenNguoiDungInput;
        public string TenNguoiDungInput
        {
            get => _tenNguoiDungInput;
            set { _tenNguoiDungInput = value; OnPropertyChanged(); }
        }

        private string _matKhauInput;
        public string MatKhauInput
        {
            get => _matKhauInput;
            set { _matKhauInput = value; OnPropertyChanged(); }
        }

        private string _vaiTroInput;
        public string VaiTroInput
        {
            get => _vaiTroInput;
            set { _vaiTroInput = value; OnPropertyChanged(); }
        }

        private string _soDienThoaiInput;
        public string SoDienThoaiInput
        {
            get => _soDienThoaiInput;
            set { _soDienThoaiInput = value; OnPropertyChanged(); }
        }

        public List<string> DanhSachVaiTro { get; } = new List<string> { "Admin", "QuanLy", "ThuNgan" };

        // 6. Dynamic Bindings cho nút Khóa tài khoản
        public string NutKhoaText => NguoiDungDangChon == null ? "Khóa Tài Khoản" : (NguoiDungDangChon.TrangThai ? "Khóa Tài Khoản" : "Mở Khóa Tài Khoản");
        public string NutKhoaMau => NguoiDungDangChon == null ? "#E74C3C" : (NguoiDungDangChon.TrangThai ? "#E74C3C" : "#2E86C1");

        // 7. Commands
        public ICommand MoFormThemCommand { get; }
        public ICommand LuuTaiKhoanCommand { get; }
        public ICommand HuyCommand { get; }
        public ICommand KhoaMoKhoaCommand { get; }

        public QuanLyNguoiDungViewModel() : this(null)
        {
        }

        public QuanLyNguoiDungViewModel(NguoiDung nguoiThucHien)
        {
            _nguoiThucHien = nguoiThucHien;

            // Chặn lỗi XAML Designer không gọi DB
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _context = new NhaTroDbContext();

            // Khởi tạo Commands
            MoFormThemCommand = new RelayCommand(o => MoFormThem());
            LuuTaiKhoanCommand = new RelayCommand(o => LuuTaiKhoan());
            HuyCommand = new RelayCommand(o => Huy());
            KhoaMoKhoaCommand = new RelayCommand(o => KhoaMoKhoa(), o => NguoiDungDangChon != null);

            TaiDuLieu();
        }

        public void TaiDuLieu()
        {
            // Lấy danh sách tài khoản
            DanhSachNguoiDung = new ObservableCollection<NguoiDung>(_context.NguoiDungs.ToList());

            // Lấy 50 hành động gần nhất, sắp xếp từ mới nhất đến cũ nhất
            DanhSachNhatKy = new ObservableCollection<NhatKyHoatDong>(
                _context.NhatKyHoatDongs
                        .Include(n => n.NguoiDung)
                        .OrderByDescending(n => n.ThoiGian)
                        .Take(50)
                        .ToList()
            );
        }

        private void MoFormThem()
        {
            TenDangNhapInput = "";
            TenNguoiDungInput = "";
            MatKhauInput = "";
            SoDienThoaiInput = "";
            VaiTroInput = "ThuNgan";
            IsFormVisible = true;
        }

        private void Huy()
        {
            IsFormVisible = false;
        }

        private void LuuTaiKhoan()
        {
            if (string.IsNullOrWhiteSpace(TenDangNhapInput) || 
                string.IsNullOrWhiteSpace(TenNguoiDungInput) || 
                string.IsNullOrWhiteSpace(MatKhauInput) ||
                string.IsNullOrWhiteSpace(SoDienThoaiInput))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin tài khoản bao gồm số điện thoại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string username = TenDangNhapInput.Trim();

            // Kiểm tra trùng tên đăng nhập
            if (_context.NguoiDungs.Any(u => u.TenDangNhap == username))
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại trên hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newNguoiDung = new NguoiDung
                {
                    TenDangNhap = username,
                    TenNguoiDung = TenNguoiDungInput.Trim(),
                    MatKhau = Services.SecurityHelper.HashPassword(MatKhauInput.Trim()),
                    SoDienThoai = SoDienThoaiInput.Trim(),
                    VaiTro = VaiTroInput,
                    TrangThai = true
                };

                _context.NguoiDungs.Add(newNguoiDung);

                // Ghi nhật ký
                var log = new NhatKyHoatDong
                {
                    NguoiDungId = _nguoiThucHien?.Id ?? 1,
                    HanhDong = "Cấp tài khoản mới",
                    ThoiGian = DateTime.Now,
                    ChiTiet = $"Cấp tài khoản {username} với vai trò {VaiTroInput}"
                };
                _context.NhatKyHoatDongs.Add(log);

                _context.SaveChanges();
                IsFormVisible = false;
                TaiDuLieu();

                MessageBox.Show("Cấp tài khoản mới thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cấp tài khoản: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void KhoaMoKhoa()
        {
            if (NguoiDungDangChon == null) return;

            if (_nguoiThucHien != null && NguoiDungDangChon.Id == _nguoiThucHien.Id)
            {
                MessageBox.Show("Bạn không thể tự khóa tài khoản của chính mình!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NguoiDungDangChon.TenDangNhap == "admin")
            {
                MessageBox.Show("Không thể khóa tài khoản quản trị hệ thống (admin)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string actionName = NguoiDungDangChon.TrangThai ? "khóa" : "mở khóa";
            var result = MessageBox.Show($"Bạn có chắc chắn muốn {actionName} tài khoản '{NguoiDungDangChon.TenDangNhap}' không?", 
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var userInDb = _context.NguoiDungs.Find(NguoiDungDangChon.Id);
                if (userInDb != null)
                {
                    userInDb.TrangThai = !userInDb.TrangThai;

                    // Ghi nhật ký
                    var log = new NhatKyHoatDong
                    {
                        NguoiDungId = _nguoiThucHien?.Id ?? 1,
                        HanhDong = userInDb.TrangThai ? "Mở khóa tài khoản" : "Khóa tài khoản",
                        ThoiGian = DateTime.Now,
                        ChiTiet = $"{(userInDb.TrangThai ? "Mở khóa" : "Khóa")} tài khoản {userInDb.TenDangNhap}"
                    };
                    _context.NhatKyHoatDongs.Add(log);

                    _context.SaveChanges();
                    TaiDuLieu();

                    MessageBox.Show($"Đã {actionName} tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thực hiện thao tác: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}