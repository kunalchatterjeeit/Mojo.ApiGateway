//using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Mojo.AuthManager.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mojo.AuthManager
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "alsdjkhdfhweriwerjkjsdhwkjn";
        public const int JWT_TOKEN_VALIDITY_MINS = 200;

        private readonly List<UserAccount> _userAccounts;
        public JwtTokenHandler()
        {

            _userAccounts = new List<UserAccount>()
            {
                new UserAccount() { UserName = "admin", Password = "admin123", Role = "Administrator" },
                new UserAccount() { UserName = "user01", Password = "user123", Role = "User " },
            };
        }

        public AuthenticationResponse? GenerateJwtToken(AuthenticationRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                return null;
            /*Validation*/
            IdentityModel? identityModel = IndetityRepository.ValidateIdentity(request.UserName, request.Password).Result;
            if(identityModel== null) return null;
            var userAccount = new UserAccount() { Role = identityModel.UserName, RegistrationId = identityModel.RegistrationId};
            if (userAccount == null) return null;

            var tokenExpiryTimestamp = DateTime.UtcNow.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, request.UserName),
                new Claim(ClaimTypes.Role, userAccount.Role)
            });

            var signingCredential = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimestamp,
                SigningCredentials = signingCredential
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse(){
                UserName = request.UserName,
                JwtToken= token,
                ExpiresIn = JWT_TOKEN_VALIDITY_MINS,
                RegistrationId = userAccount.RegistrationId
            };
        }
    }
}
