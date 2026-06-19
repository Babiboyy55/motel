//using baitaplon.ViewModels;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.User
{
    /// <summary>
    /// UserMainViewModel - Điều khiển màn hình chính của User (Người tìm phòng)
    /// </summary>
    public class UserMainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand ShowTimKiemPhongCommand { get; }

        public UserMainViewModel()
        {
            ShowTimKiemPhongCommand = new RelayCommand(o => ShowTimKiemPhong());

            // Mặc định hiển thị màn hình tìm kiếm phòng
            ShowTimKiemPhong();
        }

        private void ShowTimKiemPhong()
        {
            CurrentViewModel = new TimKiemPhongViewModel();
        }
    }
}
