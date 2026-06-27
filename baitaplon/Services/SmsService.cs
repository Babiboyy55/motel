using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace baitaplon.Services
{
    public static class SmsService
    {
        // Cấu hình tài khoản Twilio (Điền thông tin thật của bạn vào đây nếu muốn gửi tin nhắn thực tế)
        private static readonly string AccountSid = ""; // Ví dụ: "ACxxxxxxxxxxxxxxxxxxxxxxxx"
        private static readonly string AuthToken = "";  // Ví dụ: "xxxxxxxxxxxxxxxxxxxxxxxx"
        private static readonly string FromPhone = "";  // Số điện thoại Twilio cấp, ví dụ: "+1234567890"

        public static bool SendSms(string toPhone, string message)
        {
            if (string.IsNullOrWhiteSpace(toPhone)) return false;

            // Chuẩn hóa số điện thoại Việt Nam sang định dạng quốc tế (+84)
            string formattedPhone = toPhone.Trim();
            if (formattedPhone.StartsWith("0"))
            {
                formattedPhone = "+84" + formattedPhone.Substring(1);
            }

            // Nếu không có thông tin cấu hình Twilio, tự động chạy chế độ GIẢ LẬP (Hiện hộp thoại thông báo tin nhắn và mã OTP)
            if (string.IsNullOrEmpty(AccountSid) || string.IsNullOrEmpty(AuthToken) || string.IsNullOrEmpty(FromPhone))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        $"[GIẢ LẬP SMS GATEWAY]\n" +
                        $"Đang gửi SMS tới số: {toPhone} ({formattedPhone})\n" +
                        $"Nội dung: \"{message}\"\n\n" +
                        $"(Lưu ý: Để gửi SMS thật về điện thoại, hãy cấu hình tài khoản Twilio trong file Services\\SmsService.cs)",
                        "Trình giả lập SMS", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                });
                return true;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{AccountSid}:{AuthToken}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("To", formattedPhone),
                        new KeyValuePair<string, string>("From", FromPhone),
                        new KeyValuePair<string, string>("Body", message)
                    });

                    string url = $"https://api.twilio.com/2010-04-01/Accounts/{AccountSid}/Messages.json";
                    var response = client.PostAsync(url, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        System.Diagnostics.Debug.WriteLine($"Lỗi gửi SMS Twilio: {responseString}");
                        
                        // Fallback sang giả lập nếu API trả về lỗi (hết tiền, số chưa verify,...)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Twilio API báo lỗi (Có thể tài khoản hết hạn/chưa xác thực). Chuyển sang hiển thị mã OTP giả lập:\n\n{message}", 
                                "Thông báo SMS", MessageBoxButton.OK, MessageBoxImage.Warning);
                        });
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi kết nối SMS: {ex.Message}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Lỗi kết nối mạng SMS. Chuyển sang hiển thị mã OTP giả lập:\n\n{message}", 
                        "Thông báo SMS", MessageBoxButton.OK, MessageBoxImage.Warning);
                });
                return true;
            }
        }
    }
}
