using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces
{
    public interface IItemRepo
    {

        Task<ResponseResult> MassUpdateItemsAsync(List<Item> items);
        Task DeleteAllItemsByCategoryIdAsync(int categoryId, int editorId);
        Task<List<Item>> GetItemListByIds(int[] itemIds);
        Task<Item?> GetItemById(int itemId);
        Task<List<Item>> GetItemsByCategoryId(int id, PaginationDetails paginationDetails);
        Task<ResponseResult> UpdateItemAsync(Item updateItem);
        Task<ResponseResult> AddItemAsync(Item newItem);
        Task<Dictionary<int, decimal>> GetDefaultTaxesForItemsAsync(List<int> itemIds);
        Task<List<Item>> GetAllItemsAsync();
    }
}
