using System.Reflection;
using System.Security.Cryptography;
using System.Text;
namespace ORG.ServiceManagment.API.Extensions;
public class FileAesDecryptor
{
    // It's recommended to use a key size of 256 bits for strong encryption.
    private const int KeySize = 256;

    // The block size for AES is 128 bits.
    private const int BlockSize = 128;
    private static string decryptedString;

    // The number of iterations for the password-based key derivation.
    // A higher number increases security but also slows down the process.
    private const int DerivationIterations = 10000;
    public static async Task<string> decryptFile(string inputFile, string password)
    {
        // if (decryptedString == null) { // این شرط باعث می‌شود فقط یک بار فایل رمزگشایی شود
        if (string.IsNullOrEmpty(inputFile))
            throw new ArgumentNullException(nameof(inputFile));

        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        using (var inputStream = new FileStream(inputFile, FileMode.Open))
        {
            // Read the salt from the beginning of the encrypted file.
            var saltStringBytes = new byte[16];
            await inputStream.ReadAsync(saltStringBytes, 0, saltStringBytes.Length);

            // Create the key and IV from the password and salt.
            using (var passwordDeriveBytes = new Rfc2898DeriveBytes(password, saltStringBytes, DerivationIterations, HashAlgorithmName.SHA256))
            {
                var keyBytes = passwordDeriveBytes.GetBytes(KeySize / 8);
                var ivBytes = passwordDeriveBytes.GetBytes(BlockSize / 8);

                using (var aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
                        {
                            // decryptedString = await streamReader.ReadToEndAsync(); // تغییر به async
                            return await streamReader.ReadToEndAsync();
                        }
                    }
                }
            }
        }
        // }
        // return decryptedString;
    }

    //private static async Task<string> decryptFile(string inputFile, string password)
    //{
    //    if (decryptedString == null) { 
    //        if (string.IsNullOrEmpty(inputFile))
    //            throw new ArgumentNullException(nameof(inputFile));

    //        if (string.IsNullOrEmpty(password))
    //            throw new ArgumentNullException(nameof(password));

    //        var inputStream = new FileStream(inputFile, FileMode.Open);
    //        // Read the salt from the beginning of the encrypted file.
    //        var saltStringBytes = new byte[16];
    //        await inputStream.ReadAsync(saltStringBytes, 0, saltStringBytes.Length);

    //        // Create the key and IV from the password and salt.
    //        var passwordDeriveBytes = new Rfc2898DeriveBytes(password, saltStringBytes, DerivationIterations, HashAlgorithmName.SHA256);
    //        var keyBytes = passwordDeriveBytes.GetBytes(KeySize / 8);
    //        var ivBytes = passwordDeriveBytes.GetBytes(BlockSize / 8);

    //        var aes = Aes.Create();
    //        aes.Key = keyBytes;
    //        aes.IV = ivBytes;
    //        aes.Mode = CipherMode.CBC;
    //        aes.Padding = PaddingMode.PKCS7;

    //        // var cryptoStream = ;
    //        // var outputStream = new FileStream(outputFile, FileMode.Create);
    //        CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read);


    //        StreamReader streamReader = new StreamReader(cryptoStream);

    //        decryptedString = streamReader.ReadToEnd();
    //    }
    
    //  return decryptedString;
            
        
    //    //await cryptoStream.CopyToAsync(outputStream);
    //}


    public  async Task<string[]> DecrypteCipherFile(string inputFile,string password)
    {

        string encryptedFilePath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}{inputFile}";
        Console.WriteLine($"encryptedFilePath={encryptedFilePath}");
        if (File.Exists(encryptedFilePath))
        {
            
            if (password == "") return [];

            string decryptedText = await decryptFile(encryptedFilePath, password);
            return decryptedText.Split("---");

        }
        else
        {
            Console.WriteLine($"Error: '{encryptedFilePath}' not found. Please encrypt it with OpenSSL first.");
            Console.WriteLine("Example OpenSSL command: openssl aes-256-cbc -in plaintext.txt -out ciphertext.enc -k \"MySuperSecretPassword123\"");
            return ["error: not found file"];
        }

    }

    

}
public class PasswordFile
{
    public string NameFile { get; set; }
    public string Password { get; set; }
}
public class EncryptionKeysData
{
    public string Key { get; set; }
    public string Iv { get; set; }
}