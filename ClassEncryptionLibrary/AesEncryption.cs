namespace ClassEncryptionLibrary
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;

    public interface IEncryptionService
    {
        Task<string> Encrypt(string plainText);
        Task<string> Decrypt(string cipherText);
    }

    public class AesEncryption : IEncryptionService
    {
        private string _keyStr;
        private string _ivStr;
        private   byte[] _key; // Store this securely!
        private   byte[] _iv;  // IV should typically be unique per encryption, but for simplicity here, we'll fix it.
                                       // In real-world AES, generate a new IV for each encryption and prepend it to the ciphertext.
        //private readonly FileAesDecryptor fileAesDecryptor ;
        public AesEncryption(string keyStr,string ivStr)
        {
            //fileAesDecryptor = new FileAesDecryptor(nameFile);
            _keyStr = keyStr;
            _ivStr = ivStr;
        }

        private async Task initialKeyIvFile()
        {
            //string[] keys=await  fileAesDecryptor.decrypteCipherFile();
            if (_keyStr==null)
            {
                return;
            }
            // For pro duction, retrieve key and IV from a secure source (e.g., Azure Key Vault, environment variables).
            // DO NOT hardcode them like this.
            //_key = Encoding.UTF8.GetBytes(_keyStr);
            //_iv = Encoding.UTF8.GetBytes(_ivStr);
            _key = hexToBytes(_keyStr);
                _iv = hexToBytes(_ivStr);
            // Ensure key and IV are correct lengths for AES (e.g., 16, 24, or 32 bytes for key; 16 bytes for IV)
            if (_key.Length != 32 || _iv.Length != 16) // AES-256 requires 32-byte key, 16-byte IV
            {
                throw new ArgumentException("Key must be 32 bytes and IV must be 16 bytes for AES-256.");
            }
        }
        public  byte[] hexToBytes(string hex) { 
            if (hex.Length % 2 == 1) throw new ArgumentException("Invalid hex string length."); 
            byte[] bytes = new byte[hex.Length / 2]; 
            for (int i = 0; i < hex.Length; i += 2) 
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16); 
            return bytes; 
        }
        public async Task<string> Encrypt(string plainText)
        {
            await initialKeyIvFile();
            if (_key == null || _key.Length == 0)
                return "";

            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = _key.Length * 8;
                aesAlg.Key = _key;
                aesAlg.IV = _iv; // In a more robust solution, generate a new IV and prepend it to the ciphertext.

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public async Task<string> Decrypt(string cipherText)
        {
            await initialKeyIvFile();
            if (_key == null || _key.Length == 0)
                return "";

            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv; // If IV is prepended, extract it here.

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}