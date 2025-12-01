
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; // Bu using satırı önemli
using Core.Extensions;
using Core.Utilities.Security.Encyption;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// using System.Linq; // Artık gerekmeyebilir

namespace Core.Utilities.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }
        private readonly TokenOptions _tokenOptions;

        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
        }

        // --- DEĞİŞİKLİK 1: Metot imzası ITokenHelper ile aynı (Rol listesi yok) ---
        public AccessToken CreateToken(int userId, string email, string fullName)
        {
            var now = DateTime.UtcNow;
            var accessTokenExpiration = now.AddMinutes(_tokenOptions.AccessTokenExpiration);

            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);

            // Verileri 'claims'e dönüştür (Roller olmadan)
            var claims = SetClaims(userId, email, fullName);

            var jwt = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                expires: accessTokenExpiration,
                notBefore: now,
                claims: claims, // Roller olmadan oluşturulan claims listesi
                signingCredentials: signingCredentials
            );

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new AccessToken
            {
                Token = token,
                Expiration = accessTokenExpiration
            };
        }

        // --- DEĞİŞİKLİK 2: 'SetClaims' metodu da sadeleşti (Rol listesi yok) ---
        private IEnumerable<Claim> SetClaims(int userId, string email, string fullName)
        {
            var claims = new List<Claim>();
            
            claims.AddNameIdentifier(userId.ToString());
            claims.AddEmail(email);
            claims.AddName(fullName);

            // 'claims.AddRoles(...)' satırını TAMAMEN SİLİYORUZ.
            // Burası artık boş.

            return claims;
        }

        // Not: Senin eski kodundaki 'CreateJwtSecurityToken' metodu artık
        // 'CreateToken' içinde birleştiği için o metoda gerek kalmadı.
    }
}