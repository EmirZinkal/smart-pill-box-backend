using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        // Dependency Injection ile Business katmanındaki servisimizi çağırıyoruz.
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// Tüm kullanıcıları listeler.
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _userService.GetAll();
            if (result.Success)
            {
                // Başarılıysa, 200 OK (Başarılı) durum kodu ile birlikte
                // veriyi (result.Data) ve mesajı (result.Message) döndürür.
                return Ok(result);
            }

            // Başarısızsa, 400 Bad Request (Kötü İstek) durum kodu ile
            // hata mesajını (result.Message) döndürür.
            return BadRequest(result);
        }

        /// ID'ye göre tek bir kullanıcıyı getirir.
        /// <param name="id"></param>
        [HttpGet("getbyid")]
        public IActionResult GetById(int id)
        {
            var result = _userService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result); // Kullanıcı bulunamazsa 404 de döndürülebilir,
                                       // ama Core.Results yapınız 400'e daha uygun.
        }

        /// E-posta adresine göre tek bir kullanıcıyı getirir.
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("getbyemail")]
        public IActionResult GetByEmail(string email)
        {
            var result = _userService.GetByEmail(email);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// Yeni bir kullanıcı ekler (Register - Kayıt olma).
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public IActionResult Add(User user)
        {
            // Not: Gerçek bir senaryoda burada 'user' yerine
            // 'UserForRegisterDto' gibi bir DTO (Data Transfer Object) kullanılır
            // ve parola hash'leme işlemi bu katmanda (veya Business'ta) yapılır.
            // Şimdilik basit tutuyoruz.

            var result = _userService.Add(user);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result); // Örn: E-posta zaten varsa burası çalışır.
        }

        /// Bir kullanıcının bilgilerini günceller.
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public IActionResult Update(User user)
        {
            var result = _userService.Update(user);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// ID'si verilen bir kullanıcıyı siler.
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            // Repository desenimiz (IEntityRepository) Delete işlemi için tüm
            // entity'i istiyor. Bu nedenle önce entity'i bulmalıyız.
            var userResult = _userService.GetById(id);
            if (!userResult.Success)
            {
                return BadRequest(userResult); // Silinecek kullanıcı bulunamadı.
            }

            // Kullanıcıyı bulduysak, silme işlemini yap.
            var deleteResult = _userService.Delete(userResult.Data);
            if (deleteResult.Success)
            {
                return Ok(deleteResult);
            }
            return BadRequest(deleteResult);
        }
    }
}