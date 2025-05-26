using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces
{
    public interface IUserRepo
    {
        Task<ResponseResult> AddNewUserAsync(Userauthentication user);
        Task<ResponseResult> GetUsers(PaginationDetails paginationDetails);
        Task<ResponseResult> GetUserDetailsAsync(int userId);
        Task<Userdetail?> GetUserDataAsync(string email);
        Task<Userauthentication?> GetUserAuthenticationAsync(int userId);
        Task<int?> GetUserIdByEmailAsync(string email);
        Task<string?> GetEmailByUserIdAsync(int userId);
        Task<ResponseResult> AddUserDataAsync(Userdetail userData);
        Task<bool> IsEmailExistsAsync(string email);
        Task<Userauthentication?> GetUserDetailsByEmailAsync(string email);
        Task<ResponseResult> UpdateUserAuthenticationAsync(Userauthentication user);
        Task<ResponseResult> UpdateUserDetailsAsync(Userdetail user);
        Task<List<Userauthentication>> GetAllUsersAsync();
        Task<Userdetail?> GetUserDetailsByUserIdAsync(int id);
        Task<string?> GetUserNameByEmailAsync(string email);
        Task<byte[]> GetUserProfileImgByEmailAsyncAsByteStream(string email);
        Task<Userauthentication?> GetUserByEmail(string email);
        Task<ResponseResult> GetUserdetailsForProfileAsync(string emailId);

    }
}
