using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.Beans.ENUM;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly AppDbContext _appDbContext;

        public CustomerRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();
        public async Task<Customer?> GetCustomerDetailsByEmail(string emailId)
        {
            return await _appDbContext.Customers.Where(c => c.EmailId == emailId).FirstOrDefaultAsync();
        }

        public async Task<List<CustomerDetails>> GetCustomersAsync(PaginationDetails paginationDetails)
        {
            try
            {
                IQueryable<Order> query = _appDbContext.Orders.Include(o => o.Customer);

                // Apply search query filter
                if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
                {
                    query = query.Where(o => o.Customer.CustName.ToLower().Contains(paginationDetails.SearchQuery.ToLower()) ||
                                            o.Customer.EmailId.ToLower().Contains(paginationDetails.SearchQuery.ToLower()) ||
                                            o.Customer.PhoneNumber.Contains(paginationDetails.SearchQuery));
                }
                // Apply date range filter
                if (paginationDetails.DateRange != TimePeriod.All)
                {
                    DateTime startDate = paginationDetails.DateRange switch
                    {
                        TimePeriod.LastSevenDays => DateTime.Now.AddDays(-7),
                        TimePeriod.LastThirtyDays => DateTime.Now.AddDays(-30),
                        TimePeriod.CurrentMonth => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                        _ => DateTime.MinValue
                    };

                    query = query.Where(o => o.Createat >= startDate && o.Createat <= DateTime.Now);
                }

                // Apply custom date range filter
                DateTime fromDate = paginationDetails.FromDate.ToDateTime(new TimeOnly(0, 0));
                DateTime toDate = paginationDetails.ToDate.ToDateTime(new TimeOnly(23, 59, 59));

                if (fromDate != DateTime.MinValue || toDate != DateTime.MaxValue)
                {
                    query = query.Where(o => o.Createat >= fromDate && o.Createat <= toDate);
                }

                // Group and project data
                IQueryable<CustomerDetails> groupedQuery = query.GroupBy(o => new
                {
                    o.Customer.CustId,
                    o.Customer.CustName,
                    o.Customer.PhoneNumber,
                    o.Customer.EmailId,
                    o.Customer.TotalOrders
                })
                .Select(g => new CustomerDetails
                {
                    CustomerId = g.Key.CustId,
                    CustomerName = g.Key.CustName,
                    CustomerPhone = g.Key.PhoneNumber,
                    CustomerEmail = g.Key.EmailId,
                    TotalOrders = g.Key.TotalOrders,
                    LastOrder = DateOnly.FromDateTime(g.Max(o => o.Createat)) // Latest order date
                });

                // Apply sorting
                switch (paginationDetails.SortColumn.ToLower())
                {
                    case "name":
                        if (paginationDetails.SortOrder == Constants.ASC_ORDER)
                        {
                            groupedQuery = groupedQuery.OrderBy(c => c.CustomerName);
                        }
                        else
                        {
                            groupedQuery = groupedQuery.OrderByDescending(c => c.CustomerName);
                        }
                        break;

                    case "totalorder":
                        if (paginationDetails.SortOrder == Constants.ASC_ORDER)
                        {
                            groupedQuery = groupedQuery.OrderBy(c => c.TotalOrders);
                        }
                        else
                        {
                            groupedQuery = groupedQuery.OrderByDescending(c => c.TotalOrders);
                        }
                        break;

                    case "date":
                        if (paginationDetails.SortOrder ==Constants.ASC_ORDER)
                        {
                            groupedQuery = groupedQuery.OrderBy(c => c.LastOrder);
                        }
                        else
                        {
                            groupedQuery = groupedQuery.OrderByDescending(c => c.LastOrder);
                        }
                        break;

                    default:
                        groupedQuery = groupedQuery.OrderBy(c => c.CustomerName); // Default sorting
                        break;
                }

                // Set total records
                paginationDetails.TotalRecords = await groupedQuery.CountAsync();

                // Fetch and return paginated data
                return await groupedQuery.Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                                         .Take(paginationDetails.PageSize)
                                         .ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<ResponseResult> AddNewCustomerAsync(Customer newCustomer)
        {
            try
            {
                _appDbContext.Customers.Add(newCustomer);
                await _appDbContext.SaveChangesAsync();
                result.Data = newCustomer.CustId;
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.CUSTOMER);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<Customer?> GetCustomerDetailsById(int customerId)
        {
            return await _appDbContext.Customers.Where(c => c.CustId == customerId).FirstOrDefaultAsync();
        }

        public async Task<bool> CheckForDuplicateCustomer(int customerId, string email, string phoneNumber)
        {
            return await _appDbContext.Customers.AnyAsync(c => (c.EmailId == email || c.PhoneNumber == phoneNumber) && c.CustId == customerId);
        }

        public async Task<ResponseResult> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                _appDbContext.Customers.Update(customer);
                await _appDbContext.SaveChangesAsync();
                result.Data = customer.CustId;
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.CUSTOMER);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<Customer?> GetCustomerDetailsByEmailOrPhone(string email, string phoneNumber)
        {
            return await _appDbContext.Customers
                .Where(c => c.EmailId == email || c.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync();
        }
    }
}
