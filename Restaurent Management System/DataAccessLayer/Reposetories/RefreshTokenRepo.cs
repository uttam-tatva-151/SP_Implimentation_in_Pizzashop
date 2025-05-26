using Microsoft.EntityFrameworkCore;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class RefreshTokenRepo : IRefreshTokenRepo
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveRefreshToken(int userId, string refreshToken)
        {
            try{
                int tokenCount = await _context.RefreshTokens.CountAsync();
            RefreshToken token = new()
            {
                Id = tokenCount + 1,
                Token = refreshToken,
                UserId = userId,                   
                ExpiryDate = DateTime.Now.AddDays(30),  // Set expiry (e.g., 30 days)
                IsRevoked = false
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message.ToString());
            }
        
        }

        public async Task<RefreshToken?> GetRefreshToken(string refreshToken)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && x.IsRevoked == false);
        }

        public async Task RemoveRefreshToken(RefreshToken token)
        {
            token.IsRevoked = true;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshToken(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
