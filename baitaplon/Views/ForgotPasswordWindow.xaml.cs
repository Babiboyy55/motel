using baitaplon.Models;
using baitaplon.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Quanlynhatro.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        private string _generatedOtp;
        private DateTime _otpExpiry;
        private int _targetUserId;

        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSendOtp_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(phone))
            {
                ShowErrorMessage("Vui lòng nhập đầy đủ Tên đăng nhập và Số điện thoại!");
                return;
            }

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    // Kiểm tra xem tên đăng nhập và số điện thoại có khớp không
                    var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == username);

                    if (user == null || user.SoDienThoai != phone)
                    {
                        ShowErrorMessage("Thông tin tài khoản hoặc số điện thoại không chính xác!");
                        return;
                    }

                    // Tạo mã OTP ngẫu nhiên 6 chữ số
                    Random rand = new Random();
                    _generatedOtp = rand.Next(100000, 999999).ToString();
                    _otpExpiry = DateTime.Now.AddMinutes(3); // Hiệu lực 3 phút
                    _targetUserId = user.Id;

                    // Nội dung tin nhắn
                    string message = $"[QuanLyNhaTro] Ma OTP khoi phuc mat khau cua ban la: {_generatedOtp}. Hieu luc trong 3 phut.";

                    // Gửi tin nhắn
                    bool sendSuccess = SmsService.SendSms(phone, message);

                    if (sendSuccess)
                    {
                        ShowSuccessMessage("Mã OTP đã được gửi!");
                        panelStep1.Visibility = Visibility.Collapsed;
                        panelStep2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ShowErrorMessage("Gửi SMS thất bại. Vui lòng kiểm tra lại kết nối mạng!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi kết nối cơ sở dữ liệu: {ex.Message}");
            }
        }

        private void BtnVerifyOtp_Click(object sender, RoutedEventArgs e)
        {
            string enteredOtp = txtOtp.Text.Trim();

            if (string.IsNullOrEmpty(enteredOtp))
            {
                ShowErrorMessage("Vui lòng nhập mã OTP!");
                return;
            }

            if (DateTime.Now > _otpExpiry)
            {
                ShowErrorMessage("Mã OTP đã hết hiệu lực (quá 3 phút). Vui lòng gửi lại!");
                return;
            }

            if (enteredOtp == _generatedOtp)
            {
                ShowSuccessMessage("Xác thực OTP thành công!");
                panelStep2.Visibility = Visibility.Collapsed;
                panelStep3.Visibility = Visibility.Visible;
            }
            else
            {
                ShowErrorMessage("Mã OTP không chính xác. Vui lòng nhập lại!");
            }
        }

        private void BtnBackToStep1_Click(object sender, RoutedEventArgs e)
        {
            panelStep2.Visibility = Visibility.Collapsed;
            panelStep1.Visibility = Visibility.Visible;
            lblMessage.Text = "";
        }

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = txtNewPassword.Password.Trim();
            string confirmPassword = txtConfirmPassword.Password.Trim();

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowErrorMessage("Vui lòng nhập đầy đủ mật khẩu mới!");
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowErrorMessage("Mật khẩu xác nhận không khớp!");
                return;
            }

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    var user = db.NguoiDungs.Find(_targetUserId);
                    if (user != null)
                    {
                        // Băm mật khẩu mới bằng SHA-256
                        user.MatKhau = SecurityHelper.HashPassword(newPassword);

                        // Lưu nhật ký
                        var log = new NhatKyHoatDong
                        {
                            NguoiDungId = user.Id,
                            HanhDong = "Khôi phục mật khẩu",
                            ThoiGian = DateTime.Now,
                            ChiTiet = $"Tài khoản {user.TenDangNhap} tự khôi phục mật khẩu thành công qua SMS OTP"
                        };
                        db.NhatKyHoatDongs.Add(log);

                        db.SaveChanges();

                        MessageBox.Show("Đặt lại mật khẩu thành công! Vui lòng đăng nhập bằng mật khẩu mới.", 
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        ShowErrorMessage("Không tìm thấy tài khoản để cập nhật!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi lưu mật khẩu mới: {ex.Message}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            lblMessage.Foreground = Brushes.Red;
            lblMessage.Text = message;
        }

        private void ShowSuccessMessage(string message)
        {
            lblMessage.Foreground = Brushes.Green;
            lblMessage.Text = message;
        }
    }
}
