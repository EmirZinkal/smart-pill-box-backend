

namespace Core.Utilities.Security.Jwt
{
    public interface ITokenHelper
    {
        /// Gerekli kullanıcı bilgileri ile bir token oluşturur.
        AccessToken CreateToken(int userId, string email, string fullName);
    }
}