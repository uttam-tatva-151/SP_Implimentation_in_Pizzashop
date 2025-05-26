using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface IOrderAppMenuService
    {
        Task<ResponseResult> AddCustomerFeedback(CustomerReviewViewModel reviewData);
        Task<ResponseResult> AddToFavorites(int itemId, int editorId);
        Task<ResponseResult> GetCategoryList();
        Task<ResponseResult> GetDefaultMenu();
        Task<ResponseResult> GetMenuItemMapping(int itemId);
        Task<ResponseResult> GetMenuItems(bool favoritesItem, int categoryId, string searchQuery);
        Task<ResponseResult> GetOrderDetailsAsync(int orderId);
        Task<ResponseResult> RemoveFromFavorites(int itemId, int editorId);
        Task<ResponseResult> UpdateOrder(OrderExportDetails order);
        Task<ResponseResult> UpdateOrderStatus(string orderStatus,int OrderId,int editorId);
    }
}
