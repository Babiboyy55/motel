using Quanlynhatro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quanlynhatro.Services
{
    /// <summary>
    /// DataService - Quản lý dữ liệu in-memory (có thể thay bằng DB sau)
    /// </summary>
    public static class DataService
    {
        private static int _phongCounter = 10;
        private static int _khachCounter = 10;
        private static int _hopDongCounter = 10;
        private static int _hoaDonCounter = 10;

        // ====================== PHÒNG ======================
        public static ObservableCollection<PhongTro> DanhSachPhong { get; private set; }

        // ====================== KHÁCH ======================
        public static ObservableCollection<KhachThue> DanhSachKhach { get; private set; }

        // ====================== HỢP ĐỒNG ======================
        public static ObservableCollection<HopDong> DanhSachHopDong { get; private set; }

        // ====================== HÓA ĐƠN ======================
        public static ObservableCollection<HoaDon> DanhSachHoaDon { get; private set; }

        static DataService()
        {
            KhoiTaoDuLieuMau();
        }

        private static void KhoiTaoDuLieuMau()
        {
            // --- PHÒNG MẪU ---
            DanhSachPhong = new ObservableCollection<PhongTro>
            {
                new PhongTro
                {
                    PhongID = 1, TenPhong = "P101", LoaiPhong = LoaiPhong.PhongDon,
                    Tang = 1, DienTich = 18, GiaThue = 2500000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 100000,
                    TrangThai = TrangThaiPhong.DangThue, TrangThaiNoiThat = "Đầy đủ",
                    CoWifi = true, CoDieuHoa = true, CoNongLanh = true,
                    MoTa = "Phòng đơn tầng 1, view sân, nội thất đầy đủ"
                },
                new PhongTro
                {
                    PhongID = 2, TenPhong = "P102", LoaiPhong = LoaiPhong.PhongDoi,
                    Tang = 1, DienTich = 25, GiaThue = 3500000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 200000,
                    TrangThai = TrangThaiPhong.DangThue, TrangThaiNoiThat = "Đầy đủ",
                    CoWifi = true, CoDieuHoa = true, CoNongLanh = true, CoTuLanh = true, CoTivi = true,
                    MoTa = "Phòng đôi rộng rãi, nội thất cao cấp"
                },
                new PhongTro
                {
                    PhongID = 3, TenPhong = "P201", LoaiPhong = LoaiPhong.PhongDon,
                    Tang = 2, DienTich = 20, GiaThue = 2800000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 100000,
                    TrangThai = TrangThaiPhong.Trong, TrangThaiNoiThat = "Cơ bản",
                    CoWifi = true, CoNongLanh = true, CoDieuHoa = false,
                    MoTa = "Phòng đơn tầng 2, thoáng mát"
                },
                new PhongTro
                {
                    PhongID = 4, TenPhong = "P202", LoaiPhong = LoaiPhong.PhongGhep,
                    Tang = 2, DienTich = 30, GiaThue = 1500000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 100000,
                    TrangThai = TrangThaiPhong.DangThue, TrangThaiNoiThat = "Cơ bản",
                    CoWifi = true, CoMayGiat = true, CoBep = true,
                    MoTa = "Phòng ghép 3 người, có bếp, máy giặt chung"
                },
                new PhongTro
                {
                    PhongID = 5, TenPhong = "P301", LoaiPhong = LoaiPhong.CanHo,
                    Tang = 3, DienTich = 45, GiaThue = 5500000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 150000, GiaRac = 30000, GiaXeMay = 200000,
                    TrangThai = TrangThaiPhong.DangSuaChua, TrangThaiNoiThat = "Đầy đủ",
                    CoWifi = true, CoDieuHoa = true, CoNongLanh = true, CoTuLanh = true, CoTivi = true, CoBep = true, CoMayGiat = true,
                    MoTa = "Căn hộ mini đang sửa chữa, đầy đủ nội thất cao cấp"
                },
                new PhongTro
                {
                    PhongID = 6, TenPhong = "P302", LoaiPhong = LoaiPhong.PhongDon,
                    Tang = 3, DienTich = 22, GiaThue = 3000000, GiaDien = 3500, GiaNuoc = 15000,
                    GiaInternet = 100000, GiaRac = 20000, GiaXeMay = 100000,
                    TrangThai = TrangThaiPhong.DangDon, TrangThaiNoiThat = "Cơ bản",
                    CoWifi = true, CoDieuHoa = true,
                    MoTa = "Phòng đơn tầng 3, vừa có khách ra, đang dọn dẹp"
                },
            };

            // --- KHÁCH THUÊ MẪU ---
            DanhSachKhach = new ObservableCollection<KhachThue>
            {
                new KhachThue
                {
                    KhachID = 1, HoTen = "Nguyễn Văn An", CCCD = "012345678901",
                    SoDienThoai = "0901234567", Email = "an.nguyen@gmail.com",
                    NgaySinh = new DateTime(1998, 5, 15), GioiTinh = "Nam",
                    QueQuan = "Hà Nội", DiaChiHienTai = "123 Đường ABC, Q.1, TP.HCM",
                    NguoiLienHeKhan = "Nguyễn Văn Bình", QuanHeNguoiLienHe = "Bố", SoDTNguoiLienHe = "0912345678",
                    PhongID = 1, NgayVaoO = new DateTime(2025, 1, 1), DangO = true,
                    GhiChu = "Khách thuê lâu dài, trả tiền đúng hạn"
                },
                new KhachThue
                {
                    KhachID = 2, HoTen = "Trần Thị Bình", CCCD = "098765432100",
                    SoDienThoai = "0912345678", Email = "binh.tran@gmail.com",
                    NgaySinh = new DateTime(2000, 8, 20), GioiTinh = "Nữ",
                    QueQuan = "Đà Nẵng", DiaChiHienTai = "456 Đường DEF, Q.3, TP.HCM",
                    NguoiLienHeKhan = "Trần Văn C", QuanHeNguoiLienHe = "Anh", SoDTNguoiLienHe = "0923456789",
                    PhongID = 2, NgayVaoO = new DateTime(2025, 3, 1), DangO = true,
                    GhiChu = ""
                },
                new KhachThue
                {
                    KhachID = 3, HoTen = "Lê Hoàng Cường", CCCD = "011223344556",
                    SoDienThoai = "0934567890", Email = "cuong.le@gmail.com",
                    NgaySinh = new DateTime(1997, 12, 3), GioiTinh = "Nam",
                    QueQuan = "Hồ Chí Minh",
                    NguoiLienHeKhan = "Lê Thị Dung", QuanHeNguoiLienHe = "Mẹ", SoDTNguoiLienHe = "0956789012",
                    PhongID = 4, NgayVaoO = new DateTime(2025, 6, 1), DangO = true
                },
                new KhachThue
                {
                    KhachID = 4, HoTen = "Phạm Thị Duyên", CCCD = "022334455667",
                    SoDienThoai = "0945678901", Email = "duyen.pham@gmail.com",
                    NgaySinh = new DateTime(2001, 3, 8), GioiTinh = "Nữ",
                    QueQuan = "Cần Thơ",
                    NguoiLienHeKhan = "Phạm Văn E", QuanHeNguoiLienHe = "Anh", SoDTNguoiLienHe = "0967890123",
                    PhongID = 4, NgayVaoO = new DateTime(2025, 6, 1), DangO = true
                },
                new KhachThue
                {
                    KhachID = 5, HoTen = "Võ Minh Khoa", CCCD = "033445566778",
                    SoDienThoai = "0956789012", Email = "khoa.vo@gmail.com",
                    NgaySinh = new DateTime(1999, 7, 25), GioiTinh = "Nam",
                    QueQuan = "Bình Dương",
                    NguoiLienHeKhan = "Võ Thị F", QuanHeNguoiLienHe = "Mẹ", SoDTNguoiLienHe = "0978901234",
                    PhongID = 2, NgayVaoO = new DateTime(2025, 3, 1), DangO = true
                },
            };

            // --- HỢP ĐỒNG MẪU ---
            DanhSachHopDong = new ObservableCollection<HopDong>
            {
                new HopDong
                {
                    HopDongID = 1, KhachID = 1, PhongID = 1, TenKhach = "Nguyễn Văn An", TenPhong = "P101",
                    NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2026, 1, 1),
                    GiaThue = 2500000, TienCoc = 2500000, DieuKhoan = "Trả tiền trước ngày 5 hàng tháng. Báo trước 30 ngày khi dời phòng.",
                    TrangThai = "Hoạt động", GhiChu = "Hợp đồng 12 tháng"
                },
                new HopDong
                {
                    HopDongID = 2, KhachID = 2, PhongID = 2, TenKhach = "Trần Thị Bình", TenPhong = "P102",
                    NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2026, 3, 1),
                    GiaThue = 3500000, TienCoc = 3500000, DieuKhoan = "Trả tiền trước ngày 5 hàng tháng. Báo trước 30 ngày khi dời phòng.",
                    TrangThai = "Hoạt động"
                },
                new HopDong
                {
                    HopDongID = 3, KhachID = 3, PhongID = 4, TenKhach = "Lê Hoàng Cường", TenPhong = "P202",
                    NgayBatDau = new DateTime(2025, 6, 1), NgayKetThuc = new DateTime(2026, 6, 1),
                    GiaThue = 1500000, TienCoc = 1500000, DieuKhoan = "Trả tiền trước ngày 5 hàng tháng.",
                    TrangThai = "Hoạt động"
                },
            };

            // --- HÓA ĐƠN MẪU ---
            DanhSachHoaDon = new ObservableCollection<HoaDon>
            {
                new HoaDon
                {
                    HoaDonID = 1, PhongID = 1, TenPhong = "P101", TenKhach = "Nguyễn Văn An",
                    Thang = 5, Nam = 2026, TienThue = 2500000,
                    ChiSoDienDau = 1200, ChiSoDienCuoi = 1285, GiaDien = 3500,
                    ChiSoNuocDau = 45, ChiSoNuocCuoi = 51, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 100000,
                    TrangThai = "Đã thanh toán", SoTienDaTra = 2820000 + 100000 + 20000 + 100000,
                    NgayThanhToan = new DateTime(2026, 5, 4), HinhThucThanhToan = "Chuyển khoản",
                    NgayLap = new DateTime(2026, 5, 1)
                },
                new HoaDon
                {
                    HoaDonID = 2, PhongID = 2, TenPhong = "P102", TenKhach = "Trần Thị Bình",
                    Thang = 5, Nam = 2026, TienThue = 3500000,
                    ChiSoDienDau = 890, ChiSoDienCuoi = 1002, GiaDien = 3500,
                    ChiSoNuocDau = 30, ChiSoNuocCuoi = 38, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 200000,
                    TrangThai = "Chưa thanh toán", SoTienDaTra = 0,
                    NgayLap = new DateTime(2026, 5, 1)
                },
                new HoaDon
                {
                    HoaDonID = 3, PhongID = 4, TenPhong = "P202", TenKhach = "Lê Hoàng Cường",
                    Thang = 5, Nam = 2026, TienThue = 1500000,
                    ChiSoDienDau = 500, ChiSoDienCuoi = 560, GiaDien = 3500,
                    ChiSoNuocDau = 20, ChiSoNuocCuoi = 24, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 100000,
                    TrangThai = "Trả một phần", SoTienDaTra = 1000000,
                    NgayLap = new DateTime(2026, 5, 1)
                },
                // Tháng 4 đã TT
                new HoaDon
                {
                    HoaDonID = 4, PhongID = 1, TenPhong = "P101", TenKhach = "Nguyễn Văn An",
                    Thang = 4, Nam = 2026, TienThue = 2500000,
                    ChiSoDienDau = 1120, ChiSoDienCuoi = 1200, GiaDien = 3500,
                    ChiSoNuocDau = 38, ChiSoNuocCuoi = 45, GiaNuoc = 15000,
                    TienInternet = 100000, TienRac = 20000, TienXe = 100000,
                    TrangThai = "Đã thanh toán", SoTienDaTra = 3125000,
                    NgayThanhToan = new DateTime(2026, 4, 3), HinhThucThanhToan = "Tiền mặt",
                    NgayLap = new DateTime(2026, 4, 1)
                },
            };
        }

        // ======== CRUD PHÒNG ========
        public static void ThemPhong(PhongTro phong)
        {
            phong.PhongID = ++_phongCounter;
            DanhSachPhong.Add(phong);
        }

        public static void SuaPhong(PhongTro phong)
        {
            var existing = DanhSachPhong.FirstOrDefault(p => p.PhongID == phong.PhongID);
            if (existing != null)
            {
                int idx = DanhSachPhong.IndexOf(existing);
                DanhSachPhong[idx] = phong;
            }
        }

        public static void XoaPhong(int phongID)
        {
            var phong = DanhSachPhong.FirstOrDefault(p => p.PhongID == phongID);
            if (phong != null) DanhSachPhong.Remove(phong);
        }

        // ======== CRUD KHÁCH ========
        public static void ThemKhach(KhachThue khach)
        {
            khach.KhachID = ++_khachCounter;
            DanhSachKhach.Add(khach);
        }

        public static void SuaKhach(KhachThue khach)
        {
            var existing = DanhSachKhach.FirstOrDefault(k => k.KhachID == khach.KhachID);
            if (existing != null)
            {
                int idx = DanhSachKhach.IndexOf(existing);
                DanhSachKhach[idx] = khach;
            }
        }

        public static void XoaKhach(int khachID)
        {
            var khach = DanhSachKhach.FirstOrDefault(k => k.KhachID == khachID);
            if (khach != null) DanhSachKhach.Remove(khach);
        }

        // ======== CRUD HỢP ĐỒNG ========
        public static void ThemHopDong(HopDong hd)
        {
            hd.HopDongID = ++_hopDongCounter;
            DanhSachHopDong.Add(hd);
            // Cập nhật trạng thái phòng
            var phong = DanhSachPhong.FirstOrDefault(p => p.PhongID == hd.PhongID);
            if (phong != null) phong.TrangThai = TrangThaiPhong.DangThue;
        }

        public static void SuaHopDong(HopDong hd)
        {
            var existing = DanhSachHopDong.FirstOrDefault(h => h.HopDongID == hd.HopDongID);
            if (existing != null)
            {
                int idx = DanhSachHopDong.IndexOf(existing);
                DanhSachHopDong[idx] = hd;
            }
        }

        public static void ChamDutHopDong(int hopDongID)
        {
            var hd = DanhSachHopDong.FirstOrDefault(h => h.HopDongID == hopDongID);
            if (hd != null)
            {
                hd.TrangThai = "Đã chấm dứt";
                var phong = DanhSachPhong.FirstOrDefault(p => p.PhongID == hd.PhongID);
                if (phong != null) phong.TrangThai = TrangThaiPhong.Trong;
            }
        }

        // ======== CRUD HÓA ĐƠN ========
        public static void ThemHoaDon(HoaDon hd)
        {
            hd.HoaDonID = ++_hoaDonCounter;
            DanhSachHoaDon.Add(hd);
        }

        public static void SuaHoaDon(HoaDon hd)
        {
            var existing = DanhSachHoaDon.FirstOrDefault(h => h.HoaDonID == hd.HoaDonID);
            if (existing != null)
            {
                int idx = DanhSachHoaDon.IndexOf(existing);
                DanhSachHoaDon[idx] = hd;
            }
        }

        public static void XoaHoaDon(int hoaDonID)
        {
            var hd = DanhSachHoaDon.FirstOrDefault(h => h.HoaDonID == hoaDonID);
            if (hd != null) DanhSachHoaDon.Remove(hd);
        }

        public static void GhiNhanThanhToan(int hoaDonID, decimal soTien, string hinhThuc)
        {
            var hd = DanhSachHoaDon.FirstOrDefault(h => h.HoaDonID == hoaDonID);
            if (hd != null)
            {
                hd.SoTienDaTra += soTien;
                hd.HinhThucThanhToan = hinhThuc;
                hd.NgayThanhToan = DateTime.Now;
                if (hd.SoTienDaTra >= hd.TongTien)
                    hd.TrangThai = "Đã thanh toán";
                else
                    hd.TrangThai = "Trả một phần";
                SuaHoaDon(hd);
            }
        }

        /// <summary>
        /// Tạo hóa đơn tự động cho tất cả phòng đang thuê trong tháng/năm chỉ định
        /// </summary>
        public static List<HoaDon> TaoHoaDonTuDong(int thang, int nam)
        {
            var dsPhongDangThue = DanhSachPhong.Where(p => p.TrangThai == TrangThaiPhong.DangThue).ToList();
            var ketQua = new List<HoaDon>();

            foreach (var phong in dsPhongDangThue)
            {
                // Kiểm tra đã có hóa đơn tháng này chưa
                bool daCoHD = DanhSachHoaDon.Any(h => h.PhongID == phong.PhongID && h.Thang == thang && h.Nam == nam);
                if (daCoHD) continue;

                var khach = DanhSachKhach.FirstOrDefault(k => k.PhongID == phong.PhongID && k.DangO);
                // Lấy chỉ số kỳ trước
                var hdTruoc = DanhSachHoaDon
                    .Where(h => h.PhongID == phong.PhongID)
                    .OrderByDescending(h => h.Nam * 100 + h.Thang)
                    .FirstOrDefault();

                var hd = new HoaDon
                {
                    PhongID = phong.PhongID,
                    TenPhong = phong.TenPhong,
                    TenKhach = khach?.HoTen ?? "(Chưa rõ)",
                    Thang = thang,
                    Nam = nam,
                    TienThue = phong.GiaThue,
                    ChiSoDienDau = hdTruoc?.ChiSoDienCuoi ?? 0,
                    ChiSoDienCuoi = hdTruoc?.ChiSoDienCuoi ?? 0,   // Cần nhập thêm
                    GiaDien = phong.GiaDien,
                    ChiSoNuocDau = hdTruoc?.ChiSoNuocCuoi ?? 0,
                    ChiSoNuocCuoi = hdTruoc?.ChiSoNuocCuoi ?? 0,   // Cần nhập thêm
                    GiaNuoc = phong.GiaNuoc,
                    TienInternet = phong.GiaInternet,
                    TienRac = phong.GiaRac,
                    TienXe = phong.GiaXeMay,
                    TrangThai = "Chưa thanh toán",
                    NgayLap = DateTime.Today
                };

                ThemHoaDon(hd);
                ketQua.Add(hd);
            }

            return ketQua;
        }

        // Thống kê
        public static decimal TongThuThang(int thang, int nam)
            => DanhSachHoaDon.Where(h => h.Thang == thang && h.Nam == nam && h.TrangThai == "Đã thanh toán").Sum(h => h.TongTien);

        public static decimal TongConNo()
            => DanhSachHoaDon.Where(h => h.TrangThai != "Đã thanh toán").Sum(h => h.ConNo);

        public static int SoPhongTrong()
            => DanhSachPhong.Count(p => p.TrangThai == TrangThaiPhong.Trong);

        public static int SoPhongDangThue()
            => DanhSachPhong.Count(p => p.TrangThai == TrangThaiPhong.DangThue);
    }
}
