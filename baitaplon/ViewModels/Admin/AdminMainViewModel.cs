using baitaplon.ViewModels;
using Quanlynhatro.ViewModels;
using System.Windows.Input;

namespace baitaplon.ViewModels.Admin
{
    public class AdminMainViewModel : BaseViewModel
    {
        // Thuộc tính lưu trữ View hiện tại đang được hiển thị
        private BaseViewModel _currentView;
        public BaseViewModel CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

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
        }
    }
}