using Quanlynhatro.ViewModels;
using Quanlynhatro.Models;
using baitaplon.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MiniExcelLibs;

namespace Quanlynhatro.ViewModels.Admin
{
    /// <summary>
    /// QuanLyHoaDonViewModel - Quản lý hóa đơn, công nợ, thu tiền kết nối SQL Server qua Entity Framework
    /// </summary>
    public class QuanLyHoaDonViewModel : BaseViewModel
    {
        private ObservableCollection<HoaDon> _danhSachHoaDon;
        private HoaDon _hoaDonDangChon;
        private int _thangLoc;
        private int _namLoc;
        private string _locTrangThai;

        // Form hóa đơn
        private HoaDon _formHoaDon;
        private bool _isFormHoaDonVisible;
        private bool _isEditHoaDon;
        private string _formHoaDonTitle;

        // Form thanh toán
        private decimal _soTienThanhToan;
        private string _hinhThucThanhToan;
        private bool _isFormThanhToanVisible;

        public ObservableCollection<HoaDon> DanhSachHoaDon
        {
            get => _danhSachHoaDon;
            set => SetProperty(ref _danhSachHoaDon, value);
        }

        public HoaDon HoaDonDangChon
        {
            get => _hoaDonDangChon;
            set { SetProperty(ref _hoaDonDangChon, value); OnPropertyChanged(nameof(CoHoaDonDangChon)); }
        }

        public int ThangLoc
        {
            get => _thangLoc;
            set { SetProperty(ref _thangLoc, value); TimKiem(); }
        }

        public int NamLoc
        {
            get => _namLoc;
            set { SetProperty(ref _namLoc, value); TimKiem(); }
        }

        public string LocTrangThai
        {
            get => _locTrangThai;
            set { SetProperty(ref _locTrangThai, value); TimKiem(); }
        }

        // Form HoaDon
        public HoaDon FormHoaDon
        {
            get => _formHoaDon;
            set => SetProperty(ref _formHoaDon, value);
        }

        public bool IsFormHoaDonVisible
        {
            get => _isFormHoaDonVisible;
            set => SetProperty(ref _isFormHoaDonVisible, value);
        }

        public string FormHoaDonTitle
        {
            get => _formHoaDonTitle;
            set => SetProperty(ref _formHoaDonTitle, value);
        }

        // Form Thanh Toán
        public decimal SoTienThanhToan
        {
            get => _soTienThanhToan;
            set => SetProperty(ref _soTienThanhToan, value);
        }

        public string HinhThucThanhToan
        {
            get => _hinhThucThanhToan;
            set => SetProperty(ref _hinhThucThanhToan, value);
        }

        public bool IsFormThanhToanVisible
        {
            get => _isFormThanhToanVisible;
            set => SetProperty(ref _isFormThanhToanVisible, value);
        }

        public bool CoHoaDonDangChon => HoaDonDangChon != null;

        // Thống kê thu chi (Lấy trực tiếp từ DB)
        public decimal TongThuThang
        {
            get
            {
                using (var db = new NhaTroDbContext())
                {
                    return db.HoaDons
                        .Where(h => h.Thang == ThangLoc && h.Nam == NamLoc && h.TrangThai == "Đã thanh toán")
                        .ToList()
                        .Sum(h => h.TongTien);
                }
            }
        }

        public decimal TongConNo
        {
            get
            {
                using (var db = new NhaTroDbContext())
                {
                    return db.HoaDons
                        .Where(h => h.TrangThai != "Đã thanh toán")
                        .ToList()
                        .Sum(h => h.ConNo);
                }
            }
        }

        public int SoHoaDonChuaThanhToan
        {
            get
            {
                using (var db = new NhaTroDbContext())
                {
                    return db.HoaDons.Count(h => h.TrangThai == "Chưa thanh toán");
                }
            }
        }

        public int SoHoaDonDaThanhToan
        {
            get
            {
                using (var db = new NhaTroDbContext())
                {
                    return db.HoaDons.Count(h => h.TrangThai == "Đã thanh toán" && h.Thang == ThangLoc && h.Nam == NamLoc);
                }
            }
        }

        // ComboBox items
        private ObservableCollection<PhongTro> _danhSachPhong;
        public ObservableCollection<PhongTro> DanhSachPhong
        {
            get => _danhSachPhong;
            set => SetProperty(ref _danhSachPhong, value);
        }

