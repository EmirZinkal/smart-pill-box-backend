using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing; // Static HashingHelper'ı kullanmak için
using Core.Utilities.Security.Jwt;    // ITokenHelper
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;

        // Bağımlılıklar (DI): Sadece User servisi ve Token yardımcısı
        // HashingHelper static olduğu için enjekte edilmesine gerek yok.
        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        /// Kullanıcı Kayıt İşlemi
        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto)
        {
            // 1. E-posta adresi zaten kullanımda mı?
            var userExistsResult = UserExists(userForRegisterDto.Email);
            if (!userExistsResult.Success)
            {
                return new ErrorDataResult<User>(userExistsResult.Message);
            }

            // 2. Şifreyi HASH'le (Senin HashingHelper'ını kullanarak)
            // HashingHelper'ın 'out byte[]' döndürüyor.
            HashingHelper.CreatePasswordHash(userForRegisterDto.Password, out byte[] passwordHashBytes);

            // 3. Veritabanındaki 'string PasswordHash' alanına
            // 'byte[]' dizisini Base64 string olarak kaydediyoruz.
            var passwordHashString = Convert.ToBase64String(passwordHashBytes);

            // 4. Yeni User nesnesini oluştur
            var user = new User
            {
                Email = userForRegisterDto.Email,
                FullName = $"{userForRegisterDto.FirstName} {userForRegisterDto.LastName}",
                PasswordHash = passwordHashString, // Base64 string olarak atandı

                // 👇 KRİTİK EKLEME BURASI 👇
                // DTO'dan gelen rolü alıp User nesnesine atıyoruz.
                Role = userForRegisterDto.Role
            };

            // 5. Kullanıcıyı ekle
            _userService.Add(user);

            // 6. Rol atama (UserOperationClaim)
            // Bu projede rol sistemi olmadığı için bu adım atlanıyor.

            return new SuccessDataResult<User>(user, Messages.UserAdded);
        }

        /// Kullanıcı Giriş Doğrulama
        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            var userToCheckResult = _userService.GetByEmail(userForLoginDto.Email);
            if (!userToCheckResult.Success || userToCheckResult.Data == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }

            var user = userToCheckResult.Data;

            // 2. Veritabanındaki Base64 string HASH'i alıp 'byte[]' dizisine geri çevir
            byte[] storedHashBytes = Convert.FromBase64String(user.PasswordHash);

            // 3. Gelen düz metin şifre ile veritabanındaki HASH'i doğrula
            var passwordMatch = HashingHelper.VerifyPasswordHash(
                userForLoginDto.Password,
                storedHashBytes
            );

            if (!passwordMatch)
            {
                return new ErrorDataResult<User>("Parola hatalı!"); // Messages'a eklenmeli
            }

            // 4. Giriş başarılı, LastLoginAt güncelle
            user.LastLoginAt = DateTime.UtcNow;
            _userService.Update(user);

            // Kullanıcı nesnesini (Token OLMADAN) geri döndür
            return new SuccessDataResult<User>(user, "Giriş başarılı.");
        }

        /// Token Üretme
        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            // 'ITokenHelper'ımızın ihtiyaç duyduğu bilgileri (rolsüz olarak) yolluyoruz
            var accessToken = _tokenHelper.CreateToken(
                user.Id,
                user.Email,
                user.FullName
            );

            return new SuccessDataResult<AccessToken>(accessToken, "Token oluşturuldu.");
        }

        /// Kullanıcı Varlığını Kontrol Et
        public IResult UserExists(string email)
        {
            var userResult = _userService.GetByEmail(email);
            if (userResult.Data != null)
            {
                return new ErrorResult(Messages.UserEmailAlreadyExists);
            }
            return new SuccessResult();
        }
    }
}