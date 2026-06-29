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
        public ICommand XuatHoaDonCommand { get; }

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
            XuatHoaDonCommand = new RelayCommand(o => XuatHoaDon(), o => HoaDonDangChon != null);

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

        private void XuatHoaDon()
        {
            if (HoaDonDangChon == null) return;

            var hd = HoaDonDangChon;
            var dialog = new SaveFileDialog
            {
                Filter = "Hóa đơn Web/PDF (*.html)|*.html|Hóa đơn văn bản (*.txt)|*.txt",
                FileName = $"HoaDon_Phong_{hd.TenPhong}_Thang_{hd.Thang:D2}_{hd.Nam}"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string fileExtension = System.IO.Path.GetExtension(dialog.FileName).ToLower();

                    if (fileExtension == ".html" || fileExtension == ".htm")
                    {
                        string htmlContent = GenerateHtmlInvoice(hd);
                        System.IO.File.WriteAllText(dialog.FileName, htmlContent, System.Text.Encoding.UTF8);
                    }
                    else
                    {
                        string txtContent = GenerateTextInvoice(hd);
                        System.IO.File.WriteAllText(dialog.FileName, txtContent, System.Text.Encoding.UTF8);
                    }

                    var result = MessageBox.Show($"Xuất hóa đơn phòng {hd.TenPhong} thành công!\nBạn có muốn mở tệp tin hóa đơn vừa xuất không?", 
                        "Xuất hóa đơn thành công", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = dialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xuất hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string GenerateHtmlInvoice(HoaDon hd)
        {
            string statusClass = hd.TrangThai == "Đã thanh toán" ? "status-paid" : (hd.TrangThai == "Trả một phần" ? "status-partial" : "status-unpaid");
            string ngayLapText = hd.NgayLap.ToString("dd/MM/yyyy");
            string ngayThanhToanText = hd.NgayThanhToan.HasValue ? hd.NgayThanhToan.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa thanh toán";
            
            string phiKhacRow = "";
            if (hd.PhiKhac > 0)
            {
                phiKhacRow = $@"
                <tr>
                    <td class=""text-center"">7</td>
                    <td>Phí khác: {hd.GhiChuPhiKhac}</td>
                    <td class=""text-right"">{hd.PhiKhac.ToString("N0")} đ</td>
                </tr>";
            }

            string ghiChuRow = "";
            if (!string.IsNullOrEmpty(hd.GhiChu))
            {
                ghiChuRow = $@"
                <tr>
                    <td class=""label"">Ghi chú:</td>
                    <td colspan=""3"">{hd.GhiChu}</td>
                </tr>";
            }

            return $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <title>Hóa đơn phòng {hd.TenPhong} - {hd.ThangNamText}</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.5;
            color: #333;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .invoice-box {{
            max-width: 800px;
            margin: auto;
            padding: 30px;
            border: 1px solid #eee;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.15);
            background-color: #fff;
            border-radius: 8px;
        }}
        .no-print {{
            text-align: right;
            margin-bottom: 20px;
        }}
        .btn-print {{
            background-color: #0066cc;
            color: white;
            border: none;
            padding: 10px 20px;
            font-size: 14px;
            font-weight: bold;
            border-radius: 4px;
            cursor: pointer;
        }}
        .btn-print:hover {{
            background-color: #0052a3;
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
            border-bottom: 3px double #0066cc;
            padding-bottom: 15px;
        }}
        .header h1 {{
            margin: 0 0 10px 0;
            color: #0066cc;
            font-size: 26px;
            text-transform: uppercase;
        }}
        .header p {{
            margin: 5px 0;
            color: #666;
            font-size: 14px;
        }}
        .info-table {{
            width: 100%;
            margin-bottom: 20px;
            border-collapse: collapse;
        }}
        .info-table td {{
            padding: 6px 0;
            vertical-align: top;
        }}
        .info-table td.label {{
            font-weight: bold;
            width: 150px;
            color: #555;
        }}
        .details-table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 25px;
        }}
        .details-table th {{
            background-color: #f2f2f2;
            border: 1px solid #ddd;
            padding: 10px;
            font-weight: bold;
            text-align: left;
        }}
        .details-table td {{
            border: 1px solid #ddd;
            padding: 10px;
        }}
        .details-table tr:nth-child(even) {{
            background-color: #fafafa;
        }}
        .text-right {{
            text-align: right;
        }}
        .text-center {{
            text-align: center;
        }}
        .total-box {{
            float: right;
            width: 300px;
            margin-top: 10px;
            margin-bottom: 30px;
        }}
        .total-row {{
            display: flex;
            justify-content: space-between;
            padding: 6px 0;
            font-size: 14px;
        }}
        .total-row.grand-total {{
            font-size: 18px;
            font-weight: bold;
            color: #0066cc;
            border-top: 2px solid #0066cc;
            border-bottom: 2px solid #0066cc;
            padding: 10px 0;
            margin-top: 5px;
        }}
        .clear {{
            clear: both;
        }}
        .footer {{
            margin-top: 40px;
            display: flex;
            justify-content: space-between;
        }}
        .signature-box {{
            text-align: center;
            width: 200px;
        }}
        .signature-box .title {{
            font-weight: bold;
            margin-bottom: 60px;
        }}
        .signature-box .name {{
            font-style: italic;
            color: #555;
        }}
        .status-badge {{
            display: inline-block;
            padding: 4px 10px;
            border-radius: 4px;
            font-weight: bold;
            font-size: 12px;
            text-transform: uppercase;
        }}
        .status-paid {{
            background-color: #e6f4ea;
            color: #137333;
        }}
        .status-partial {{
            background-color: #fef7e0;
            color: #b06000;
        }}
        .status-unpaid {{
            background-color: #fdf2f2;
            color: #d93025;
        }}
        @media print {{
            body {{
                background-color: #fff;
                padding: 0;
            }}
            .invoice-box {{
                box-shadow: none;
                border: none;
                padding: 0;
            }}
            .no-print {{
                display: none;
            }}
        }}
    </style>
