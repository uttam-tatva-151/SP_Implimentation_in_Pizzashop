using PMSCore.Beans;

namespace PMSData.Interfaces
{
    public interface IInvoiceRepo
    {
        Task<ResponseResult> AddInvoiceAsync(Invoice invoice);
        Task<Invoice> GetInvoiceNumberAsync(int orderId);
    }
}
