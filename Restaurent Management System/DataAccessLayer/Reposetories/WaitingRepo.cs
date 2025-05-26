using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class WaitingRepo : IWaitingRepo
    {
        private readonly AppDbContext _appDbContext;

        public WaitingRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();
        public async Task<ResponseResult> AddWaitingTokenAsync(WaitingList token)
        {
            try
            {
                _appDbContext.WaitingLists.Add(token);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.WAITING_TOKEN);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<List<WaitingList>> GetWaitingTokensBySectionAsync(int sectionId)
        {
            IQueryable<WaitingList> query = _appDbContext.WaitingLists
                                            .Include(w => w.Customer).AsNoTracking().OrderBy(w => w.Createat);
            if (sectionId == 0)
            {
                query = query.Where(w => w.Isactive == true);
            }
            else
            {
                query = query.Where(w => w.SectionId == sectionId && w.Isactive == true);
            }

            return await query.ToListAsync();
        }
         public async Task<List<WaitingList>> GetAllWaitingTokensAsync(int sectionId)
        {
            IQueryable<WaitingList> query = _appDbContext.WaitingLists.AsNoTracking().OrderBy(w => w.Createat);
            if (sectionId != 0)
            {
                query = query.Where(w => w.SectionId == sectionId);
            }

            return await query.ToListAsync();
        }

        public async Task<WaitingList?> GetWaitingTokenByIdAsync(int tokenId)
        {
            return await _appDbContext.WaitingLists
                                            .Include(w => w.Customer)
                                            .Where(w => w.TokenId == tokenId && w.Isactive == true)
                                            .FirstOrDefaultAsync();
        }

        public async Task<ResponseResult> UpdateWaitingToken(WaitingList waitingToken)
        {
            try
            {
                _appDbContext.WaitingLists.Update(waitingToken);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.WAITING_TOKEN);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

    }
}
