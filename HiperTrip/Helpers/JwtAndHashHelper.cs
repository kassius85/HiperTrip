using Entities.DTOs;
using HiperTrip.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HiperTrip.Helpers
{
    public static class JwtAndHashHelper
    {
        /// <summary>
        /// Crea contraseña con Hash.
        /// </summary>
        /// <param name="contrasena"></param>
        /// <param name="contrhash"></param>
        /// <param name="contrsalt"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public static bool CrearContrasenaHash(string contrasena, out byte[] contrhash, out byte[] contrsalt, out string mensaje)
        {
            contrhash = null;
            contrsalt = null;
            mensaje = "";

            if (!string.IsNullOrEmpty(contrasena) && !string.IsNullOrWhiteSpace(contrasena))
            {
                using (HMACSHA512 hmac = new HMACSHA512())
                {
                    contrsalt = hmac.Key; // Guardar el Salt para este usuario.
                    contrhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(contrasena)); // Aplicar Hash a la contraseña.
                }

                return true;
            }
            {
                mensaje = "La contraseña no puede ser nula o vacía.";

                return false;
            }
        }

        public static byte[] HashCode(string codigo, byte[] contrsalt)
        {
            byte[] hashedCode = default;

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                hmac.Key = contrsalt;
                hashedCode = hmac.ComputeHash(Encoding.UTF8.GetBytes(codigo)); // Aplicar Hash.
            }

            return hashedCode;
        }

         /// <summary>
            /// Verificar contraseña válida.
            /// </summary>
            /// <param name="password"></param>
            /// <param name="storedHash"></param>
            /// <param name="storedSalt"></param>
            /// <returns></returns>
        public static bool VerificarContrasenadHash(byte[] contrsalt, byte[] contrhash, string contrasena)
        {
            using (HMACSHA512 hmac = new HMACSHA512(contrsalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(contrasena));

                return computedHash.ArraysEquals(contrhash);
            }
        }

        /// <summary>
        /// Construye el token para validar peticiones de usuario.
        /// </summary>
        /// <param name="dtoUsuario"></param>
        /// <param name="rol"></param>
        /// <param name="_appSettings"></param>
        /// <returns></returns>
        public static string BuildToken(UsuarioDto usuario, string secretKey, int expireMinutes)
        {
            if (!usuario.IsNull())
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.ASCII.GetBytes(secretKey);

                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(JwtRegisteredClaimNames.UniqueName, usuario.NombreUsuar),
                    new Claim(JwtRegisteredClaimNames.Email, usuario.CorreoUsuar),
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