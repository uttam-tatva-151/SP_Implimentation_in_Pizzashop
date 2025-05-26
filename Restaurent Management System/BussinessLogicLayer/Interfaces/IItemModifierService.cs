using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;

namespace PMSServices.Interfaces
{
    public interface IItemModifierService
    {
        Task<ResponseResult> AddMultipleEntriesOfItemModifier(List<ItemModifierGroupRelation>? itemModifiers);
        Task<List<ItemModifierGroupRelation>> GetItemModifierDetails(int itemId);
        Task<ResponseResult> UpdateItemModifiers(List<ItemModifierGroupRelation> itemModifierGroupRelations, int itemId );
    }
}
