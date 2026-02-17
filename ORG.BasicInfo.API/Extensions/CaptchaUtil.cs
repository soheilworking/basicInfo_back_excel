using System.Security.Cryptography;
using System.Text;

public static class CaptchaUtil
{
    private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // حذف حروف گیج‌کننده
    public static string GenerateCode(int length = 6)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
        var sb = new StringBuilder(length);
        var buf = new byte[length];
        RandomNumberGenerator.Fill(buf);
        for (int i = 0; i < length; i++)
        {
            var idx = buf[i] % Chars.Length;
            sb.Append(Chars[idx]);
        }
        return sb.ToString();
    }
}
