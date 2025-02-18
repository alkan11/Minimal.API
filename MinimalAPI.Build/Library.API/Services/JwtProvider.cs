using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Library.API.Services
{
    public class JwtProvider
    {
        public string CreateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretkeymysecretkeymysecretkeymysecretkeymysecretkeymysecretkeymysecretkeymysecretkey"));

            JwtSecurityToken securityToken = new
            (
                issuer: "alkan",
                audience: "alkan",
                claims: null,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512)
            );

            JwtSecurityTokenHandler handler = new();
            string token = handler.WriteToken(securityToken);

            return token;
        }
    }
}
