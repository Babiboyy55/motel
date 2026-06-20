using Quanlynhatro.ViewModels;
using Quanlynhatro.Services;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.Admin
{
    /// <summary>
    /// AdminMainViewModel - Điều khiển navigation chính của Admin
    /// </summary>
    public class AdminMainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        private int _selectedMenuIndex;

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
        }

        private void ShowQuanLyHoaDon()
        {
            CurrentViewModel = new QuanLyHoaDonViewModel();
        }
    }
}