        public string[] DanhSachTrangThaiHD => new[] { "Tất cả", "Chưa thanh toán", "Trả một phần", "Đã thanh toán" };
        public string[] DanhSachHinhThucTT => new[] { "Tiền mặt", "Chuyển khoản", "Ví điện tử" };
        public int[] DanhSachThang => Enumerable.Range(1, 12).ToArray();
        public int[] DanhSachNam => Enumerable.Range(2020, 10).ToArray();

        // Commands
        public ICommand TaoHoaDonTuDongCommand { get; }
        public ICommand ThemHoaDonThuCongCommand { get; }
        public ICommand SuaHoaDonCommand { get; }
        public ICommand XoaHoaDonCommand { get; }
        public ICommand LuuHoaDonCommand { get; }
        public ICommand HuyHoaDonCommand { get; }
        public ICommand GhiNhanThanhToanCommand { get; }
        public ICommand MoThanhToanCommand { get; }
        public ICommand HuyThanhToanCommand { get; }
        public ICommand XuatHoaDonExcelCommand { get; }

        public QuanLyHoaDonViewModel()
        {
            ThangLoc = DateTime.Today.Month;
            NamLoc = DateTime.Today.Year;
            LocTrangThai = "Tất cả";
            HinhThucThanhToan = "Tiền mặt";

            TaoHoaDonTuDongCommand = new RelayCommand(o => TaoHoaDonTuDong());
            ThemHoaDonThuCongCommand = new RelayCommand(o => MoFormThemHoaDon());
            SuaHoaDonCommand = new RelayCommand(o => MoFormSuaHoaDon(), o => HoaDonDangChon != null);
            XoaHoaDonCommand = new RelayCommand(o => XoaHoaDon(), o => HoaDonDangChon != null);
            LuuHoaDonCommand = new RelayCommand(o => LuuHoaDon());
            HuyHoaDonCommand = new RelayCommand(o => IsFormHoaDonVisible = false);
            GhiNhanThanhToanCommand = new RelayCommand(o => GhiNhanThanhToan());
            MoThanhToanCommand = new RelayCommand(o => MoFormThanhToan(), o => HoaDonDangChon != null && HoaDonDangChon.TrangThai != "Đã thanh toán");
            HuyThanhToanCommand = new RelayCommand(o => IsFormThanhToanVisible = false);
            XuatHoaDonExcelCommand = new RelayCommand(o => XuatHoaDonExcel(), o => HoaDonDangChon != null);

            LamMoiDanhSach();
        }

