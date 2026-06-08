using baitaplon.ViewModels;
using Quanlynhatro.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.Admin
{
    /// <summary>
    /// QuanLyKhachViewModel - Xử lý logic Duyệt khách, Quản lý hợp đồng
    /// </summary>
    public class QuanLyKhachViewModel : BaseViewModel
    {
        private ObservableCollection<KhachThue> _danhSachKhach;
        private ObservableCollection<HopDong> _danhSachHopDong;
        private KhachThue _khachDangChon;
        private HopDong _hopDongDangChon;

        public ObservableCollection<KhachThue> DanhSachKhach
        {
            get => _danhSachKhach;
            set => SetProperty(ref _danhSachKhach, value);
        }

        public ObservableCollection<HopDong> DanhSachHopDong
        {
            get => _danhSachHopDong;
            set => SetProperty(ref _danhSachHopDong, value);
        }

        public KhachThue KhachDangChon
        {
            get => _khachDangChon;
            set => SetProperty(ref _khachDangChon, value);
        }

        public HopDong HopDongDangChon
        {
            get => _hopDongDangChon;
            set => SetProperty(ref _hopDongDangChon, value);
        }

        public ICommand DuyetKhachCommand { get; }
        public ICommand CapNhatHopDongCommand { get; }

        public QuanLyKhachViewModel()
        {
            DanhSachKhach = new ObservableCollection<KhachThue>();
            DanhSachHopDong = new ObservableCollection<HopDong>();
            DuyetKhachCommand = new RelayCommand(o => DuyetKhach(), o => KhachDangChon != null);
            CapNhatHopDongCommand = new RelayCommand(o => CapNhatHopDong(), o => HopDongDangChon != null);

            // TODO: Load dữ liệu từ database
            LoadDuLieu();
        }

        private void LoadDuLieu()
        {
            // TODO: Implement load dữ liệu từ database
        }

        private void DuyetKhach()
        {
            // TODO: Implement duyệt khách
        }

        private void CapNhatHopDong()
        {
            // TODO: Implement cập nhật hợp đồng
        }
    }
}
