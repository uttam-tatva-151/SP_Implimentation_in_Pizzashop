using PMSCore.Beans;

namespace PMSServices.Interfaces
{
    public interface IOrdersService
    {
        (byte[], string) CreatePdf(string invoiceId,string partialView);

        Task<ResponseResult> ExportOrderList(string orderSearch, string status, string dateRange);
        Task<ResponseResult> GetOrderDetailsAsync(int orderId);
        Task<(string, string)> GetTableBasedOrdersDetails(int[] tableIds);
        Task<ResponseResult> GetOrderList(PaginationDetails paginationDetails);
    }
}
