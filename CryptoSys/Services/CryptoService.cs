using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoAPI.Services
{
    public class CryptoService
    {
        // AES Encryption
        public string EncryptAES(string text, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = new byte[16];

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // AES Decryption
        public string DecryptAES(string encryptedText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        // RSA Key Generation
        public (string publicKey, string privateKey) GenerateRSAKeys()
        {
            using (RSA rsa = RSA.Create(2048)) // 2048-bit anahtar uzunluğu
            {
                try
                {
                    string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                    string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                    return (publicKey, privateKey);
                }
                catch (Exception ex)
                {
                    throw new CryptographicException("RSA anahtar çifti oluşturulamadı: " + ex.Message);
                }
            }
        }

        // RSA Encryption
        public string EncryptRSA(string text, string publicKey)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Şifrelenecek metin boş olamaz.");

            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("Public key boş olamaz.");

            using (RSA rsa = RSA.Create())
            {
                try
                {
                    byte[] publicKeyBytes = Convert.FromBase64String(publicKey);
                    rsa.ImportRSAPublicKey(publicKeyBytes, out _);

                    byte[] data = Encoding.UTF8.GetBytes(text);
                    byte[] encryptedData = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
                    return Convert.ToBase64String(encryptedData);
                }
                catch (FormatException)
                {
                    throw new CryptographicException("Geçersiz public key formatı.");
                }
                catch (CryptographicException ex)
                {
                    throw new CryptographicException("RSA şifreleme hatası: " + ex.Message);
                }
            }
        }

        // RSA Decryption
        public string DecryptRSA(string encryptedText, string privateKey)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentException("Çözülecek metin boş olamaz.");

            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentException("Private key boş olamaz.");

            using (RSA rsa = RSA.Create())
            {
                try
                {
                    byte[] privateKeyBytes = Convert.FromBase64String(privateKey);
                    rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

                    byte[] encryptedData = Convert.FromBase64String(encryptedText);
                    byte[] decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
                    return Encoding.UTF8.GetString(decryptedData);
                }
                catch (FormatException)
                {
                    throw new CryptographicException("Geçersiz private key formatı.");
                }
                catch (CryptographicException ex)
                {
                    throw new CryptographicException("RSA çözme hatası: " + ex.Message);
                }
            }
        }

        // SHA256 Hash
        public string ComputeSHA256(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Hash'lenecek metin boş olamaz.");

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        // SHA256 File Hash
        public async Task<string> ComputeFileSHA256(Stream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentException("Dosya akışı boş olamaz.");

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = await sha256.ComputeHashAsync(fileStream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
} 