using PMSCore.Beans;

namespace PMSData.Interfaces
{
    public interface ICommonRepo
    {
        Task<ResponseResult> GetCountryList();
        Task<ResponseResult> GetStateList(int countryId);
        Task<ResponseResult> GetCityList(int stateId);
        Task<ResponseResult> GetRoleList();
        Task<ResponseResult> GetUserByEmail(string email);
    }
}
