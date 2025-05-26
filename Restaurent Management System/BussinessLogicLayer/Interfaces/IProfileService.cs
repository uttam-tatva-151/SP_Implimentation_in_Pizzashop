using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface IProfileService
    {
        Task<ResponseResult> GetProfileAsync(string profileId);
        Task<ResponseResult> UpdateProfileAsync(UserProfileVM profile);
        Task<ResponseResult> ChangePassword(ChangePasswordVM ChangePasswordVM);
        // Task<ResponseResult> Logout();
    }
}
