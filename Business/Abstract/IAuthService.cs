using Core.Utilities.Results;
using Core.Utilities.Security.Jwt; // AccessToken
using Entities.Concrete;          // User
using Entities.Dtos;              // DTOs

namespace Business.Abstract
{
    public interface IAuthService
    {
        /// Yeni kullanıcı kayıt işlemi. DTO alır, User nesnesi döner.
        IDataResult<User> Register(UserForRegisterDto userForRegisterDto);

        /// Kullanıcı giriş işlemi. DTO'yu doğrular, User nesnesi döner.
        /// (Token üretmez, sadece doğrular).
        IDataResult<User> Login(UserForLoginDto userForLoginDto);

        /// E-postanın zaten kayıtlı olup olmadığını kontrol eder.
        IResult UserExists(string email);

        /// Doğrulanmış bir 'User' nesnesini alır ve ona bir AccessToken üretir.
        IDataResult<AccessToken> CreateAccessToken(User user);

        // Not: Şifre sıfırlama (Password Reset) metotlarını
        // 'PasswordResetToken' entity'si oluşturduktan sonra buraya ekleyebiliriz.
    }
}