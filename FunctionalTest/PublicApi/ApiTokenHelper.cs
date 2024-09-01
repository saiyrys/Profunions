using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalTest.PublicApi
{
    public class ApiTokenHelper
    {
        public static string GetAdminUserToken()
        {
            string userName = "admin";
            string[] role = { "ADMIN" };

            return CreateToken(userName, role);
        }

        public static string GetUserToken()
        {
            string userName = "user";
            string[] role = { "USER" };

            return CreateToken(userName, role);
        }

        private static string CreateToken(string userName, string[] role)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };

            foreach (var r in role)
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }

            var key = Encoding.ASCII.GetBytes("S19v59LSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
