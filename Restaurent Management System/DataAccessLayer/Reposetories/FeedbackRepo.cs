using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class FeedbackRepo : IFeedbackRepo
    {
         private readonly AppDbContext _appDbContext;

            public FeedbackRepo(AppDbContext appDbContext)
            {
                _appDbContext = appDbContext;
            }
        readonly ResponseResult result = new();
        public async Task<ResponseResult> AddCustomerFeedbackAsync(FeedbackForm feedback)
        {
            try{
                _appDbContext.FeedbackForms.Add(feedback);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.ORDER);
                result.Status = ResponseStatus.Success;
            
            }catch(Exception ex){
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> UpdateCustomerFeedbackAsync(FeedbackForm feedback)
        {
            try{
                _appDbContext.FeedbackForms.Update(feedback);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.ORDER);
                result.Status = ResponseStatus.Success;
            
            }catch(Exception ex){
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
    }
}

