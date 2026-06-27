# Hệ Thống Quản Lý Nhà Trọ (WPF - MVVM)

Đây là ứng dụng quản lý nhà trọ (desktop application) được xây dựng bằng **WPF (.NET Framework 4.7.2)** theo mô hình kiến trúc **MVVM**, sử dụng **Entity Framework 6 (Code First)** và cơ sở dữ liệu **SQL Server**.

Dự án hỗ trợ các chủ nhà trọ dễ dàng quản lý phòng, thông tin khách thuê, hợp đồng, tính toán hóa đơn dịch vụ hàng tháng và theo dõi doanh thu.

---

## 🛠 Công nghệ sử dụng
* **Ngôn ngữ**: C# / XAML
* **Framework**: WPF (.NET Framework 4.7.2)
* **Kiến trúc**: MVVM (Model-View-ViewModel)
* **Database**: Microsoft SQL Server / LocalDB
* **ORM**: Entity Framework 6 (EF6)

---

## 🔑 Các chức năng chính

### 1. Đăng nhập & Tài khoản
* Đăng nhập hệ thống phân quyền Admin (chủ trọ) và User (khách tìm phòng).
* Chức năng quên mật khẩu, hỗ trợ gửi mã OTP giả lập qua SMS.

### 2. Quản lý phòng trọ
* Xem danh sách phòng trọ, trạng thái phòng (Trống, Đang thuê, Đang sửa, Đang dọn dẹp).
* Cấu hình giá thuê cơ bản và các biểu phí dịch vụ (Điện, Nước, Internet, Vệ sinh, Xe máy).
* Quản lý danh mục tiện nghi của phòng (Điều hòa, Nóng lạnh, Tủ lạnh, Giường...).

### 3. Quản lý khách thuê & Hợp đồng
* Lưu trữ thông tin chi tiết khách thuê (Họ tên, CCCD, SĐT, Quê quán, Người liên hệ khẩn cấp).
* Tạo hợp đồng thuê phòng: liên kết khách thuê với phòng, nhập tiền đặt cọc, ngày bắt đầu/kết thúc và điều khoản.
* Tự động cập nhật trạng thái phòng tương ứng khi tạo hoặc kết thúc hợp đồng.

### 4. Tính hóa đơn & Thanh toán
* Ghi nhận chỉ số điện, nước đầu kỳ và cuối kỳ để tính tiền thực tế sử dụng.
* Hỗ trợ tạo hóa đơn tự động hàng tháng cho tất cả các phòng đang thuê.
* Ghi nhận thanh toán hóa đơn (trả hết hoặc trả một phần), lưu hình thức thanh toán (chuyển khoản, tiền mặt).

### 5. Thống kê & Báo cáo
* Giao diện Dashboard hiển thị số lượng phòng trống, phòng đang thuê, số phòng đang sửa chữa.
* Thống kê tổng số doanh thu thực tế đã thu trong tháng và số tiền khách còn nợ.

---

## 📂 Cấu trúc thư mục dự án
```text
baitaplon/
├── Models/              # Chứa các Model và DB Context (EF Code First)
│   ├── HoaDon.cs
│   ├── HopDong.cs
│   ├── KhachThue.cs
│   ├── PhongTro.cs
│   └── NhaTroDbContext.cs
├── ViewModels/          # Xử lý logic và liên kết dữ liệu (Data Binding)
│   ├── BaseViewModel.cs # Triển khai INotifyPropertyChanged
│   ├── RelayCommand.cs  # Triển khai ICommand
│   └── Admin/           # ViewModels cho các chức năng quản trị
├── Views/               # Giao diện XAML
│   ├── Admin/           # Giao diện quản lý phòng, khách, hóa đơn
│   ├── LoginWindow.xaml
│   └── ForgotPasswordWindow.xaml
├── App.config           # Cấu hình chuỗi kết nối cơ sở dữ liệu
└── baitaplon.sln        # File Solution của dự án
```

---

## 🚀 Hướng dẫn cài đặt và chạy thử

### Yêu cầu hệ thống:
* Hệ điều hành Windows 10/11.
* Visual Studio 2019 hoặc 2022 (đã cài đặt gói phát triển `.NET Desktop Development`).
* SQL Server Express hoặc LocalDB.

### Các bước thực hiện:

1. **Tải mã nguồn về máy**
   ```bash
   git clone https://github.com/Babiboyy55/motel.git
   cd motel
   ```

2. **Cấu hình Database**
   * Mở file `baitaplon/App.config`.
   * Tìm thẻ `<connectionStrings>` và sửa lại thuộc tính `connectionString` (đặc biệt là mục `Data Source`) cho phù hợp với server SQL Server của bạn (ví dụ: `.\SQLEXPRESS` hoặc `(localdb)\MSSQLLocalDB`).
   ```xml
   <connectionStrings>
       <add name="DefaultConnection"
            connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyNhaTroDB;Integrated Security=True;"
            providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

3. **Tự động tạo database**
   * Mở file Solution `baitaplon.sln` bằng Visual Studio.
   * Chọn **Tools** -> **NuGet Package Manager** -> **Package Manager Console**.
   * Chạy lệnh sau để Entity Framework tự động tạo database và bảng:
     ```powershell
     Update-Database
     ```

4. **Chạy ứng dụng**
   * Nhấn `F5` hoặc chọn nút **Start** trên thanh công cụ của Visual Studio để chạy chương trình.
