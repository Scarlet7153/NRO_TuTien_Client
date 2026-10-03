using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Utility class để hash mật khẩu phía client trước khi gửi qua socket.
/// Dùng MD5 để hash plaintext, ngăn mật khẩu thuần truyền qua mạng.
/// Server Java sẽ nhận MD5-hash và BCrypt để lưu vào database.
/// </summary>
public static class PasswordUtil
{
    /// <summary>
    /// Hash mật khẩu bằng MD5 trước khi gửi lên server.
    /// </summary>
    /// <param name="plainPassword">Mật khẩu plaintext người dùng nhập</param>
    /// <returns>MD5 hex string (32 ký tự lowercase)</returns>
    public static string Md5Hash(string plainPassword)
    {
        if (string.IsNullOrEmpty(plainPassword))
            return string.Empty;

        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainPassword);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Kiểm tra xem chuỗi đã là MD5 hash chưa (32 ký tự hex).
    /// Dùng để tránh hash 2 lần khi load từ RMS.
    /// </summary>
    /// <param name="value">Chuỗi cần kiểm tra</param>
    /// <returns>true nếu đã là MD5 hash</returns>
    public static bool IsMd5Hash(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 32)
            return false;

        foreach (char c in value)
        {
            if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Chuẩn bị mật khẩu để gửi lên server: hash nếu chưa hash.
    /// </summary>
    /// <param name="password">Mật khẩu có thể là plaintext hoặc đã MD5</param>
    /// <returns>MD5 hash để gửi lên server</returns>
    public static string PrepareForSend(string password)
    {
        if (string.IsNullOrEmpty(password))
            return string.Empty;

        // Nếu đã là MD5 hash (load từ RMS) thì không hash lại
        if (IsMd5Hash(password))
            return password;

        return Md5Hash(password);
    }
}
