using Microsoft.AspNetCore.Mvc;
using CryptoAPI.Services;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System;
using System.Security.Cryptography;

namespace CryptoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoController : ControllerBase
    {
        private readonly CryptoService _cryptoService;

        public CryptoController(CryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpPost("aes/encrypt")]
        public IActionResult EncryptAES([FromBody] AESRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.Key))
                {
                    return BadRequest(new { message = "Metin ve anahtar boş olamaz!" });
                }

                var result = _cryptoService.EncryptAES(request.Text, request.Key);
                return Ok(new { encryptedText = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Şifreleme hatası: {ex.Message}" });
            }
        }

        [HttpPost("aes/decrypt")]
        public IActionResult DecryptAES([FromBody] AESRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.Key))
                {
                    return BadRequest(new { message = "Metin ve anahtar boş olamaz!" });
                }

                var result = _cryptoService.DecryptAES(request.Text, request.Key);
                return Ok(new { decryptedText = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Çözme hatası: {ex.Message}" });
            }
        }

        [HttpPost("rsa/generate-keys")]
        public IActionResult GenerateRSAKeys()
        {
            try
            {
                var (publicKey, privateKey) = _cryptoService.GenerateRSAKeys();
                return Ok(new { publicKey, privateKey });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Anahtar oluşturma hatası: {ex.Message}" });
            }
        }

        [HttpPost("rsa/encrypt-text")]
        public IActionResult EncryptRSA([FromBody] RSAEncryptRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Text))
                {
                    return BadRequest(new { message = "Şifrelenecek metin boş olamaz!" });
                }

                if (string.IsNullOrEmpty(request.PublicKey))
                {
                    return BadRequest(new { message = "Public key boş olamaz!" });
                }

                var result = _cryptoService.EncryptRSA(request.Text, request.PublicKey);
                return Ok(new { encryptedText = result });
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Geçersiz public key formatı!" });
            }
            catch (CryptographicException)
            {
                return BadRequest(new { message = "Geçersiz public key!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Şifreleme hatası: {ex.Message}" });
            }
        }

        [HttpPost("rsa/decrypt-text")]
        public IActionResult DecryptRSA([FromBody] RSADecryptRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Text))
                {
                    return BadRequest(new { message = "Çözülecek metin boş olamaz!" });
                }

                if (string.IsNullOrEmpty(request.PrivateKey))
                {
                    return BadRequest(new { message = "Private key boş olamaz!" });
                }

                var result = _cryptoService.DecryptRSA(request.Text, request.PrivateKey);
                return Ok(new { decryptedText = result });
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Geçersiz private key formatı!" });
            }
            catch (CryptographicException)
            {
                return BadRequest(new { message = "Geçersiz private key!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Çözme hatası: {ex.Message}" });
            }
        }

        [HttpPost("sha256/text")]
        public IActionResult ComputeTextHash([FromBody] HashRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Text))
                {
                    return BadRequest(new { message = "Hash'lenecek metin boş olamaz!" });
                }

                var result = _cryptoService.ComputeSHA256(request.Text);
                return Ok(new { hash = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Hash hesaplama hatası: {ex.Message}" });
            }
        }

        [HttpPost("sha256/file")]
        public async Task<IActionResult> ComputeFileHash(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Dosya yüklenmedi." });

                using var stream = file.OpenReadStream();
                var result = await _cryptoService.ComputeFileSHA256(stream);
                return Ok(new { hash = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Hash hesaplama hatası: {ex.Message}" });
            }
        }
    }

    public class AESRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }
    }

    public class RSAEncryptRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }
    }

    public class RSADecryptRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("privateKey")]
        public string PrivateKey { get; set; }
    }

    public class HashRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
} 