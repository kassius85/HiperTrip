using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Helpers.Extensions
{
    public static class StringHashExtensions
    {
        public static bool BuildHashString(this string stringCode, out byte[] paswdhash, out byte[] paswdsalt, out string mensaje)
        {
            paswdhash = default;
            paswdsalt = default;
            mensaje = string.Empty;

            if (!string.IsNullOrEmpty(stringCode.Trim()))
            {
                using (HMACSHA512 hmac = new HMACSHA512())
                {
                    paswdsalt = hmac.Key; // Crear el Salt.
                    paswdhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringCode)); // Aplicar Hash al código.
                }

                return true;
            }
            else
            {
                mensaje = "La contraseña no puede ser nula o vacía.";

                return false;
            }
        }

        public static byte[] BuildHashCode(this string stringCode, byte[] paswdsalt)
        {
            byte[] hashedCode = default;

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                hmac.Key = paswdsalt;
                hashedCode = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringCode)); // Aplicar Hash.
            }

            return hashedCode;
        }

        public static bool VerifyHashCode(this string contrasena, byte[] paswdsalt, byte[] paswdhash)
        {
            using (HMACSHA512 hmac = new HMACSHA512(paswdsalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(contrasena));

                return computedHash.ArraysEquals(paswdhash);
            }
        }

        public static string BuildToken(this string CodigoUsuar, string CorreoUsuar, string secretKey, int expireMinutes)
        {
            if (!string.IsNullOrEmpty(CodigoUsuar))
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.ASCII.GetBytes(secretKey);

                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.UniqueName, CodigoUsuar),
                        new Claim(JwtRegisteredClaimNames.Email, CorreoUsuar),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())//, // Identificador único del token por cada conexión incluso de un mismo usuario.
                        //new Claim(ClaimTypes.Role, "admin")
                    }),
                    NotBefore = DateTime.UtcNow, // No usar antes de esta hora
                    Expires = DateTime.UtcNow.AddMinutes(expireMinutes), // El tiempo de expiración se define en appsettings.json
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }

            return string.Empty;
        }

    }
}