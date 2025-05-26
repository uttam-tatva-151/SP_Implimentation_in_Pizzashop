using PMSCore.Beans;

namespace PMSData.Interfaces
{
    public interface IFeedbackRepo
    {
        Task<ResponseResult> AddCustomerFeedbackAsync(FeedbackForm feedback);
        Task<ResponseResult> UpdateCustomerFeedbackAsync(FeedbackForm feedback);
    }
}

