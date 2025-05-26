using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.Beans.ENUM;
using PMSCore.DTOs;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class OrderRepo : IOrderRepo
    {
        private readonly AppDbContext _appDbContext;

        public OrderRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();
        public async Task<List<KOTDTO>> GetKotDataAsync(string status, int categoryId)
        {
            DbConnection connection = _appDbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            string sql = "SELECT * FROM get_kot_data(@status, @categoryId)";
            IEnumerable<KOTDTO> result = await Dapper.SqlMapper.QueryAsync<KOTDTO>(
                connection, sql, new { status, categoryId });

            return result.ToList();
        }
        public async Task<Dictionary<int, (int TotalOrders, DateOnly LastOrderDate)>> GetCustomerOrderDataAsync(List<int> customerIds)
        {
            // Query the database to fetch total orders and last order dates for the given customer IDs
            return await _appDbContext.Orders
                .Where(o => customerIds.Contains(o.CustomerId))
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalOrders = g.Count(),
                    LastOrderDate = g.Max(o => o.Createat)
                })
                .ToDictionaryAsync(
                    x => x.CustomerId,
                    x => (
                        x.TotalOrders,
                        DateOnly.FromDateTime(x.LastOrderDate)
                    )
                );
        }
        public async Task<List<OrderDetail>> GetOrderListAsync(PaginationDetails paginationDetails)
        {
            try
            {
                // Include related entities
                IQueryable<OrderDetail> query = _appDbContext.OrderDetails
                                                            .Include(o => o.Order)
                                                                .ThenInclude(o => o.Customer)
                                                            .Include(o => o.Feedback)
                                                            .Include(o => o.Payment);

                // Apply search query filter
                if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
                {
                    query = query.Where(o => o.Payment.PaymentMethod.Contains(paginationDetails.SearchQuery) ||
                                             o.Order.Customer.CustName.ToLower().Contains(paginationDetails.SearchQuery) ||
                                             o.OrderId.ToString().Contains(paginationDetails.SearchQuery));
                }

                // Apply order status filter
                if (paginationDetails.OrderStatus != OrderStatus.All)
                {
                    query = query.Where(o => o.Order.Status == paginationDetails.OrderStatus.ToString());
                }

                // Apply date range filter
                if (paginationDetails.DateRange != TimePeriod.All)
                {
                    DateTime endDate = DateTime.Now;
                    DateTime startDate = paginationDetails.DateRange switch
                    {
                        TimePeriod.LastSevenDays => endDate.AddDays(-7),
                        TimePeriod.LastThirtyDays => endDate.AddDays(-30),
                        TimePeriod.CurrentMonth => new DateTime(endDate.Year, endDate.Month, 1),
                        _ => DateTime.MinValue,
                    };
                    query = query.Where(o => o.Createdat >= startDate && o.Createdat <= endDate);
                }

                // Apply custom date range filter
                DateTime fromDate = paginationDetails.FromDate.ToDateTime(new TimeOnly(0, 0));
                DateTime toDate = paginationDetails.ToDate.ToDateTime(new TimeOnly(23, 59, 59));

                if (fromDate != DateTime.MinValue || toDate != DateTime.MaxValue)
                {
                    query = query.Where(o => o.Createdat >= fromDate && o.Createdat <= toDate);
                }
                // Apply sorting
                query = paginationDetails.SortColumn.ToLower() switch
                {
                    "id" => (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.OrderId) : query.OrderByDescending(o => o.OrderId),
                    "date" => (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Createdat) : query.OrderByDescending(o => o.Createdat),
                    "customer" => (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Order.Customer.CustName) : query.OrderByDescending(o => o.Order.Customer.CustName),
                    "status" => (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Order.Status) : query.OrderByDescending(o => o.Order.Status),
                    "paymetmethod" => (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Payment.PaymentMethod) : query.OrderByDescending(o => o.Payment.PaymentMethod),
                    "totalamount" => (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Payment.ActualPrice) : query.OrderByDescending(o => o.Payment.ActualPrice),
                    _ => query.OrderBy(o => o.OrderId),// Default sorting
                };

                // Count filtered results
                paginationDetails.TotalRecords = await query.CountAsync();

                if (paginationDetails.PageSize == 0) paginationDetails.PageSize = paginationDetails.TotalRecords;
                return await query
                                .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                                .Take(paginationDetails.PageSize)
                                .ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<OrderDetail?> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            try
            {
                return await _appDbContext.OrderDetails
                                            .Include(o => o.Order)
                                                .ThenInclude(c => c.Customer)
                                            .Include(o => o.Order)
                                                .ThenInclude(c => c.InvoiceItemModifierMappings)
                                                .ThenInclude(i => i.Item)
                                            .Include(o => o.Order)
                                                .ThenInclude(c => c.InvoiceItemModifierMappings)
                                                .ThenInclude(i => i.Modifier)
                                            .Include(p => p.Payment)
                                            .Include(o => o.Order)
                                                .ThenInclude(c => c.Invoices)
                                            .Where(o => o.OrderId == orderId)
                                            .FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<Order>> GetOrdersAsync(PaginationDetails paginationDetails)
        {
            IQueryable<Order> query = _appDbContext.Orders
                                           .Include(o => o.Invoices)
                                           .Include(o => o.PaymentDetails)
                                           .Include(o => o.OrderDetails)
                                               .ThenInclude(od => od.Payment)
                                           .Include(o => o.Customer)
                                               .ThenInclude(c => c.WaitingLists)
                                           .Include(o => o.InvoiceItemModifierMappings)
                                               .ThenInclude(iim => iim.Item);
            // Apply date range filter
            if (paginationDetails.DateRange != TimePeriod.All)
            {
                DateTime endDate = DateTime.Now;
                DateTime startDate = paginationDetails.DateRange switch
                {
                    TimePeriod.LastSevenDays => endDate.AddDays(-7),
                    TimePeriod.LastThirtyDays => endDate.AddDays(-30),
                    TimePeriod.CurrentMonth => new DateTime(endDate.Year, endDate.Month, 1),
                    _ => DateTime.MinValue,
                };
                query = query.Where(o => o.Createat >= startDate && o.Createat <= endDate);
            }
            if (paginationDetails.SortColumn == Constants.SORT_BY_DATE)
            {
                query = query.OrderBy(o => o.Createat);
            }
            // Apply custom date range filter
            DateTime fromDate = paginationDetails.FromDate.ToDateTime(new TimeOnly(0, 0));
            DateTime toDate = paginationDetails.ToDate.ToDateTime(new TimeOnly(23, 59, 59));
            if (fromDate != DateTime.MinValue || toDate != DateTime.MaxValue)
            {
                query = query.Where(o => o.Createat >= fromDate && o.Createat <= toDate);
            }
            return await query.ToListAsync();
        }
        public async Task<List<Order>> GetOrdersDataByCutomerIdAsync(int customerId)
        {
            try
            {
                // Query the orders for the given customer
                return await _appDbContext.Orders
                                          .Include(o => o.Customer)
                                          .Include(o => o.PaymentDetails)
                                          .Include(o => o.InvoiceItemModifierMappings)
                                          .Where(o => o.CustomerId == customerId)
                                          .ToListAsync();

            }
            catch
            {
                throw;
            }
        }

        public async Task<Order?> GetOrderDetailsByTableAssign(int tableId)
        {
            try
            {
                return await _appDbContext.Orders
                                        .Include(o => o.PaymentDetails)
                                        .Include(o => o.OrderDetails)
                                        .Where(o => (o.Status == "InProgress" || o.Status == "Pending") && o.OrderDetails.Any(od => od.TableId.Contains(tableId)))
                                        .FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<ResponseResult> AddOrderAsync(Order newOrder)
        {
            try
            {
                _appDbContext.Orders.Add(newOrder);
                await _appDbContext.SaveChangesAsync();
                result.Data = newOrder.OrderId;
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.ORDER);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> AddOrderDetialsAsync(OrderDetail orderDetail)
        {
            try
            {
                _appDbContext.OrderDetails.Add(orderDetail);
                await _appDbContext.SaveChangesAsync();
                result.Data = orderDetail.OrderId;
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.ORDERDETAILS);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> UpdateOrderAsync(OrderDetail existingOrder)
        {
            try
            {
                _appDbContext.OrderDetails.Update(existingOrder);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.ORDERDETAILS);
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
