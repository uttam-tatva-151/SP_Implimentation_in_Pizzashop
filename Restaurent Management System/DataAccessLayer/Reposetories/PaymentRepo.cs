using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class PaymentRepo : IPaymentRepo
    {
        private readonly AppDbContext _appDbContext;
        public PaymentRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();
        public async Task<ResponseResult> AddPaymentDetailsAsync(PaymentDetail paymentDetail)
        {
            try
            {
                _appDbContext.PaymentDetails.Add(paymentDetail);
                await _appDbContext.SaveChangesAsync();
                result.Data = paymentDetail.PaymentId;
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.PAYMENT_DETAILS);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

    }
}
