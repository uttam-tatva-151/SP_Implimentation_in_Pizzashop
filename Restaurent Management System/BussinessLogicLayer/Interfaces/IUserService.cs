using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface IUserService
    {
        Task<ResponseResult> AddUser(NewUser user);
        Task<ResponseResult> EditUser(UpdateUser user);
        Task<ResponseResult> DeleteUser(int id,int editorId);
        Task<ResponseResult> GetUsers(PaginationDetails paginationDetails);
        Task<ResponseResult> GetUserDetails(int userId);
        Task<string?> GetUserName(string email);
        Task<byte[]> GetUserProfileImgByEmailAsByteStream(string email);
        Task<int> GetUserIdByEmailId(string emailId);
    }
}
