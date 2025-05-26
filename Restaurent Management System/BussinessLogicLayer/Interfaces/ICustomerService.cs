using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface ICustomerService
    {
        Task<ResponseResult> GetCustomerList(PaginationDetails paginationDetails);
        Task<ResponseResult> ExportCustomerDataAsync(PaginationDetails paginationDetails);
        Task<ResponseResult> GetCustomerHistoryAsync(int customerId);
        Task<ResponseResult> UpdateCustomerAsync(CustomerDetails customer);
    }
}
