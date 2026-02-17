using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Extensions;
using ORG.Files.Security;
using System.IO.Compression;
using System.Security.Cryptography;

public static class ExcelFromBase64
{
    private static byte[] _key;
    private static void  initial()
    {
        if(_key==null){
            _key = AesFileCrypto.GenerateRandomKey();
            AesFileCrypto.Initialize(_key);
        }
    }
    public static byte[] SaveBase64ExcelToFile(string base64, string outputPath)
    {
        initial();
        if (string.IsNullOrWhiteSpace(base64)) return null;
  
        // حذف پیشوند data URI در صورت وجود
        int comma = base64.IndexOf(',');
        if (comma >= 0) base64 = base64.Substring(comma + 1);

        base64 = base64.Trim();

        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            byte[] bytesEnc = AesFileCrypto.Instance.Encrypt(bytes);
            // اگر می‌دانید فرمت .xlsx است از همین استفاده کنید
            File.WriteAllBytes(outputPath, bytesEnc);
            return _key;
        }
        catch (FormatException)
        {
            // رشته Base64 نامعتبر است
            return null;
        }
        catch (IOException)
        {
            // خطای IO هنگام نوشتن فایل
            return null;
        }
    }

    public static FileContentResult SendDecryptedExcelFile(string encryptedFilePath, byte[] keyBytes, string downloadFileName = "file.xlsx")
    {
        initial();
        if (string.IsNullOrWhiteSpace(encryptedFilePath))
            return null;

        if (!File.Exists(encryptedFilePath))
            return null;

        if (keyBytes == null || keyBytes.Length == 0)
            return null;

       
        const int ExpectedKeyLength = 16;
        if (keyBytes.Length != ExpectedKeyLength)
            return null;

        byte[] encryptedBytes;
        try
        {
            encryptedBytes = File.ReadAllBytes(encryptedFilePath);
        }
        catch (Exception ex)
        {
            return null;
        }

        byte[] plaintext;
        try
        {
            // استفاده از AesFileCrypto موجود (نسخه‌ای که ctor byte[] می‌پذیرد)
            //AesFileCrypto.Initialize(keyBytes); 
            plaintext = AesFileCrypto.Instance.Decrypt(encryptedBytes, keyBytes);
        }
        catch (CryptographicException)
        {
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }

        // تعیین content-type (تشخیص از بایت‌ها یا استفاده از پسوند پیشنهادی)
        string contentType = GetContentTypeFromBytes(plaintext, downloadFileName);

        return new FileContentResult(plaintext, contentType)
        {
            FileDownloadName = downloadFileName
        };
    }

    private static string GetContentTypeFromBytes(byte[] bytes, string fallbackName)
    {
        if (IsXls(bytes)) return "application/vnd.ms-excel";
        if (IsZip(bytes) && IsXlsx(bytes)) return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        var ext = Path.GetExtension(fallbackName ?? string.Empty).ToLowerInvariant();
        return ext switch
        {
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".csv" => "text/csv",
            _ => "application/octet-stream"
        };
    }

    private static bool IsXls(byte[] b)
    {
        byte[] header = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
        if (b == null || b.Length < header.Length) return false;
        for (int i = 0; i < header.Length; i++) if (b[i] != header[i]) return false;
        return true;
    }

    private static bool IsZip(byte[] b)
    {
        if (b == null || b.Length < 4) return false;
        return b[0] == 0x50 && b[1] == 0x4B && (b[2] == 0x03 || b[2] == 0x05 || b[2] == 0x07) && (b[3] == 0x04 || b[3] == 0x06 || b[3] == 0x08);
    }

    private static bool IsXlsx(byte[] b)
    {
        try
        {
            using (var ms = new MemoryStream(b))
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: false))
            {
                var entries = archive.Entries.Select(e => e.FullName).ToList();
                var required = new[] { "[Content_Types].xml", "xl/workbook.xml" };
                return required.All(r => entries.Contains(r));
            }
        }
        catch
        {
            return false;
        }
    }
}
