using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class CommonRepo : ICommonRepo
    {
        private readonly AppDbContext _context;
        public CommonRepo(AppDbContext context)
        {
            _context = context;
        }
        readonly ResponseResult result = new();


        public async Task<ResponseResult> GetCountryList()
        {
            try
            {
                IQueryable<ContryList> query = _context.ContryLists.Where(c => c.IsContinue == true);
                List<ContryList> countryList = await query.ToListAsync();
                if (countryList.Count > 0)
                {
                    result.Data = countryList;
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.COUNTRY_LIST);
                }
                else
                {
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.COUNTRY_LIST);
                }
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
                IQueryable<StateList> query = _context.StateLists.Where(s => s.ContryId == countryId);
                //List<(int,string)> not working!!! therefor i use VAR here
                var stateList = await query.Select(c => new { c.StateId, c.StateName }).ToListAsync();
                if (stateList.Count > 0)
                {
                    result.Data = stateList;
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.STATE_LIST);
                }
                else
                {
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.STATE_LIST);
                }
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
                IQueryable<CityList> query = _context.CityLists.Where(c => c.StateId == stateId);
                 //List<(int,string)> not working!!! therefor i use VAR here
                var cityList = await query.Select(c => new { c.CityId, c.CityName }).ToListAsync();
                if (cityList.Count > 0)
                {
                    result.Data = cityList;
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.CITY_LIST);
                }
                else
                {
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CITY_LIST);
                }
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
                IQueryable<Role> query = _context.Roles;
                List<Role> roleList = await query.ToListAsync();
                if (roleList.Count > 0)
                {
                    result.Data = roleList;
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.ROLE_LIST);
                }
                else
                {
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ROLE_LIST);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> GetUserByEmail(string email){
            try{
                IQueryable<Userdetail> query = _context.Userdetails.Include(u=>u.User).Where(u=>u.User.EmailId == email);
                Userdetail? userData = await query.FirstOrDefaultAsync();
                if(userData != null){
                    result.Data = userData;
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.USER);
                }
                else{
                    result.Status = ResponseStatus.NotFound;
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.USER);
                }
            }catch(Exception ex){
                result.Message = ex.Message;
                result.Status= ResponseStatus.Error;
            }
            return result;
        }

    }
}
