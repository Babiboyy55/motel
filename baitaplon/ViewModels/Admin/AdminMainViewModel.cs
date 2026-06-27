using baitaplon.Models;
using Quanlynhatro.Models;
using Quanlynhatro.Services;
using Quanlynhatro.ViewModels;
using System;
using System.Linq;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.Admin
{
    /// <summary>
    /// AdminMainViewModel - Điều khiển navigation chính của Admin và Phân Quyền (RBAC)
    /// </summary>
    public class AdminMainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        private int _selectedMenuIndex;
        private NguoiDung _currentUser;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public int SelectedMenuIndex
        {
            get => _selectedMenuIndex;
            set => SetProperty(ref _selectedMenuIndex, value);
        }

        public NguoiDung CurrentUser
        {
            get => _currentUser;
            set
            {
                SetProperty(ref _currentUser, value);
                OnPropertyChanged(nameof(UserRoleDisplay));
                OnPropertyChanged(nameof(UserNameDisplay));
                
                // Cập nhật quyền hiển thị menu
                OnPropertyChanged(nameof(IsDashboardVisible));
                OnPropertyChanged(nameof(IsPhongVisible));
                OnPropertyChanged(nameof(IsKhachVisible));
                OnPropertyChanged(nameof(IsHopDongVisible));
                OnPropertyChanged(nameof(IsHoaDonVisible));
                OnPropertyChanged(nameof(IsNguoiDungVisible));
            }
        }

        public string UserNameDisplay => CurrentUser?.TenNguoiDung ?? "Người dùng";
        
        public string UserRoleDisplay
        {
            get
            {
                if (CurrentUser == null) return "Chưa đăng nhập";
                return CurrentUser.VaiTro switch
                {
                    "Admin" => "Chủ cơ sở (Admin)",
                    "ThuNgan" => "Thu ngân",
                    "QuanLy" => "Quản lý vận hành",
                    _ => CurrentUser.VaiTro
                };
            }
        }

        // --- PHÂN QUYỀN HIỂN THỊ MENU (RBAC) ---
        public bool IsDashboardVisible => CurrentUser?.VaiTro == "Admin";
        public bool IsPhongVisible => CurrentUser?.VaiTro == "Admin" || CurrentUser?.VaiTro == "QuanLy";
        public bool IsKhachVisible => CurrentUser?.VaiTro == "Admin" || CurrentUser?.VaiTro == "QuanLy";
        public bool IsHopDongVisible => CurrentUser?.VaiTro == "Admin" || CurrentUser?.VaiTro == "QuanLy";
        public bool IsHoaDonVisible => CurrentUser?.VaiTro == "Admin" || CurrentUser?.VaiTro == "ThuNgan";
        public bool IsNguoiDungVisible => CurrentUser?.VaiTro == "Admin";

        // Thống kê nhanh dưới Sidebar (Lấy từ Database thực tế)
        public int SoPhongTong
        {
            get
            {
                try
                {
                    using (var db = new NhaTroDbContext())
                        return db.PhongTros.Count();
                }
                catch { return 0; }
            }
        }

        public int SoPhongTrong
        {
            get
            {
                try
                {
                    using (var db = new NhaTroDbContext())
                        return db.PhongTros.Count(p => p.TrangThai == TrangThaiPhong.Trong);
                }
                catch { return 0; }
            }
        }

        public int SoKhachDangO
        {
            get
            {
                try
                {
                    using (var db = new NhaTroDbContext())
                        return db.KhachThues.Count(k => k.DangO);
                }
                catch { return 0; }
            }
        }

        public int SoHoaDonChuaTT
        {
            get
            {
                try
                {
                    using (var db = new NhaTroDbContext())
                        return db.HoaDons.Count(h => h.TrangThai != "Đã thanh toán");
                }
                catch { return 0; }
            }
        }

        // --- COMMANDS ---
        public ICommand ShowDashboardCommand { get; }
        public ICommand ShowQuanLyPhongCommand { get; }
        public ICommand ShowQuanLyKhachCommand { get; }
        public ICommand ShowQuanLyHopDongCommand { get; }
        public ICommand ShowQuanLyHoaDonCommand { get; }
        public ICommand ShowQuanLyNguoiDungCommand { get; }

        public AdminMainViewModel() : this(null)
        {
        }

        public AdminMainViewModel(NguoiDung user)
        {
            CurrentUser = user;

            ShowDashboardCommand = new RelayCommand(o => { ShowDashboard(); SelectedMenuIndex = 0; }, o => IsDashboardVisible);
            ShowQuanLyPhongCommand = new RelayCommand(o => { ShowQuanLyPhong(); SelectedMenuIndex = 1; }, o => IsPhongVisible);
            ShowQuanLyKhachCommand = new RelayCommand(o => { ShowQuanLyKhach(); SelectedMenuIndex = 2; }, o => IsKhachVisible);
            ShowQuanLyHopDongCommand = new RelayCommand(o => { ShowQuanLyHopDong(); SelectedMenuIndex = 3; }, o => IsHopDongVisible);
            ShowQuanLyHoaDonCommand = new RelayCommand(o => { ShowQuanLyHoaDon(); SelectedMenuIndex = 4; }, o => IsHoaDonVisible);
            ShowQuanLyNguoiDungCommand = new RelayCommand(o => { ShowQuanLyNguoiDung(); SelectedMenuIndex = 5; }, o => IsNguoiDungVisible);

            // Chọn view mặc định dựa trên vai trò
            if (IsDashboardVisible)
            {
                ShowDashboard();
                SelectedMenuIndex = 0;
            }
            else if (IsPhongVisible)
            {
                ShowQuanLyPhong();
                SelectedMenuIndex = 1;
            }
            else if (IsHoaDonVisible)
            {
                ShowQuanLyHoaDon();
                SelectedMenuIndex = 4;
            }
        }

        // --- HÀM CHUYỂN VIEW ---
        private void ShowDashboard()
        {
            CurrentViewModel = new baitaplon.ViewModels.Admin.DashboardViewModel();
        }

        private void ShowQuanLyPhong()
        {
            CurrentViewModel = new QuanLyPhongViewModel();
        }

        private void ShowQuanLyKhach()
        {
            CurrentViewModel = new QuanLyKhachViewModel();
        }

        private void ShowQuanLyHopDong()
        {
            CurrentViewModel = new baitaplon.ViewModels.Admin.QuanLyHopDongViewModel();
        }

        private void ShowQuanLyHoaDon()
        {
            CurrentViewModel = new QuanLyHoaDonViewModel();
        }

        private void ShowQuanLyNguoiDung()
        {
            CurrentViewModel = new baitaplon.ViewModels.Admin.QuanLyNguoiDungViewModel(CurrentUser);
        }

        // Hàm làm mới các chỉ số thống kê
        public void LamMoiThongKe()
        {
            OnPropertyChanged(nameof(SoPhongTong));
            OnPropertyChanged(nameof(SoPhongTrong));
            OnPropertyChanged(nameof(SoKhachDangO));
            OnPropertyChanged(nameof(SoHoaDonChuaTT));
        }
    }
}