        private void TimKiem()
        {
            try
            {
                using (var db = new NhaTroDbContext())
                {
                    // Lấy danh sách phòng cho ComboBox
                    DanhSachPhong = new ObservableCollection<PhongTro>(db.PhongTros.ToList());

                    var ds = db.HoaDons.AsQueryable();

                    if (ThangLoc > 0)
                        ds = ds.Where(h => h.Thang == ThangLoc && h.Nam == NamLoc);

                    if (LocTrangThai != "Tất cả" && !string.IsNullOrEmpty(LocTrangThai))
                        ds = ds.Where(h => h.TrangThai == LocTrangThai);

                    DanhSachHoaDon = new ObservableCollection<HoaDon>(ds.OrderByDescending(h => h.HoaDonID).ToList());
                    CapNhatThongKe();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TaoHoaDonTuDong()
        {
            var res = MessageBox.Show($"Tạo hóa đơn tự động cho tháng {ThangLoc}/{NamLoc}?\nHệ thống sẽ tạo hóa đơn cho tất cả phòng đang thuê chưa có hóa đơn tháng này.",
                "Xác nhận tạo hóa đơn", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res != MessageBoxResult.Yes) return;

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    var dsPhongDangThue = db.PhongTros.Where(p => p.TrangThai == TrangThaiPhong.DangThue).ToList();
                    int count = 0;

                    foreach (var phong in dsPhongDangThue)
                    {
                        // Kiểm tra đã có hóa đơn chưa
                        bool daCoHD = db.HoaDons.Any(h => h.PhongID == phong.PhongID && h.Thang == ThangLoc && h.Nam == NamLoc);
                        if (daCoHD) continue;

                        var khach = db.KhachThues.FirstOrDefault(k => k.PhongID == phong.PhongID && k.DangO);
                        
                        // Lấy chỉ số cũ
                        var hdTruoc = db.HoaDons
                            .Where(h => h.PhongID == phong.PhongID)
                            .OrderByDescending(h => h.Nam * 100 + h.Thang)
                            .FirstOrDefault();

                        var hopDong = db.HopDongs
                            .FirstOrDefault(h => h.PhongID == phong.PhongID && h.TrangThai == "Hoạt động");

                        var hd = new HoaDon
                        {
                            PhongID = phong.PhongID,
                            TenPhong = phong.TenPhong,
                            TenKhach = khach?.HoTen ?? "(Chưa rõ)",
                            Thang = ThangLoc,
                            Nam = NamLoc,
                            TienThue = phong.GiaThue,
                            ChiSoDienDau = hdTruoc?.ChiSoDienCuoi ?? 0,
                            ChiSoDienCuoi = hdTruoc?.ChiSoDienCuoi ?? 0,
                            GiaDien = phong.GiaDien,
                            ChiSoNuocDau = hdTruoc?.ChiSoNuocCuoi ?? 0,
                            ChiSoNuocCuoi = hdTruoc?.ChiSoNuocCuoi ?? 0,
                            GiaNuoc = phong.GiaNuoc,
                            TienInternet = phong.GiaInternet,
                            TienRac = phong.GiaRac,
                            TienXe = phong.GiaXeMay,
                            TrangThai = "Chưa thanh toán",
                            NgayLap = DateTime.Today,
                            HopDongId = hopDong?.HopDongID
                        };

                        db.HoaDons.Add(hd);
                        count++;
                    }

                    if (count == 0)
                    {
                        MessageBox.Show("Tất cả phòng đang thuê đã có hóa đơn tháng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        db.SaveChanges();
                        MessageBox.Show($"Đã tạo {count} hóa đơn thành công!\nVui lòng nhập chỉ số điện/nước cho từng phòng.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                LamMoiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MoFormThemHoaDon()
        {
            FormHoaDon = new HoaDon { Thang = ThangLoc, Nam = NamLoc };
            _isEditHoaDon = false;
            FormHoaDonTitle = "TẠO HÓA ĐƠN THỦ CÔNG";
            IsFormHoaDonVisible = true;
        }

        private void MoFormSuaHoaDon()
        {
            if (HoaDonDangChon == null) return;
            FormHoaDon = new HoaDon
            {
                HoaDonID = HoaDonDangChon.HoaDonID,
                PhongID = HoaDonDangChon.PhongID,
                TenPhong = HoaDonDangChon.TenPhong,
                TenKhach = HoaDonDangChon.TenKhach,
                Thang = HoaDonDangChon.Thang,
                Nam = HoaDonDangChon.Nam,
                TienThue = HoaDonDangChon.TienThue,
                ChiSoDienDau = HoaDonDangChon.ChiSoDienDau,
                ChiSoDienCuoi = HoaDonDangChon.ChiSoDienCuoi,
                GiaDien = HoaDonDangChon.GiaDien,
                ChiSoNuocDau = HoaDonDangChon.ChiSoNuocDau,
                ChiSoNuocCuoi = HoaDonDangChon.ChiSoNuocCuoi,
                GiaNuoc = HoaDonDangChon.GiaNuoc,
                TienInternet = HoaDonDangChon.TienInternet,
                TienRac = HoaDonDangChon.TienRac,
                TienXe = HoaDonDangChon.TienXe,
                PhiKhac = HoaDonDangChon.PhiKhac,
                GhiChuPhiKhac = HoaDonDangChon.GhiChuPhiKhac,
                TrangThai = HoaDonDangChon.TrangThai,
                SoTienDaTra = HoaDonDangChon.SoTienDaTra,
                GhiChu = HoaDonDangChon.GhiChu,
                HopDongId = HoaDonDangChon.HopDongId
            };
            _isEditHoaDon = true;
            FormFormHoaDonTitle();
            IsFormHoaDonVisible = true;
        }

        private void FormFormHoaDonTitle()
        {
            FormHoaDonTitle = $"SỬA HÓA ĐƠN - {HoaDonDangChon.TenPhong} ({HoaDonDangChon.ThangNamText})";
        }

        private void LuuHoaDon()
        {
            if (FormHoaDon == null) return;
            if (FormHoaDon.PhongID == 0)
            {
                MessageBox.Show("Vui lòng chọn phòng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ràng buộc nhập liệu chỉ số điện/nước
            if (FormHoaDon.ChiSoDienCuoi < FormHoaDon.ChiSoDienDau)
            {
                MessageBox.Show($"Chỉ số điện cuối kỳ ({FormHoaDon.ChiSoDienCuoi}) không được nhỏ hơn chỉ số điện đầu kỳ ({FormHoaDon.ChiSoDienDau})!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (FormHoaDon.ChiSoNuocCuoi < FormHoaDon.ChiSoNuocDau)
            {
                MessageBox.Show($"Chỉ số nước cuối kỳ ({FormHoaDon.ChiSoNuocCuoi}) không được nhỏ hơn chỉ số nước đầu kỳ ({FormHoaDon.ChiSoNuocDau})!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    var phong = db.PhongTros.Find(FormHoaDon.PhongID);
                    var khach = db.KhachThues.FirstOrDefault(k => k.PhongID == FormHoaDon.PhongID && k.DangO);
                    var hopDong = db.HopDongs.FirstOrDefault(h => h.PhongID == FormHoaDon.PhongID && h.TrangThai == "Hoạt động");

                    FormHoaDon.TenPhong = phong?.TenPhong;
                    FormHoaDon.TenKhach = khach?.HoTen ?? "(Chưa rõ)";
                    FormHoaDon.HopDongId = hopDong?.HopDongID;

                    if (_isEditHoaDon)
                    {
                        var existing = db.HoaDons.Find(FormHoaDon.HoaDonID);
                        if (existing != null)
                        {
                            existing.PhongID = FormHoaDon.PhongID;
                            existing.TenPhong = FormHoaDon.TenPhong;
                            existing.TenKhach = FormHoaDon.TenKhach;
                            existing.Thang = FormHoaDon.Thang;
                            existing.Nam = FormHoaDon.Nam;
                            existing.TienThue = FormHoaDon.TienThue;
                            existing.ChiSoDienDau = FormHoaDon.ChiSoDienDau;
                            existing.ChiSoDienCuoi = FormHoaDon.ChiSoDienCuoi;
                            existing.GiaDien = FormHoaDon.GiaDien;
                            existing.ChiSoNuocDau = FormHoaDon.ChiSoNuocDau;
                            existing.ChiSoNuocCuoi = FormHoaDon.ChiSoNuocCuoi;
                            existing.GiaNuoc = FormHoaDon.GiaNuoc;
                            existing.TienInternet = FormHoaDon.TienInternet;
                            existing.TienRac = FormHoaDon.TienRac;
                            existing.TienXe = FormHoaDon.TienXe;
                            existing.PhiKhac = FormHoaDon.PhiKhac;
                            existing.GhiChuPhiKhac = FormHoaDon.GhiChuPhiKhac;
                            existing.TrangThai = FormHoaDon.TrangThai;
                            existing.SoTienDaTra = FormHoaDon.SoTienDaTra;
                            existing.GhiChu = FormHoaDon.GhiChu;
                            existing.HopDongId = FormHoaDon.HopDongId;
                        }
                    }
                    else
                    {
                        db.HoaDons.Add(FormHoaDon);
                    }
                    db.SaveChanges();
                }

                IsFormHoaDonVisible = false;
                LamMoiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void XoaHoaDon()
        {
            if (HoaDonDangChon == null) return;
            if (HoaDonDangChon.TrangThai == "Đã thanh toán")
            {
                MessageBox.Show("Không thể xóa hóa đơn đã thanh toán!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var res = MessageBox.Show($"Xóa hóa đơn phòng {HoaDonDangChon.TenPhong} tháng {HoaDonDangChon.Thang}/{HoaDonDangChon.Nam}?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new NhaTroDbContext())
                    {
                        var existing = db.HoaDons.Find(HoaDonDangChon.HoaDonID);
                        if (existing != null)
                        {
                            db.HoaDons.Remove(existing);
                            db.SaveChanges();
                        }
                    }
                    LamMoiDanhSach();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MoFormThanhToan()
        {
            if (HoaDonDangChon == null) return;
            SoTienThanhToan = HoaDonDangChon.ConNo;
            HinhThucThanhToan = "Tiền mặt";
            IsFormThanhToanVisible = true;
        }

        private void GhiNhanThanhToan()
        {
            if (HoaDonDangChon == null) return;
            if (SoTienThanhToan <= 0)
            {
                MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    var hd = db.HoaDons.Find(HoaDonDangChon.HoaDonID);
                    if (hd != null)
                    {
                        hd.SoTienDaTra += SoTienThanhToan;
                        hd.HinhThucThanhToan = HinhThucThanhToan;
                        hd.NgayThanhToan = DateTime.Now;
                        if (hd.SoTienDaTra >= hd.TongTien)
                            hd.TrangThai = "Đã thanh toán";
                        else
                            hd.TrangThai = "Trả một phần";
                        db.SaveChanges();
                    }
                }

                IsFormThanhToanVisible = false;
                MessageBox.Show("Đã ghi nhận thanh toán thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                LamMoiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi ghi nhận thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LamMoiDanhSach()
        {
            TimKiem();
        }

        private void CapNhatThongKe()
        {
            OnPropertyChanged(nameof(TongThuThang));
            OnPropertyChanged(nameof(TongConNo));
            OnPropertyChanged(nameof(SoHoaDonChuaThanhToan));
            OnPropertyChanged(nameof(SoHoaDonDaThanhToan));
        }

        private void XuatHoaDonExcel()
        {
            if (HoaDonDangChon == null) return;

            var hd = HoaDonDangChon;
            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"HoaDon_Phong_{hd.TenPhong}_Thang_{hd.Thang:D2}_{hd.Nam}.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var dataToExport = new[]
                    {
                        new { KhoanMuc = "HÓA ĐƠN TIỀN PHÒNG & DỊCH VỤ", GiaTri = "" },
                        new { KhoanMuc = $"Kỳ hóa đơn: {hd.ThangNamText}", GiaTri = "" },
                        new { KhoanMuc = $"Phòng: {hd.TenPhong}", GiaTri = "" },
                        new { KhoanMuc = $"Khách thuê đại diện: {hd.TenKhach}", GiaTri = "" },
                        new { KhoanMuc = "--------------------------------------", GiaTri = "" },
                        new { KhoanMuc = "Khoản mục chi tiết", GiaTri = "Thành tiền" },
                        new { KhoanMuc = "1. Tiền thuê phòng", GiaTri = hd.TienThue.ToString("N0") + " đ" },
                        new { KhoanMuc = $"2. Tiền điện (Số đầu: {hd.ChiSoDienDau}, Số cuối: {hd.ChiSoDienCuoi}, Tiêu thụ: {hd.SoDienTieuThu} kWh, Đơn giá: {hd.GiaDien:N0} đ/kWh)", GiaTri = hd.TienDien.ToString("N0") + " đ" },
                        new { KhoanMuc = $"3. Tiền nước (Số đầu: {hd.ChiSoNuocDau}, Số cuối: {hd.ChiSoNuocCuoi}, Tiêu thụ: {hd.SoNuocTieuThu} m³, Đơn giá: {hd.GiaNuoc:N0} đ/m³)", GiaTri = hd.TienNuoc.ToString("N0") + " đ" },
                        new { KhoanMuc = "4. Tiền Internet", GiaTri = hd.TienInternet.ToString("N0") + " đ" },
                        new { KhoanMuc = "5. Tiền gửi xe", GiaTri = hd.TienXe.ToString("N0") + " đ" },
                        new { KhoanMuc = "6. Tiền rác & dịch vụ khác", GiaTri = hd.TienRac.ToString("N0") + " đ" },
                        new { KhoanMuc = "--------------------------------------", GiaTri = "" },
                        new { KhoanMuc = "TỔNG TIỀN PHẢI THANH TOÁN", GiaTri = hd.TongTien.ToString("N0") + " đ" },
                        new { KhoanMuc = "Đã thanh toán thực tế", GiaTri = hd.SoTienDaTra.ToString("N0") + " đ" },
                        new { KhoanMuc = "Còn lại cần đóng", GiaTri = hd.ConNo.ToString("N0") + " đ" },
                        new { KhoanMuc = "Trạng thái thanh toán", GiaTri = hd.TrangThai },
                        new { KhoanMuc = $"Ngày thu tiền: {(hd.NgayThanhToan.HasValue ? hd.NgayThanhToan.Value.ToString("dd/MM/yyyy") : "Chưa thanh toán")}", GiaTri = "" },
                        new { KhoanMuc = $"Hình thức thanh toán: {hd.HinhThucThanhToan ?? "N/A"}", GiaTri = "" },
                        new { KhoanMuc = $"Ngày lập hóa đơn: {hd.NgayLap.ToString("dd/MM/yyyy")}", GiaTri = "" }
                    };

                    MiniExcel.SaveAs(dialog.FileName, dataToExport);
                    MessageBox.Show($"Xuất hóa đơn phòng {hd.TenPhong} thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xuất hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
