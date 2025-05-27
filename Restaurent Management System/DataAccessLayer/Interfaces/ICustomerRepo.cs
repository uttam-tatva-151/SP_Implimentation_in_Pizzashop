using Azure;
using PMSCore.Beans;
using PMSCore.DTOs;
using PMSCore.ViewModel;

namespace PMSData.Interfaces
{
    public interface ICustomerRepo
    {
        Task<Customer?> GetCustomerDetailsByEmail(string emailId);
        Task<List<CustomerDetails>> GetCustomersAsync(PaginationDetails paginationDetails);
        Task<ResponseResult> AddNewCustomerAsync(CustomerDTO newCustomer);
        Task<Customer?> GetCustomerDetailsById(int tokenId);
        Task<bool> CheckForDuplicateCustomer(int customerId,string email, string phoneNumber);
        Task<ResponseResult> UpdateCustomerAsync(Customer customer);
        Task<Customer?> GetCustomerDetailsByEmailOrPhone(string email, string phoneNumber);
    }
}
