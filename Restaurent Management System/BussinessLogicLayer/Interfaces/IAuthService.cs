using PMSCore.Beans;
using PMSCore.ViewModel;


namespace PMSServices.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseResult> LoginUser(LoginRequest loginRequest);
        Task<ResponseResult> ResetPassword(UpdatePassword updatePassword);
        Task<ResponseResult> SendForgotPassLink(string emailId);

        Task<string> GetEmailBodyAsync(string templateName);
        Task<bool> SendEmailAsync(string emailId, string subject, string emailBody);
    }
}
