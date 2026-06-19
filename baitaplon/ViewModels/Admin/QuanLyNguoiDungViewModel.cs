using baitaplon.Models;
using Quanlynhatro.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace baitaplon.ViewModels.Admin
{
    public class QuanLyNguoiDungViewModel : BaseViewModel
    {
        private NhaTroDbContext _context;

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

        public QuanLyNguoiDungViewModel()
        {
            // Chặn lỗi XAML Designer không gọi DB
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _context = new NhaTroDbContext();
            TaiDuLieu();
        }

        public void TaiDuLieu()
        {
            // Lấy danh sách tài khoản
            DanhSachNguoiDung = new ObservableCollection<NguoiDung>(_context.NguoiDungs.ToList());

            // Lấy 50 hành động gần nhất, sắp xếp từ mới nhất đến cũ nhất
            DanhSachNhatKy = new ObservableCollection<NhatKyHoatDong>(
                _context.NhatKyHoatDongs.OrderByDescending(n => n.ThoiGian).Take(50).ToList()
            );
        }
    }
}