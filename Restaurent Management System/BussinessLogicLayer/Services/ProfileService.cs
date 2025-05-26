using Microsoft.AspNetCore.Http;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class ProfileService : IProfileService
    {

        private readonly IUserRepo _userRepo;

        public ProfileService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        ResponseResult result = new();
        public async Task<ResponseResult> GetProfileAsync(string emailId)
        {
            try
            {
                result = await _userRepo.GetUserdetailsForProfileAsync(emailId);
                Userdetail userData = (Userdetail)result.Data;
                if (userData != null)
                {
                    UserProfileVM userProfile = new()
                    {
                        EmailId = userData.User.EmailId,
                        FirstName = userData.FirstName,
                        LastName = userData.LastName,
                        UserName = userData.User.UserName,
                        RoleName = userData.User.Role.RoleName,
                        PhoneNumber = userData.PhoneNumber,
                        Address = userData.Address,
                        CityId = userData.CityId,
                        StateId = userData.StateId,
                        ContryId = userData.ContryId,
                        ZipCode = userData.ZipCode,
                        // Photo = userData.Photo,
                    };
                    result.Data = userProfile;
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.USER);
                }
                else
                {
                    result.Status = ResponseStatus.Success;
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
        public async Task<ResponseResult> UpdateProfileAsync(UserProfileVM profile)
        {
            try
            {
                Userauthentication userData = await _userRepo.GetUserDetailsByEmailAsync(profile.EmailId) ?? new Userauthentication();

                if (userData != null)
                {
                    if (userData.Userdetail != null)
                    {
                        userData.Userdetail.FirstName = profile.FirstName;
                        userData.Userdetail.LastName = profile.LastName;
                        if (userData.Userdetail.User != null)
                        {
                            userData.UserName = profile.UserName;
                            userData.Modifyat = DateTime.Now;
                        }
                        userData.Userdetail.PhoneNumber = profile.PhoneNumber;
                        userData.Userdetail.Address = profile.Address;
                        userData.Userdetail.CityId = profile.CityId;
                        userData.Userdetail.StateId = profile.StateId;
                        userData.Userdetail.ContryId = profile.ContryId;
                        userData.Userdetail.ZipCode = profile.ZipCode;
                        if(profile.Photo != null)
                        {
                            userData.Userdetail.Photo = ConvertImageToByteArray(profile.Photo);
                        }
                        else
                        {
                            userData.Userdetail.Photo = userData.Userdetail.Photo;
                        }
                        userData.Userdetail.Modifyat = DateTime.Now;
                        userData.Userdetail.ModifyBy = profile.editorId;

                        result = await _userRepo.UpdateUserAuthenticationAsync(userData);
                    }
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.USER);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                if (result.Status == ResponseStatus.Success)
                {
                    result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.USER);
                }
                else
                {
                    result.Message = MessageHelper.GetErrorMessageForUpdateOperation(Constants.USER);
                    result.Status = ResponseStatus.Error;
                }
            }
            catch
            {
                result.Message = MessageHelper.GetErrorMessageForUpdateOperation(Constants.USER);
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            try
            {
                Userauthentication? userData = await _userRepo.GetUserByEmail(changePasswordVM.Email);
                if(userData != null){
                    
                
                if ( BCrypt.Net.BCrypt.Verify(changePasswordVM.OldPassword, userData.PasswordHash))
                {
                    userData.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordVM.NewPassword);
                    userData.Modifyat = DateTime.Now;
                    result = await _userRepo.UpdateUserAuthenticationAsync(userData);
                    if (result.Status == ResponseStatus.Success)
                    {
                        result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.PASSWORD);
                    }
                }
                else
                {
                    result.Message = Constants.ERROR_PASSWORD_MISMATCH;
                    result.Status = ResponseStatus.Error;
                }
            }else{
                result.Message = MessageHelper.GetNotFoundMessage(Constants.USER);
                result.Status = ResponseStatus.NotFound;
            }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
    
        //Helper Methods
        public static byte[] ConvertImageToByteArray(IFormFile file)
        {
            using MemoryStream memoryStream = new();
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

    }
}
