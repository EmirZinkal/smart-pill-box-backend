using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Core.Utilities.Security.Hashing
{
    public class HashingHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("marketim_static_key"); // 16-32 byte arası olmalı
        public static void CreatePasswordHash(string password, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512(Key))
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash)
        {
            using (var hmac = new HMACSHA512(Key))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }


        // ==========================================
        // MESAJ: Çift yönlü şifreleme (AES-GCM, AEAD)
        // - key: 32 byte (AES-256) olmalı
        // - çıktı formatı: base64(nonce) : base64(cipher) : base64(tag)
        // ==========================================

        /// <summary>
        /// Metni AES-GCM ile şifreler. Dönen format: "nonce:cipher:tag" (base64)
        /// </summary>
        public static string EncryptMessage(string plaintext, byte[] key)
        {
            // Temel kontroller
            if (key == null || key.Length != 32)
                throw new ArgumentException("Crypto key 32 byte (AES-256) olmalı.", nameof(key));
            if (plaintext == null)
                plaintext = string.Empty;

            // GCM için önerilen nonce (IV) uzunluğu 12 byte
            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] cipher = new byte[plainBytes.Length];
            byte[] tag = new byte[16]; // GCM auth tag 16 byte

            // AES-GCM şifreleme (AEAD: bütünlük + gizlilik)
            using (var aes = new AesGcm(key))
            {
                // AAD (ek veri) gerekliyse 5. parametreye byte[] verebilirsin; burada kullanmıyoruz (null).
                aes.Encrypt(nonce, plainBytes, cipher, tag);
            }

            // Nonce, cipher ve tag’i base64’e çevirip tek stringte birleştiriyoruz
            return $"{Convert.ToBase64String(nonce)}:{Convert.ToBase64String(cipher)}:{Convert.ToBase64String(tag)}";
        }

        /// <summary>
        /// "nonce:cipher:tag" (base64) formatındaki metni AES-GCM ile çözer.
        /// </summary>
        public static string DecryptMessage(string combined, byte[] key)
        {
            if (key == null || key.Length != 32)
                throw new ArgumentException("Crypto key 32 byte (AES-256) olmalı.", nameof(key));
            if (string.IsNullOrWhiteSpace(combined))
                return string.Empty;

            var parts = combined.Split(':');
            if (parts.Length != 3)
                throw new CryptographicException("Şifreli veri formatı geçersiz (nonce:cipher:tag bekleniyor).");

            byte[] nonce = Convert.FromBase64String(parts[0]);
            byte[] cipher = Convert.FromBase64String(parts[1]);
            byte[] tag = Convert.FromBase64String(parts[2]);

            byte[] plain = new byte[cipher.Length];

            using (var aes = new AesGcm(key))
            {
                aes.Decrypt(nonce, cipher, tag, plain);
            }

            return Encoding.UTF8.GetString(plain);
        }
    }
}
