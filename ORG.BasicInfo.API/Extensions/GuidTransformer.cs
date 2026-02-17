using System;
using System.Numerics;

public static class GuidTransformer
{
    // ثابت 2^128 برای مدولو
    private static readonly BigInteger Mod128 = BigInteger.One << 128;

    // تبدیل Guid به BigInteger (بدون علامت)
    public static BigInteger GuidToBigInteger(Guid guid)
    {
        byte[] bytes = guid.ToByteArray(); // 16 بایت
        byte[] unsigned = new byte[17];    // یک بایت اضافی برای جلوگیری از تفسیر علامت‌دار
        Array.Copy(bytes, 0, unsigned, 0, 16);
        unsigned[16] = 0;                  // بایت افزوده برابر صفر => مقدار بدون علامت
        return new BigInteger(unsigned);   // BigInteger از آرایه بایت little-endian می‌سازد
    }

    // تبدیل BigInteger به Guid (فرض: مقدار داخل بازه 0..2^128-1)
    public static Guid BigIntegerToGuid(BigInteger value)
    {
        // اطمینان از بازه با مدولو 2^128
        value = (value % Mod128 + Mod128) % Mod128;

        byte[] unsigned = value.ToByteArray(); // ممکن است طول < 16 یا >16 باشد
        byte[] bytes = new byte[16];

        // کپی حداقل 16 بایت (little-endian). اگر کمتر باشد، بقیه صفر خواهند بود.
        int len = Math.Min(unsigned.Length, 16);
        Array.Copy(unsigned, 0, bytes, 0, len);

        return new Guid(bytes);
    }

    // نمونه تابعی که فرمول 8*(n*5+45) را اعمال می‌کند
    public static Guid TransformGuid(Guid input)
    {
        BigInteger n = GuidToBigInteger(input);
        BigInteger transformed = 8 * (n * 5 + 45);
        transformed %= Mod128;
        return BigIntegerToGuid(transformed);
    }
}