</head>
<body>
    <div class=""no-print"">
        <button class=""btn-print"" onclick=""window.print()"">In Hóa Đơn / Lưu PDF</button>
    </div>
    <div class=""invoice-box"">
        <div class=""header"">
            <h1>Hóa Đơn Tiền Phòng & Dịch Vụ</h1>
            <p>Hệ thống quản lý nhà trọ chuyên nghiệp</p>
        </div>
        <table class=""info-table"">
            <tr>
                <td class=""label"">Mã hóa đơn:</td>
                <td>HD-{hd.HoaDonID}</td>
                <td class=""label"">Ngày lập:</td>
                <td>{ngayLapText}</td>
            </tr>
            <tr>
                <td class=""label"">Phòng:</td>
                <td><strong>{hd.TenPhong}</strong></td>
                <td class=""label"">Kỳ hóa đơn:</td>
                <td><strong>{hd.ThangNamText}</strong></td>
            </tr>
            <tr>
                <td class=""label"">Khách thuê:</td>
                <td>{hd.TenKhach}</td>
                <td class=""label"">Trạng thái:</td>
                <td><span class=""status-badge {statusClass}"">{hd.TrangThai}</span></td>
            </tr>
        </table>
        <table class=""details-table"">
            <thead>
                <tr>
                    <th style=""width: 50px;"" class=""text-center"">STT</th>
                    <th>Khoản mục chi tiết</th>
                    <th style=""width: 150px;"" class=""text-right"">Thành tiền</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td class=""text-center"">1</td>
                    <td>Tiền thuê phòng</td>
                    <td class=""text-right"">{hd.TienThue.ToString("N0")} đ</td>
                </tr>
                <tr>
                    <td class=""text-center"">2</td>
                    <td>
                        Tiền điện (Chỉ số: {hd.ChiSoDienDau} &rarr; {hd.ChiSoDienCuoi} = {hd.SoDienTieuThu} kWh)<br>
                        <small style=""color: #666;"">Đơn giá: {hd.GiaDien.ToString("N0")} đ/kWh</small>
                    </td>
                    <td class=""text-right"">{hd.TienDien.ToString("N0")} đ</td>
                </tr>
                <tr>
                    <td class=""text-center"">3</td>
                    <td>
                        Tiền nước (Chỉ số: {hd.ChiSoNuocDau} &rarr; {hd.ChiSoNuocCuoi} = {hd.SoNuocTieuThu} m³)<br>
                        <small style=""color: #666;"">Đơn giá: {hd.GiaNuoc.ToString("N0")} đ/m³</small>
                    </td>
                    <td class=""text-right"">{hd.TienNuoc.ToString("N0")} đ</td>
                </tr>
                <tr>
                    <td class=""text-center"">4</td>
                    <td>Tiền Internet (WiFi)</td>
                    <td class=""text-right"">{hd.TienInternet.ToString("N0")} đ</td>
                </tr>
                <tr>
                    <td class=""text-center"">5</td>
                    <td>Tiền gửi xe</td>
                    <td class=""text-right"">{hd.TienXe.ToString("N0")} đ</td>
                </tr>
                <tr>
                    <td class=""text-center"">6</td>
                    <td>Tiền rác & dịch vụ chung</td>
                    <td class=""text-right"">{hd.TienRac.ToString("N0")} đ</td>
                </tr>
                {phiKhacRow}
            </tbody>
        </table>
        <div class=""total-box"">
            <div class=""total-row"">
                <span>Cộng tiền phòng & dịch vụ:</span>
                <span>{hd.TongTien.ToString("N0")} đ</span>
            </div>
            <div class=""total-row"">
                <span>Số tiền đã thanh toán:</span>
                <span>{hd.SoTienDaTra.ToString("N0")} đ</span>
            </div>
            <div class=""total-row grand-total"">
                <span>CÒN LẠI CẦN ĐÓNG:</span>
                <span>{hd.ConNo.ToString("N0")} đ</span>
            </div>
        </div>
        <div class=""clear""></div>
        <table class=""info-table"" style=""margin-top: 10px;"">
            <tr>
                <td class=""label"">Hình thức TT:</td>
                <td>{hd.HinhThucThanhToan ?? "N/A"}</td>
                <td class=""label"">Ngày thanh toán:</td>
                <td>{ngayThanhToanText}</td>
            </tr>
            {ghiChuRow}
        </table>
        <div class=""footer"">
            <div class=""signature-box"">
                <div class=""title"">Khách thuê ký nhận</div>
                <div class=""name"">(Ký và ghi rõ họ tên)</div>
            </div>
            <div class=""signature-box"">
                <div class=""title"">Người lập hóa đơn</div>
                <div class=""name"">(Ký và ghi rõ họ tên)</div>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerateTextInvoice(HoaDon hd)
        {
            string ngayLapText = hd.NgayLap.ToString("dd/MM/yyyy");
            string ngayThanhToanText = hd.NgayThanhToan.HasValue ? hd.NgayThanhToan.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa thanh toán";
            
            string phiKhacLine = "";
            if (hd.PhiKhac > 0)
            {
                phiKhacLine = $"\n7. Phí khác ({hd.GhiChuPhiKhac}): {hd.PhiKhac.ToString("N0").PadLeft(20)} đ";
            }

            return $@"==================================================
           HÓA ĐƠN TIỀN PHÒNG & DỊCH VỤ           
==================================================
Mã hóa đơn: HD-{hd.HoaDonID}
Ngày lập: {ngayLapText}
Kỳ hóa đơn: {hd.ThangNamText}
Phòng: {hd.TenPhong}
Khách thuê: {hd.TenKhach}
Trạng thái: {hd.TrangThai}
--------------------------------------------------
1. Tiền thuê phòng:          {hd.TienThue.ToString("N0").PadLeft(20)} đ
2. Tiền điện:                 {hd.TienDien.ToString("N0").PadLeft(20)} đ
   (Chỉ số: {hd.ChiSoDienDau} -> {hd.ChiSoDienCuoi} = {hd.SoDienTieuThu} kWh * {hd.GiaDien.ToString("N0")} đ)
3. Tiền nước:                 {hd.TienNuoc.ToString("N0").PadLeft(20)} đ
   (Chỉ số: {hd.ChiSoNuocDau} -> {hd.ChiSoNuocCuoi} = {hd.SoNuocTieuThu} m3 * {hd.GiaNuoc.ToString("N0")} đ)
4. Tiền Internet:             {hd.TienInternet.ToString("N0").PadLeft(20)} đ
5. Tiền gửi xe:               {hd.TienXe.ToString("N0").PadLeft(20)} đ
6. Tiền rác & dịch vụ khác:   {hd.TienRac.ToString("N0").PadLeft(20)} đ{phiKhacLine}
--------------------------------------------------
CỘNG TIỀN PHÒNG & DỊCH VỤ:    {hd.TongTien.ToString("N0").PadLeft(20)} đ
Đã thanh toán thực tế:        {hd.SoTienDaTra.ToString("N0").PadLeft(20)} đ
CÒN LẠI CẦN ĐÓNG:             {hd.ConNo.ToString("N0").PadLeft(20)} đ
--------------------------------------------------
Hình thức thanh toán: {hd.HinhThucThanhToan ?? "N/A"}
Ngày thanh toán: {ngayThanhToanText}
Ghi chú: {hd.GhiChu ?? "Không"}
==================================================
        Cảm ơn quý khách đã thanh toán!           
==================================================
";
        }
    }
}
