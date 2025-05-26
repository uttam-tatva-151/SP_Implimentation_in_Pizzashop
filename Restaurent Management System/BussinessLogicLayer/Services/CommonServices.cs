using System.Text;
using Microsoft.AspNetCore.Http;
using PMSCore.Beans;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class CommonServices : ICommonServices
    {
        private readonly ICommonRepo _dalCommonServices;
        public CommonServices(ICommonRepo dalCommonServices)
        {
            _dalCommonServices = dalCommonServices;
        }
        ResponseResult result = new();

        public async Task<ResponseResult> GetCountryList()
        {
            try
            {
                result = await _dalCommonServices.GetCountryList();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> GetStateList(int countryId)
        {
            try
            {
                result = await _dalCommonServices.GetStateList(countryId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> GetCityList(int stateId)
        {
            try
            {
                result = await _dalCommonServices.GetCityList(stateId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> GetRoleList()
        {
            try
            {
                result = await _dalCommonServices.GetRoleList();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public string Encrypt(string password)
        {
            byte[] encode = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(encode);
        }
    }
}
