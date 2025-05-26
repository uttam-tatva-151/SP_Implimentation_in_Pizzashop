using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PMSData.Reposetories
{
    public class AuthRepo : IAuthRepo
    {
        private readonly AppDbContext _context;

        public AuthRepo(AppDbContext context)
        {
            _context = context;
        }

        readonly ResponseResult result = new();
        public async Task<ResponseResult> AddResetToken(ResetPasswordToken newTokenRow){
            try
            {
                _context.ResetPasswordTokens.Add(newTokenRow);
                await _context.SaveChangesAsync();
                result.Status = ResponseStatus.Success;
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.USER);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> CheckResetToken(UpdatePassword updatePassword)
        {
            try
            {
                IQueryable<ResetPasswordToken> query = _context.ResetPasswordTokens.AsNoTracking().Where(rpt => rpt.ResetToken == updatePassword.token);
                ResetPasswordToken? userData = await query.FirstOrDefaultAsync();
                if (userData != null)
                {
                    result.Data = userData;
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.USER);
                }
                else
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.USER);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            return result;
        }
        public async Task<bool> UpdatePassword(UpdatePassword updatePassword)
        {
            IQueryable<ResetPasswordToken> query = _context.ResetPasswordTokens.AsNoTracking()
                                .Include(u => u.User) 
                                .Where(u => u.ResetToken == updatePassword.token && u.IsContinue == true);

            ResetPasswordToken? userData = await query.FirstOrDefaultAsync();

            if (userData != null)
            {
                userData.User.PasswordHash = updatePassword.Password;
                userData.User.Modifyat = DateTime.Now;
                _context.Update(userData);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

    }
}
