using PMSCore.Beans;

namespace PMSServices.Interfaces
{
    public interface ICommonServices
    {
        Task<ResponseResult> GetCountryList();
        Task<ResponseResult> GetStateList(int countryId);
        Task<ResponseResult> GetCityList(int stateId);
        Task<ResponseResult> GetRoleList();
        string Encrypt(string password);
    }
}
