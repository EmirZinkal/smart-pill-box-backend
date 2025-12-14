using Business.Abstract;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        /// Kullanıcı Giriş (Login) endpoint'i
        [HttpPost("login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            // 1. Adım: Kullanıcıyı doğrula
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin); // Hata (örn: Parola yanlış)
            }

            // 2. Adım: Token üret
            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                // İŞTE BURASI: Flutter'a sadece token değil, ROLÜ de gönderiyoruz.
                return Ok(new
                {
                    token = result.Data.Token,
                    expiration = result.Data.Expiration,
                    role = userToLogin.Data.Role, // "Patient" veya "Relative" dönecek
                    userId = userToLogin.Data.Id
                });// Başarılı (Token ve mesaj döner)
            }

            return BadRequest(result);
        }

        /// Kullanıcı Kayıt (Register) endpoint'i
        [HttpPost("register")]
        public IActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            // 1. Adım: E-posta mevcut mu? (Register metodu zaten yapıyor)
            var registerResult = _authService.Register(userForRegisterDto);
            if (!registerResult.Success)
            {
                return BadRequest(registerResult); // Hata (örn: E-posta mevcut)
            }

            // 2. Adım: Kayıt başarılıysa, otomatik giriş yaptırıp token ver
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (result.Success)
            {
                // İPUCU: Burada da rolü dönebiliriz ki kayıt sonrası hemen doğru sayfaya gitsin
                return Ok(new
                {
                    token = result.Data.Token,
                    expiration = result.Data.Expiration,
                    role = registerResult.Data.Role, // Kayıt olan kişinin rolünü dönüyoruz
                    userId = registerResult.Data.Id
                }); // Başarılı (Token ve mesaj döner)
            }

            return BadRequest(result);
        }

        /// E-posta adresine göre Kullanıcı ID'sini döner.
        [HttpGet("getidbyemail")]
        public IActionResult GetIdByEmail(string email)
        {
            // Servisten gelen sonuç (IDataResult tipinde)
            var userResult = _userService.GetByEmail(email);

            // 1. İşlem başarılı mı kontrol et (user != null yerine .Success kullanılır)
            if (userResult.Success)
            {
                // 2. Asıl kullanıcı verisine .Data diyerek ulaş
                var user = userResult.Data;

                return Ok(new
                {
                    success = true,
                    data = user.Id,
                    name = user.FullName
                });
            }

            // Kullanıcı bulunamadıysa servisten gelen mesajı dön
            return BadRequest(new { success = false, message = userResult.Message });
        }
    }
}