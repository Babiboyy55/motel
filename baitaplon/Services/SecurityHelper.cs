using System;
using System.Security.Cryptography;
using System.Text;

namespace baitaplon.Services
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash)) return false;
            string hashedInput = HashPassword(password);
            return string.Equals(hashedInput, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
