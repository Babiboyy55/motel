using baitaplon.ViewModels;
using Quanlynhatro.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.User
{
    /// <summary>
    /// TimKiemPhongViewModel - Xử lý logic tìm kiếm và xem chi tiết phòng
    /// </summary>
    public class TimKiemPhongViewModel : BaseViewModel
    {
        private ObservableCollection<PhongTro> _danhSachPhong;
        private PhongTro _phongDangChon;
        private string _tuKhoa;
        private decimal _giaMin;
        private decimal _giaMax;

        public ObservableCollection<PhongTro> DanhSachPhong
        {
            get => _danhSachPhong;
            set => SetProperty(ref _danhSachPhong, value);
        }

        public PhongTro PhongDangChon
        {
            get => _phongDangChon;
            set => SetProperty(ref _phongDangChon, value);
        }

        public string TuKhoa
        {
            get => _tuKhoa;
            set => SetProperty(ref _tuKhoa, value);
        }

        public decimal GiaMin
        {
            get => _giaMin;
            set => SetProperty(ref _giaMin, value);
        }

        public decimal GiaMax
        {
            get => _giaMax;
            set => SetProperty(ref _giaMax, value);
        }

        public ICommand TimKiemCommand { get; }
        public ICommand XemChiTietCommand { get; }

        public TimKiemPhongViewModel()
        {
            DanhSachPhong = new ObservableCollection<PhongTro>();
            TimKiemCommand = new RelayCommand(o => TimKiem());
            XemChiTietCommand = new RelayCommand(o => XemChiTiet(), o => PhongDangChon != null);

            // TODO: Load dữ liệu từ database
            LoadDuLieu();
        }

        private void LoadDuLieu()
        {
            // TODO: Implement load dữ liệu từ database
        }

        private void TimKiem()
        {
            // TODO: Implement logic tìm kiếm phòng theo từ khóa và giá
        }

        private void XemChiTiet()
        {
            // TODO: Implement xem chi tiết phòng được chọn
        }
    }
}
