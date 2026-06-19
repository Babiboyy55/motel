//using baitaplon.ViewModels;
using Quanlynhatro.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.Admin
{
    /// <summary>
    /// QuanLyPhongViewModel - Xử lý logic Thêm, Sửa, Xóa phòng
    /// </summary>
    public class QuanLyPhongViewModel : BaseViewModel
    {
        private ObservableCollection<PhongTro> _danhSachPhong;
        private PhongTro _phongDangChon;
        private string _tenPhong;
        private string _dia;
        private decimal _giaThue;
        private int _dienTich;

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

        public string TenPhong
        {
            get => _tenPhong;
            set => SetProperty(ref _tenPhong, value);
        }

        public string Dia
        {
            get => _dia;
            set => SetProperty(ref _dia, value);
        }

        public decimal GiaThue
        {
            get => _giaThue;
            set => SetProperty(ref _giaThue, value);
        }

        public int DienTich
        {
            get => _dienTich;
            set => SetProperty(ref _dienTich, value);
        }

        public ICommand ThemPhongCommand { get; }
        public ICommand SuaPhongCommand { get; }
        public ICommand XoaPhongCommand { get; }
        public ICommand DoiGiaCommand { get; }

        public QuanLyPhongViewModel()
        {
            DanhSachPhong = new ObservableCollection<PhongTro>();
            ThemPhongCommand = new RelayCommand(o => ThemPhong());
            SuaPhongCommand = new RelayCommand(o => SuaPhong(), o => PhongDangChon != null);
            XoaPhongCommand = new RelayCommand(o => XoaPhong(), o => PhongDangChon != null);
            DoiGiaCommand = new RelayCommand(o => DoiGia(), o => PhongDangChon != null);

            // TODO: Load dữ liệu từ database
            LoadDuLieu();
        }

        private void LoadDuLieu()
        {
            // TODO: Implement load dữ liệu từ database
        }

        private void ThemPhong()
        {
            // TODO: Implement thêm phòng
        }

        private void SuaPhong()
        {
            // TODO: Implement sửa phòng
        }

        private void XoaPhong()
        {
            // TODO: Implement xóa phòng
        }

        private void DoiGia()
        {
            // TODO: Implement đổi giá phòng
        }
    }
}
