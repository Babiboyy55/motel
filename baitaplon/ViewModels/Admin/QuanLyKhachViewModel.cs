using Quanlynhatro.ViewModels;
using Quanlynhatro.Models;
using baitaplon.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Quanlynhatro.ViewModels.Admin
{
    /// <summary>
    /// QuanLyKhachViewModel - Quản lý khách thuê và hợp đồng kết nối SQL Server qua Entity Framework
    /// </summary>
    public class QuanLyKhachViewModel : BaseViewModel
    {
        private ObservableCollection<KhachThue> _danhSachKhach;
        private ObservableCollection<HopDong> _danhSachHopDong;
        private KhachThue _khachDangChon;
        private HopDong _hopDongDangChon;
        private string _tuKhoaTimKiem;
        private int _tabIndex; // 0 = Khách thuê, 1 = Hợp đồng

        // Form khách
        private KhachThue _formKhach;
        private bool _isFormKhachVisible;
        private bool _isEditKhach;
        private string _formKhachTitle;

        // Form hợp đồng
        private HopDong _formHopDong;
        private bool _isFormHopDongVisible;
        private bool _isEditHopDong;
        private string _formHopDongTitle;

        // Caching list gốc để tìm kiếm
        private ObservableCollection<KhachThue> _danhSachKhachGoc;
        private ObservableCollection<HopDong> _danhSachHopDongGoc;

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
            set { SetProperty(ref _khachDangChon, value); OnPropertyChanged(nameof(CoKhachDangChon)); }
        }

        public HopDong HopDongDangChon
        {
            get => _hopDongDangChon;
            set { SetProperty(ref _hopDongDangChon, value); OnPropertyChanged(nameof(CoHopDongDangChon)); }
        }

        public string TuKhoaTimKiem
        {
            get => _tuKhoaTimKiem;
            set { SetProperty(ref _tuKhoaTimKiem, value); TimKiem(); }
        }

        public int TabIndex
        {
            get => _tabIndex;
            set => SetProperty(ref _tabIndex, value);
        }

        // Form Khách
        public KhachThue FormKhach
        {
            get => _formKhach;
            set => SetProperty(ref _formKhach, value);
        }

        public bool IsFormKhachVisible
        {
            get => _isFormKhachVisible;
            set => SetProperty(ref _isFormKhachVisible, value);
        }

        public string FormKhachTitle
        {
            get => _formKhachTitle;
            set => SetProperty(ref _formKhachTitle, value);
        }

        // Form HopDong
        public HopDong FormHopDong
        {
            get => _formHopDong;
            set => SetProperty(ref _formHopDong, value);
        }

        public bool IsFormHopDongVisible
        {
            get => _isFormHopDongVisible;
            set => SetProperty(ref _isFormHopDongVisible, value);
        }

        public string FormHopDongTitle
        {
            get => _formHopDongTitle;
            set => SetProperty(ref _formHopDongTitle, value);
        }

        public bool CoKhachDangChon => KhachDangChon != null;
        public bool CoHopDongDangChon => HopDongDangChon != null;

        // Thống kê (Truy vấn DB trực tiếp)
        public int TongSoKhach
        {
            get
            {
                using (var db = new NhaTroDbContext())
                    return db.KhachThues.Count(k => k.DangO);
            }
        }

        public int SoHopDongHoatDong
        {
            get
            {
                using (var db = new NhaTroDbContext())
                    return db.HopDongs.Count(h => h.TrangThai == "Hoạt động");
            }
        }

        public int SoHopDongSapHetHan
        {
            get
            {
                using (var db = new NhaTroDbContext())
                {
                    DateTime threshold = DateTime.Today.AddDays(30);
                    return db.HopDongs.Count(h => h.TrangThai == "Hoạt động" && h.NgayKetThuc <= threshold);
                }
            }
        }

        // Danh sách phòng cho ComboBox
        private ObservableCollection<PhongTro> _danhSachPhong;
        public ObservableCollection<PhongTro> DanhSachPhong
        {
            get => _danhSachPhong;
            set => SetProperty(ref _danhSachPhong, value);
        }

        public string[] DanhSachGioiTinh => new[] { "Nam", "Nữ", "Khác" };
        public string[] DanhSachQuanHe => new[] { "Bố", "Mẹ", "Anh", "Chị", "Em", "Vợ", "Chồng", "Bạn bè", "Khác" };

        // Commands - Khách
        public ICommand ThemKhachCommand { get; }
        public ICommand SuaKhachCommand { get; }
        public ICommand XoaKhachCommand { get; }
        public ICommand LuuKhachCommand { get; }
        public ICommand HuyKhachCommand { get; }

        // Commands - HopDong
        public ICommand ThemHopDongCommand { get; }
        public ICommand SuaHopDongCommand { get; }
        public ICommand ChamDutHopDongCommand { get; }
        public ICommand LuuHopDongCommand { get; }
        public ICommand HuyHopDongCommand { get; }

        public QuanLyKhachViewModel()
        {
            ThemKhachCommand = new RelayCommand(o => MoFormThemKhach());
            SuaKhachCommand = new RelayCommand(o => MoFormSuaKhach(), o => KhachDangChon != null);
            XoaKhachCommand = new RelayCommand(o => XoaKhach(), o => KhachDangChon != null);
            LuuKhachCommand = new RelayCommand(o => LuuKhach());
            HuyKhachCommand = new RelayCommand(o => IsFormKhachVisible = false);

            ThemHopDongCommand = new RelayCommand(o => MoFormThemHopDong());
            SuaHopDongCommand = new RelayCommand(o => MoFormSuaHopDong(), o => HopDongDangChon != null);
            ChamDutHopDongCommand = new RelayCommand(o => ChamDutHopDong(), o => HopDongDangChon?.TrangThai == "Hoạt động");
            LuuHopDongCommand = new RelayCommand(o => LuuHopDong());
            HuyHopDongCommand = new RelayCommand(o => IsFormHopDongVisible = false);

            TaiDuLieuTuDatabase();
        }

        private void TaiDuLieuTuDatabase()
        {
            try
            {
                using (var db = new NhaTroDbContext())
                {
                    _danhSachKhachGoc = new ObservableCollection<KhachThue>(db.KhachThues.ToList());
                    _danhSachHopDongGoc = new ObservableCollection<HopDong>(db.HopDongs.ToList());
                    DanhSachPhong = new ObservableCollection<PhongTro>(db.PhongTros.ToList());
                    
                    TimKiem();
                    CapNhatThongKe();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu khách thuê/hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TimKiem()
        {
            if (_danhSachKhachGoc == null || _danhSachHopDongGoc == null) return;

            if (string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                DanhSachKhach = new ObservableCollection<KhachThue>(_danhSachKhachGoc);
                DanhSachHopDong = new ObservableCollection<HopDong>(_danhSachHopDongGoc);
            }
            else
            {
                string kw = TuKhoaTimKiem.ToLower();
                
                DanhSachKhach = new ObservableCollection<KhachThue>(
                    _danhSachKhachGoc.Where(k =>
                        (k.HoTen?.ToLower().Contains(kw) ?? false) ||
                        (k.CCCD?.Contains(kw) ?? false) ||
                        (k.SoDienThoai?.Contains(kw) ?? false)));

                DanhSachHopDong = new ObservableCollection<HopDong>(
                    _danhSachHopDongGoc.Where(h =>
                        (h.TenKhach?.ToLower().Contains(kw) ?? false) ||
                        (h.TenPhong?.Contains(kw) ?? false)));
            }
        }

        // ---- Khách ----
        private void MoFormThemKhach()
        {
            FormKhach = new KhachThue { NgayVaoO = DateTime.Today, DangO = true };
            _isEditKhach = false;
            FormKhachTitle = "THÊM KHÁCH THUÊ MỚI";
            IsFormKhachVisible = true;
        }

        private void MoFormSuaKhach()
        {
            if (KhachDangChon == null) return;
            FormKhach = new KhachThue
            {
                KhachID = KhachDangChon.KhachID,
                HoTen = KhachDangChon.HoTen,
                CCCD = KhachDangChon.CCCD,
                SoDienThoai = KhachDangChon.SoDienThoai,
                Email = KhachDangChon.Email,
                NgaySinh = KhachDangChon.NgaySinh,
                GioiTinh = KhachDangChon.GioiTinh,
                QueQuan = KhachDangChon.QueQuan,
                DiaChiHienTai = KhachDangChon.DiaChiHienTai,
                NguoiLienHeKhan = KhachDangChon.NguoiLienHeKhan,
                QuanHeNguoiLienHe = KhachDangChon.QuanHeNguoiLienHe,
                SoDTNguoiLienHe = KhachDangChon.SoDTNguoiLienHe,
                PhongID = KhachDangChon.PhongID,
                NgayVaoO = KhachDangChon.NgayVaoO,
                DangO = KhachDangChon.DangO,
                GhiChu = KhachDangChon.GhiChu
            };
            _isEditKhach = true;
            FormKhachTitle = $"SỬA THÔNG TIN - {KhachDangChon.HoTen}";
            IsFormKhachVisible = true;
        }

        private void LuuKhach()
        {
            if (FormKhach == null) return;
            if (string.IsNullOrWhiteSpace(FormKhach.HoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên khách!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    if (_isEditKhach)
                    {
                        var existing = db.KhachThues.Find(FormFormKhachId(FormKhach));
                        if (existing != null)
                        {
                            existing.HoTen = FormKhach.HoTen;
                            existing.CCCD = FormKhach.CCCD;
                            existing.SoDienThoai = FormKhach.SoDienThoai;
                            existing.Email = FormKhach.Email;
                            existing.NgaySinh = FormKhach.NgaySinh;
                            existing.GioiTinh = FormKhach.GioiTinh;
                            existing.QueQuan = FormKhach.QueQuan;
                            existing.DiaChiHienTai = FormKhach.DiaChiHienTai;
                            existing.NguoiLienHeKhan = FormKhach.NguoiLienHeKhan;
                            existing.QuanHeNguoiLienHe = FormKhach.QuanHeNguoiLienHe;
                            existing.SoDTNguoiLienHe = FormKhach.SoDTNguoiLienHe;
                            existing.PhongID = FormKhach.PhongID;
                            existing.NgayVaoO = FormKhach.NgayVaoO;
                            existing.DangO = FormKhach.DangO;
                            existing.GhiChu = FormKhach.GhiChu;
                        }
                    }
                    else
                    {
                        db.KhachThues.Add(FormKhach);
                    }
                    db.SaveChanges();
                }

                IsFormKhachVisible = false;
                TaiDuLieuTuDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu khách thuê: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int FormFormKhachId(KhachThue form) => form.KhachID;

        private void XoaKhach()
        {
            if (KhachDangChon == null) return;
            if (KhachDangChon.DangO)
            {
                MessageBox.Show("Không thể xóa khách đang ở! Hãy chấm dứt hợp đồng trước.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var res = MessageBox.Show($"Xóa khách '{KhachDangChon.HoTen}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new NhaTroDbContext())
                    {
                        var existing = db.KhachThues.Find(KhachDangChon.KhachID);
                        if (existing != null)
                        {
                            db.KhachThues.Remove(existing);
                            db.SaveChanges();
                        }
                    }
                    TaiDuLieuTuDatabase();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa khách thuê: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ---- Hợp Đồng ----
        private void MoFormThemHopDong()
        {
            FormHopDong = new HopDong { NgayBatDau = DateTime.Today, NgayKetThuc = DateTime.Today.AddMonths(12), TrangThai = "Hoạt động" };
            _isEditHopDong = false;
            FormHopDongTitle = "TẠO HỢP ĐỒNG MỚI";
            IsFormHopDongVisible = true;
        }

        private void MoFormSuaHopDong()
        {
            if (HopDongDangChon == null) return;
            FormHopDong = new HopDong
            {
                HopDongID = HopDongDangChon.HopDongID,
                KhachID = HopDongDangChon.KhachID,
                PhongID = HopDongDangChon.PhongID,
                TenKhach = HopDongDangChon.TenKhach,
                TenPhong = HopDongDangChon.TenPhong,
                NgayBatDau = HopDongDangChon.NgayBatDau,
                NgayKetThuc = HopDongDangChon.NgayKetThuc,
                GiaThue = HopDongDangChon.GiaThue,
                TienCoc = HopDongDangChon.TienCoc,
                DaHoanCoc = HopDongDangChon.DaHoanCoc,
                DieuKhoan = HopDongDangChon.DieuKhoan,
                TrangThai = HopDongDangChon.TrangThai,
                GhiChu = HopDongDangChon.GhiChu
            };
            _isEditHopDong = true;
            FormHopDongTitle = $"SỬA HỢP ĐỒNG #{HopDongDangChon.HopDongID}";
            IsFormHopDongVisible = true;
        }

        private void LuuHopDong()
        {
            if (FormHopDong == null) return;
            if (FormHopDong.KhachID == 0 || FormHopDong.PhongID == 0)
            {
                MessageBox.Show("Vui lòng chọn khách và phòng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    // Lấy tên khách, tên phòng từ CSDL
                    var khach = db.KhachThues.Find(FormHopDong.KhachID);
                    var phong = db.PhongTros.Find(FormHopDong.PhongID);
                    FormHopDong.TenKhach = khach?.HoTen;
                    FormHopDong.TenPhong = phong?.TenPhong;
                    FormHopDong.GiaThue = phong?.GiaThue ?? FormHopDong.GiaThue;

                    if (_isEditHopDong)
                    {
                        var existing = db.HopDongs.Find(FormHopDong.HopDongID);
                        if (existing != null)
                        {
                            existing.KhachID = FormHopDong.KhachID;
                            existing.PhongID = FormHopDong.PhongID;
                            existing.TenKhach = FormHopDong.TenKhach;
                            existing.TenPhong = FormHopDong.TenPhong;
                            existing.NgayBatDau = FormHopDong.NgayBatDau;
                            existing.NgayKetThuc = FormHopDong.NgayKetThuc;
                            existing.GiaThue = FormHopDong.GiaThue;
                            existing.TienCoc = FormHopDong.TienCoc;
                            existing.DaHoanCoc = FormHopDong.DaHoanCoc;
                            existing.DieuKhoan = FormHopDong.DieuKhoan;
                            existing.TrangThai = FormHopDong.TrangThai;
                            existing.GhiChu = FormHopDong.GhiChu;
                        }
                    }
                    else
                    {
                        db.HopDongs.Add(FormHopDong);
                        
                        // Khi tạo hợp đồng mới -> Cập nhật trạng thái phòng thành Đang thuê
                        if (phong != null)
                        {
                            phong.TrangThai = TrangThaiPhong.DangThue;
                        }
                    }
                    
                    db.SaveChanges();
                }

                IsFormHopDongVisible = false;
                TaiDuLieuTuDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChamDutHopDong()
        {
            if (HopDongDangChon == null) return;
            var res = MessageBox.Show($"Chấm dứt hợp đồng #{HopDongDangChon.HopDongID} với phòng {HopDongDangChon.TenPhong}?\nPhòng sẽ được chuyển về trạng thái Trống.",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new NhaTroDbContext())
                    {
                        var hd = db.HopDongs.Find(HopDongDangChon.HopDongID);
                        if (hd != null)
                        {
                            hd.TrangThai = "Đã chấm dứt";
                            
                            // Cập nhật phòng tương ứng thành Trống
                            var phong = db.PhongTros.Find(hd.PhongID);
                            if (phong != null)
                            {
                                phong.TrangThai = TrangThaiPhong.Trong;
                            }
                            
                            // Cập nhật khách thuê tương ứng thành đã ra ở
                            var khach = db.KhachThues.Find(hd.KhachID);
                            if (khach != null)
                            {
                                khach.DangO = false;
                            }
                        }
                        db.SaveChanges();
                    }
                    TaiDuLieuTuDatabase();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi chấm dứt hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CapNhatThongKe()
        {
            OnPropertyChanged(nameof(TongSoKhach));
            OnPropertyChanged(nameof(SoHopDongHoatDong));
            OnPropertyChanged(nameof(SoHopDongSapHetHan));
        }
    }
}
