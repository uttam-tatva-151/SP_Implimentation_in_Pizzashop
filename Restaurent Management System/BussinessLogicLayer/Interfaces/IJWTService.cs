using PMSCore.DTOs;

namespace PMSServices.Interfaces
{
    public interface IJWTService
    {
    
        Task<string> GenerateAccessToken(string email);
        string GenerateRefreshToken();
        Task SaveRefreshToken(int userId, string refreshToken);
        Task RevokeRefreshToken(string refreshToken);
        Task<string> RefreshTokenAsync(string refreshToken);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
        Task<UserDto> ValidateAndGenerateTokenAsync(string userToken);
    }
}
