using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PMSCore.Beans;
using PMSCore.DTOs;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class JWTService : IJWTService
    {
        private readonly IUserRepo _userRepo;
        private readonly IRefreshTokenRepo _refreshTokenRepo;
        private readonly JwtConfig _jwtConfig;

        public JWTService( IUserRepo userRepo, IOptions<JwtConfig> jwtConfig, IRefreshTokenRepo refreshTokenRepo)
        {
            _userRepo = userRepo;
            _jwtConfig = jwtConfig.Value;
            _refreshTokenRepo = refreshTokenRepo;
        }
        public async Task<UserDto> ValidateAndGenerateTokenAsync(string userToken)
        {
            Userauthentication userData = await ValidateRefreshToken(userToken);
            string AccessToken = await GenerateAccessToken(userData.EmailId);
            UserDto userDto = new()
            {
                AccessToken = AccessToken,
                Email = userData.EmailId,
                UserName = userData.UserName,
            };
            return userDto;

        }
        public async Task<string> GenerateAccessToken(string email)
        {
            Userauthentication user = await _userRepo.GetUserDetailsByEmailAsync(email) ?? throw new Exception(MessageHelper.GetInfoMessageForNoRecordsFound(Constants.USER));
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtConfig.Key ?? string.Empty));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            string roleName = MapRoleIdToRoleName(user.RoleId);
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.EmailId),
                new Claim(ClaimTypes.Role, roleName)
            };

            JwtSecurityToken token = new(
                _jwtConfig.Issuer,
            _jwtConfig.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private static string MapRoleIdToRoleName(int roleId)
        {

            return roleId switch
            {
                1 => Constants.ADMIN_ROLE,
                2 => Constants.CHEF_ROLE,
                3 => Constants.ACCOUNT_MANAGER_ROLE,
                _ => Constants.GUEST_ROLE // Default role
            };
        }
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<Userauthentication> ValidateRefreshToken(string refreshToken)
        {
            RefreshToken storedToken = await _refreshTokenRepo.GetRefreshToken(refreshToken) ?? new RefreshToken();

            if (storedToken.Token == null || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException(Constants.INVALID_REFRESH_TOKEN);
            }
            Userauthentication user = await _userRepo.GetUserAuthenticationAsync(storedToken.UserId) ?? throw new Exception(MessageHelper.GetInfoMessageForNoRecordsFound(Constants.USER));

            return user;
        }

        public async Task SaveRefreshToken(int userId, string refreshToken)
        {
            await _refreshTokenRepo.SaveRefreshToken(userId, refreshToken);
        }

        public async Task RevokeRefreshToken(string refreshToken)
        {
            RefreshToken token = await _refreshTokenRepo.GetRefreshToken(refreshToken) ?? new RefreshToken();
            if (token != null)
            {
                _ = _refreshTokenRepo.RemoveRefreshToken(token);
            }
        }
        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            RefreshToken token = await _refreshTokenRepo.GetRefreshToken(refreshToken) ?? new RefreshToken();
            if (token != null)
            {
                return token.ExpiryDate > DateTime.UtcNow;
            }
            return false;
        }

        public async Task< string> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                RefreshToken token = await _refreshTokenRepo.GetRefreshToken(refreshToken) ?? new RefreshToken();
                if (token == null || token.ExpiryDate < DateTime.Now)
                {
                    return string.Empty;
                }
                Userauthentication user = await _userRepo.GetUserAuthenticationAsync(token.UserId) ?? throw new Exception(MessageHelper.GetInfoMessageForNoRecordsFound(Constants.USER));
                string newAccessToken = await GenerateAccessToken(user.EmailId);
                token.ExpiryDate = DateTime.Now.AddDays(30); // Set new expiry
                await _refreshTokenRepo.UpdateRefreshToken(token);

                return newAccessToken;
            }
            catch
            {
                // Log the exception for debugging
                throw ;
            }

        }
    }
}
