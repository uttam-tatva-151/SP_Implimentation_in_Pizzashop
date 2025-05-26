using PMSCore.Beans;

namespace PMSData.Interfaces
{
    public interface IPaymentRepo
    {
        Task<ResponseResult> AddPaymentDetailsAsync(PaymentDetail paymentDetail);

    }
}
