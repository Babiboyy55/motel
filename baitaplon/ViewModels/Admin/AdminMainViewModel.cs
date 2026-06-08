using baitaplon.ViewModels;
using baitaplon.ViewModels.Admin;
using Quanlynhatro.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.Admin
{
    /// <summary>
    /// AdminMainViewModel - Điều khiển menu chính của Admin
    /// </summary>
    public class AdminMainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand ShowQuanLyPhongCommand { get; }
        public ICommand ShowQuanLyKhachCommand { get; }

        public AdminMainViewModel()
        {
            ShowQuanLyPhongCommand = new RelayCommand(o => ShowQuanLyPhong());
            ShowQuanLyKhachCommand = new RelayCommand(o => ShowQuanLyKhach());

            // Mặc định hiển thị màn hình quản lý phòng
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
    }
}
