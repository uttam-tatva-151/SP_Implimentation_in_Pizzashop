namespace PMSData.Interfaces
{
    public interface IRefreshTokenRepo
    {
        Task SaveRefreshToken(int userId,string refreshToken);
        Task<RefreshToken?> GetRefreshToken(string refreshToken);
        Task RemoveRefreshToken(RefreshToken token);
        Task UpdateRefreshToken(RefreshToken token);
    }
}
