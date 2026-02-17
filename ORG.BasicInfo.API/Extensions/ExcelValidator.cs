using System.IO.Compression;
namespace ORG.BasicInfo.API.Extensions;
// نتیجه بررسی
public enum ExcelCheckResult
{
    NotBase64,
    NotExcel,
    ExcelXlsx,
    ExcelXls,
    ExcelValidOpenXml // یعنی قابل پارس با OpenXML/EPPlus
}

public static class ExcelValidator
{
    // ورودی: رشته Base64 (ممکن است data URI)
    // خروجی: نتیجه از نوع ExcelCheckResult
    public static ExcelCheckResult ValidateBase64IsExcel(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return ExcelCheckResult.NotBase64;

        // حذف پیشوند data URI در صورت وجود
        int comma = base64.IndexOf(',');
        if (comma >= 0) base64 = base64.Substring(comma + 1);
        base64 = base64.Trim();

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            return ExcelCheckResult.NotBase64;
        }

        // بررسی magic bytes برای XLS (BIFF compound file) و XLSX (ZIP/PK)
        if (IsXls(bytes)) return ExcelCheckResult.ExcelXls;
        if (IsZip(bytes))
        {
            // اگر ZIP است، احتمالا XLSX یا دیگر فرمت‌های مبتنی بر OOXML
            if (IsXlsx(bytes)) return ExcelCheckResult.ExcelXlsx;
            // ZIP ولی محتویات مورد انتظار xlsx را ندارد
            return ExcelCheckResult.NotExcel;
        }

        return ExcelCheckResult.NotExcel;
    }

    // شناسایی پرونده XLS (BIFF Compound File) — header: D0 CF 11 E0 A1 B1 1A E1
    private static bool IsXls(byte[] b)
    {
        byte[] header = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
        if (b.Length < header.Length) return false;
        for (int i = 0; i < header.Length; i++)
            if (b[i] != header[i]) return false;
        return true;
    }

    // شناسایی ZIP (header: PK 03 04)
    private static bool IsZip(byte[] b)
    {
        if (b.Length < 4) return false;
        return b[0] == 0x50 && b[1] == 0x4B && (b[2] == 0x03 || b[2] == 0x05 || b[2] == 0x07) && (b[3] == 0x04 || b[3] == 0x06 || b[3] == 0x08);
    }

    // بررسی اینکه ZIP حاوی ساختار OpenXML مربوط به XLSX است
    private static bool IsXlsx(byte[] b)
    {
        try
        {
            using (var ms = new MemoryStream(b))
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: false))
            {
                // فایل‌های کلیدی که در فایل xlsx وجود دارند
                var required = new[] { "[Content_Types].xml", "xl/workbook.xml" };
                var entries = archive.Entries.Select(e => e.FullName).ToList();
                // بررسی وجود فایل‌های کلیدی
                return required.All(r => entries.Contains(r));
            }
        }
        catch
        {
            return false;
        }
    }
}
