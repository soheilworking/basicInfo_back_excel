using ORG.Files.Security;
using System;
using System.Security.Cryptography;

namespace ORG.Files.Security
{
    using System;
    using System.Security.Cryptography;

    public sealed class AesFileCrypto
    {
        private const int KeySizeBytes = 16;   // 128 bit
        private const int NonceSizeBytes = 12; // توصیه شده برای GCM
        private const int TagSizeBytes = 16;   // 128 bit auth tag

        private readonly byte[] _key;

        // Singleton backing field
        private static AesFileCrypto? _instance;
        private static readonly object _initLock = new object();

        // Private ctor
        private AesFileCrypto(byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (key.Length != KeySizeBytes) throw new ArgumentException($"Key must be {KeySizeBytes} bytes long", nameof(key));
            _key = new byte[KeySizeBytes];
            Buffer.BlockCopy(key, 0, _key, 0, KeySizeBytes);
        }

        // Initialize singleton once. Throws if called multiple times with a different key.
        public static void Initialize(byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (_instance != null) // already initialized
            {
                // Optionally check that provided key matches existing
                if (!AreEqual(key, _instance._key))
                    throw new InvalidOperationException("AesFileCrypto is already initialized with a different key.");
                return;
            }

            lock (_initLock)
            {
                if (_instance == null)
                {
                    // make a defensive copy of key
                    var keyCopy = new byte[key.Length];
                    Buffer.BlockCopy(key, 0, keyCopy, 0, key.Length);
                    _instance = new AesFileCrypto(keyCopy);
                }
                else
                {
                    if (!AreEqual(key, _instance._key))
                        throw new InvalidOperationException("AesFileCrypto is already initialized with a different key.");
                }
            }
        }

        // Singleton accessor (throws if not initialized)
        public static AesFileCrypto Instance
        {
            get
            {
                if (_instance == null) throw new InvalidOperationException("AesFileCrypto is not initialized. Call Initialize(key) first.");
                return _instance;
            }
        }

        // Optional helper to check equality securely (constant-time-ish)
        private static bool AreEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }

        // Rest of the methods unchanged
        public byte[] Encrypt(byte[] plaintext)
        {
            if (plaintext == null) throw new ArgumentNullException(nameof(plaintext));

            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSizeBytes);
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[TagSizeBytes];

            using (var aesGcm = new AesGcm(_key))
            {
                aesGcm.Encrypt(nonce, plaintext, ciphertext, tag, null);
            }

            byte[] output = new byte[NonceSizeBytes + TagSizeBytes + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, output, 0, NonceSizeBytes);
            Buffer.BlockCopy(tag, 0, output, NonceSizeBytes, TagSizeBytes);
            Buffer.BlockCopy(ciphertext, 0, output, NonceSizeBytes + TagSizeBytes, ciphertext.Length);

            return output;
        }

        public byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            if (encrypted == null) throw new ArgumentNullException(nameof(encrypted));
            if (encrypted.Length < NonceSizeBytes + TagSizeBytes) throw new CryptographicException("Invalid encrypted data");

            byte[] nonce = new byte[NonceSizeBytes];
            byte[] tag = new byte[TagSizeBytes];
            int cipherStart = NonceSizeBytes + TagSizeBytes;
            int cipherLength = encrypted.Length - cipherStart;
            byte[] ciphertext = new byte[cipherLength];

            Buffer.BlockCopy(encrypted, 0, nonce, 0, NonceSizeBytes);
            Buffer.BlockCopy(encrypted, NonceSizeBytes, tag, 0, TagSizeBytes);
            Buffer.BlockCopy(encrypted, cipherStart, ciphertext, 0, cipherLength);

            byte[] plaintext = new byte[cipherLength];

            using (var aesGcm = new AesGcm(key))
            {
                aesGcm.Decrypt(nonce, ciphertext, tag, plaintext, null);
            }

            return plaintext;
        }

        public static byte[] GenerateRandomKey()
        {
            return RandomNumberGenerator.GetBytes(KeySizeBytes);
        }
    }
}
//byte[] key = AesFileCrypto.GenerateRandomKey();
//var crypto = new AesFileCrypto(key);

//// رمزگذاری از آرایه بایت
//byte[] plain = File.ReadAllBytes("plain.pdf");
//byte[] encrypted = crypto.Encrypt(plain);
//File.WriteAllBytes("encrypted.dat", encrypted);

//// رمزگشایی به آرایه بایت
//byte[] loaded = File.ReadAllBytes("encrypted.dat");
//byte[] decrypted = crypto.Decrypt(loaded);
//File.WriteAllBytes("decrypted.pdf", decrypted);