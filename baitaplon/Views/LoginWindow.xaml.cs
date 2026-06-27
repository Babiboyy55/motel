using baitaplon.Models;
using Quanlynhatro.Views.Admin;
using Quanlynhatro.ViewModels.Admin;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Quanlynhatro.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoadRememberedUser();
        }

        private void LoadRememberedUser()
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "remember.txt");
                if (System.IO.File.Exists(path))
                {
                    string username = System.IO.File.ReadAllText(path).Trim();
                    if (!string.IsNullOrEmpty(username))
                    {
                        txtUsername.Text = username;
                        chkRememberMe.IsChecked = true;
                    }
                }
            }
            catch { }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            ThucHienDangNhap();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ThucHienDangNhap();
            }
        }

        private void HlForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var forgotWindow = new ForgotPasswordWindow();
            forgotWindow.ShowDialog();
        }

        private void ThucHienDangNhap()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
                return;
            }

            try
            {
                using (var db = new NhaTroDbContext())
                {
                    // Tìm tài khoản theo tên đăng nhập trong DB
                    var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == username);

                    if (user != null && baitaplon.Services.SecurityHelper.VerifyPassword(password, user.MatKhau))
                    {
                        if (!user.TrangThai)
                        {
                            lblError.Text = "Tài khoản của bạn đã bị khóa!";
                            return;
                        }

                        // Lưu trạng thái Ghi nhớ tài khoản
                        try
                        {
                            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "remember.txt");
                            if (chkRememberMe.IsChecked == true)
                            {
                                System.IO.File.WriteAllText(path, username);
                            }
                            else
                            {
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                            }
                        }
                        catch { }

                        // Ghi nhận nhật ký đăng nhập thành công
                        var nhatKy = new NhatKyHoatDong
                        {
                            NguoiDungId = user.Id,
                            HanhDong = "Đăng nhập",
                            ThoiGian = DateTime.Now,
                            ChiTiet = $"Đăng nhập thành công với vai trò {user.VaiTro}"
                        };
                        db.NhatKyHoatDongs.Add(nhatKy);
                        db.SaveChanges();

                        // Khởi tạo AdminWindow và truyền DataContext có thông tin user
                        var adminWindow = new AdminWindow();
                        adminWindow.DataContext = new AdminMainViewModel(user);

                        adminWindow.Show();
                        this.Close(); // Đóng cửa sổ Đăng nhập
                    }
                    else
                    {
                        lblError.Text = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Lỗi kết nối cơ sở dữ liệu: {ex.Message}";
            }
        }
    }
}
