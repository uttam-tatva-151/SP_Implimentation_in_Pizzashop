
using Microsoft.AspNetCore.Http;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;


namespace PMSServices.Services
{
    public class UserService : IUserService
    {
        private readonly IAuthService _bllAuthService;
        private readonly IUserRepo _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor, IUserRepo userRepo, IAuthService bllAuthService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepo = userRepo;
            _bllAuthService = bllAuthService;
        }

        ResponseResult result = new();

        public async Task<ResponseResult> AddUser(NewUser user)
        {
            try
            {
                //  Input Validation
                if (await _userRepo.IsEmailExistsAsync(user.EmailId))
                    return ErrorResponse(Constants.WARNING_EMAIL_ALL_READY_EXISTS);

                if (user.EditorId == 0)
                    return ErrorResponse(Constants.WARNING_EDITOR_ID_NOT_FOUND);
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    
                Userauthentication userAuth = new()
                {
                    UserName = user.UserName,
                    EmailId = user.EmailId,
                    PasswordHash = user.Password,
                    RoleId = user.RoleId,
                    Createat = DateTime.Now,
                    Iscontinued = true
                };
                result = await _userRepo.AddNewUserAsync(userAuth);
                if (result.Status != ResponseStatus.Success)
                    return result;
                int userId = (int)result.Data;
                Userdetail userdetail = new()
                {
                    UserId = userId,//refrence of UserAuthentication table
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    CityId = user.CityId,
                    StateId = user.StateId,
                    ContryId = user.ContryId,
                    ZipCode = user.ZipCode,
                    Status = "Active",
                    Createat = DateTime.UtcNow,
                    CreateBy = user.EditorId,
                    Isactive = true
                };
                DateTime dateTimeUnspecified = DateTime.SpecifyKind(userdetail.Createat, DateTimeKind.Unspecified);
                userdetail.Createat = dateTimeUnspecified;
                if (user.Photo != null)
                {
                    userdetail.Photo = ConvertImageToByteArray(user.Photo);
                }

                result = await _userRepo.AddUserDataAsync(userdetail);

                if (result.Status == ResponseStatus.Success)
                {
                    if (await SendLoginDetailsToEmail(user.EmailId, user.UserName, user.Password))
                    {
                        return result;
                    }
                    else
                    {
                        result.Message = Constants.ERROR_SENDING_LOGIN_DETAILS_EMAIL;
                        result.Status = ResponseStatus.NotFound;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
            return result;
        }

        public async Task<ResponseResult> EditUser(UpdateUser user)
        {
            try
            {
                Userdetail? userData = await _userRepo.GetUserDataAsync(user.EmailId);
                if (userData == null)
                    return ErrorResponse(MessageHelper.GetInfoMessageForNoRecordsFound(Constants.USER));
                else
                {
                    userData.FirstName = user.FirstName;
                    userData.LastName = user.LastName;
                    userData.User.UserName = user.UserName;
                    userData.User.RoleId = user.RoleId;
                    userData.Status = user.Status;
                    userData.PhoneNumber = user.PhoneNumber;
                    userData.Address = user.Address;
                    userData.CityId = user.CityId;
                    userData.StateId = user.StateId;
                    userData.ContryId = user.ContryId;
                    userData.ZipCode = user.ZipCode;
                    if (user.Photo != null)
                    {
                        userData.Photo = ConvertImageToByteArray(user.Photo);
                    }
                    // userData.Photo = user.Photo;
                    userData.ModifyBy = user.EditorId;
                    userData.Modifyat = DateTime.Now;
                    result = await _userRepo.UpdateUserDetailsAsync(userData);
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
            return result;
        }

        public async Task<ResponseResult> DeleteUser(int id, int editor)
        {
            try
            {
                if (id <= 0 || editor <= 0)
                    return ErrorResponse(MessageHelper.GetWarningMessageForInvalidInput(Constants.USER));

                Userdetail? existingUser = await _userRepo.GetUserDetailsByUserIdAsync(id);
                if (existingUser == null)
                    return ErrorResponse(MessageHelper.GetInfoMessageForNoRecordsFound(Constants.USER));
                else
                {
                    existingUser.Isactive = false;
                    existingUser.ModifyBy = editor;
                    existingUser.Modifyat = DateTime.Now;
                    existingUser.User.Iscontinued = false;
                    existingUser.User.Modifyat = DateTime.Now;
                }
                result = await _userRepo.UpdateUserDetailsAsync(existingUser);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
            return result;
        }

        public async Task<ResponseResult> GetUsers(PaginationDetails paginationDetails)
        {
            try
            {
                result = await _userRepo.GetUsers(paginationDetails);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
            return result;
        }
        public async Task<int> GetUserIdByEmailId(string emailId)
        {
            try
            {
                return await _userRepo.GetUserIdByEmailAsync(emailId) ?? 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<ResponseResult> GetUserDetails(int userId)
        {
            try
            {
                if (userId <= 0)
                    return ErrorResponse(MessageHelper.GetWarningMessageForInvalidInput(Constants.USER));

                result = await _userRepo.GetUserDetailsAsync(userId);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
            return result;
        }

        public async Task<string?> GetUserName(string email)
        {
            return await _userRepo.GetUserNameByEmailAsync(email);
        }

        public async Task<byte[]> GetUserProfileImgByEmailAsByteStream(string email)
        {
            return await _userRepo.GetUserProfileImgByEmailAsyncAsByteStream(email);
        }
        //  Helper Methods for Error Handling

        private static ResponseResult ErrorResponse(string message)
        {
            return new ResponseResult
            {
                Status = ResponseStatus.Error,
                Message = message
            };
        }
        //Helper Methods
        public static byte[] ConvertImageToByteArray(IFormFile file)
        {
            using MemoryStream memoryStream = new();
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
        private static ResponseResult HandleException(Exception ex)
        {
            return new ResponseResult { Status = ResponseStatus.Error, Message = $"An error occurred: {ex.Message}" };
        }
        private async Task<bool> SendLoginDetailsToEmail(string email, string userName, string password)
        {
            try
            {
                string emailBody = await _bllAuthService.GetEmailBodyAsync("AccountLoginDetailsFormat.html");
                emailBody = emailBody.Replace("{{username}}", userName);
                emailBody = emailBody.Replace("{{password}}", password);
                string subject = "Login Details";
                return await _bllAuthService.SendEmailAsync(email, subject, emailBody);
            }
            catch 
            {
                throw;
            }
        }
    }
}
