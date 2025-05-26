using PMSCore.Beans;

namespace PMSData.Interfaces
{
    public interface IItemModifierRepo
    {
        Task<ResponseResult> AddMultipleEntriesAsync(List<ItemModifierGroupsMapping> itemModifiers);
        Task<List<ItemModifierGroupsMapping>> GetItemModifierDetails(int itemId);
        Task<List<ItemModifierGroupsMapping>> GetMappingsByItemIdAsync(int itemId);
        Task<List<int>> GetRelationsByItemIdAsync(int itemId);
        Task<ResponseResult> RemoveMultipleEntriesAsync(List<int> ids);
        Task<ResponseResult> UpdateMultipleEntriesAsync(List<ItemModifierGroupsMapping> toUpdate);
    }
}
