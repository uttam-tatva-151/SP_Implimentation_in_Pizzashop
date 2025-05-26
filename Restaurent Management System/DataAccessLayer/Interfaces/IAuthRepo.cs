using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces
{
    public interface IAuthRepo
    {
        Task<ResponseResult> AddResetToken(ResetPasswordToken newTokenRow);
        Task<ResponseResult> CheckResetToken(UpdatePassword updatePassword);
        Task<bool> UpdatePassword(UpdatePassword updatePassword);
    }
}
