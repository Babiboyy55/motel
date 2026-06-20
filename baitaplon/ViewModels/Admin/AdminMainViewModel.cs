<<<<<<< HEAD
using Quanlynhatro.ViewModels;
using Quanlynhatro.Services;
=======
﻿using baitaplon.ViewModels;
using Quanlynhatro.ViewModels;
>>>>>>> 27f0392155f94e0205868ea7fb00a61493c57513
using System.Windows.Input;

namespace baitaplon.ViewModels.Admin
{
<<<<<<< HEAD
    /// <summary>
    /// AdminMainViewModel - Điều khiển navigation chính của Admin
    /// </summary>
    public class AdminMainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        private int _selectedMenuIndex;

        public BaseViewModel CurrentViewModel
=======
    public class AdminMainViewModel : BaseViewModel
    {
        // Thuộc tính lưu trữ View hiện tại đang được hiển thị
        private BaseViewModel _currentView;
        public BaseViewModel CurrentView
>>>>>>> 27f0392155f94e0205868ea7fb00a61493c57513
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

<<<<<<< HEAD
        public int SelectedMenuIndex
        {
            get => _selectedMenuIndex;
            set => SetProperty(ref _selectedMenuIndex, value);
        }

        // Thống kê Dashboard
        public int SoPhongTong => DataService.DanhSachPhong.Count;
        public int SoPhongTrong => DataService.SoPhongTrong();
        public int SoKhachDangO => DataService.DanhSachKhach.Count;
        public int SoHoaDonChuaTT => DataService.DanhSachHoaDon.Count;

        public ICommand ShowQuanLyPhongCommand { get; }
        public ICommand ShowQuanLyKhachCommand { get; }
        public ICommand ShowQuanLyHoaDonCommand { get; }

        public AdminMainViewModel()
        {
            ShowQuanLyPhongCommand = new RelayCommand(o => { ShowQuanLyPhong(); SelectedMenuIndex = 0; });
            ShowQuanLyKhachCommand = new RelayCommand(o => { ShowQuanLyKhach(); SelectedMenuIndex = 1; });
            ShowQuanLyHoaDonCommand = new RelayCommand(o => { ShowQuanLyHoaDon(); SelectedMenuIndex = 2; });

            // Mặc định hiển thị Quản lý Phòng
            ShowQuanLyPhong();
        }

        private void ShowQuanLyPhong()
        {
            CurrentViewModel = new QuanLyPhongViewModel();
        }

        private void ShowQuanLyKhach()
        {
            CurrentViewModel = new QuanLyKhachViewModel();
=======
        // Các lệnh (Command) cho nút bấm Sidebar
        public ICommand HienThiDashboardCommand { get; set; }
        public ICommand HienThiHopDongCommand { get; set; }
        public ICommand HienThiNguoiDungCommand { get; set; }

        public AdminMainViewModel()
        {
            // Mặc định khi vừa mở app lên sẽ hiển thị Dashboard
            CurrentView = new DashboardViewModel();

            // Khởi tạo các sự kiện đổi trang (Đã gỡ bỏ tham số p=>true để tránh lỗi RelayCommand)
            HienThiDashboardCommand = new RelayCommand(p => CurrentView = new DashboardViewModel());
            HienThiHopDongCommand = new RelayCommand(p => CurrentView = new QuanLyHopDongViewModel());
            HienThiNguoiDungCommand = new RelayCommand(p => CurrentView = new QuanLyNguoiDungViewModel());
>>>>>>> 27f0392155f94e0205868ea7fb00a61493c57513
        }

        private void ShowQuanLyHoaDon()
        {
            CurrentViewModel = new QuanLyHoaDonViewModel();
        }
    }
}