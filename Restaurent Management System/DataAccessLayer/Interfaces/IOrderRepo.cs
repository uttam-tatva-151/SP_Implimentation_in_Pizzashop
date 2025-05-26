using PMSCore.Beans;
using PMSCore.DTOs;

namespace PMSData.Interfaces
{
    public interface IOrderRepo
    {
        Task<Dictionary<int, (int TotalOrders, DateOnly LastOrderDate)>> GetCustomerOrderDataAsync(List<int> customerIds);
        Task<OrderDetail?> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<List<KOTDTO>> GetKotDataAsync(string status, int categoryId);
        Task<List<OrderDetail>> GetOrderListAsync(PaginationDetails paginationDetails);
        Task<List<Order>> GetOrdersDataByCutomerIdAsync(int customerId);
        Task<Order?> GetOrderDetailsByTableAssign(int tableId);
        Task<ResponseResult> AddOrderAsync(Order newOrder);
        Task<ResponseResult> AddOrderDetialsAsync(OrderDetail orderDetail);
        Task<ResponseResult> UpdateOrderAsync(OrderDetail existingOrder);
        Task<List<Order>> GetOrdersAsync(PaginationDetails paginationDetails);
    }
}
