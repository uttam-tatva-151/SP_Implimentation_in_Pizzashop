using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface IMenuService
    {
        Task<ResponseResult> GetDefaultMenu(PaginationDetails paginationDetails);

    #region Category
        Task<ResponseResult> AddCategory(CategoryDetails newCategory);
        Task<ResponseResult> EditCategory(CategoryDetails updateCategory);
        Task<ResponseResult> DeleteCategory(int categoryId,int editorId);
    #endregion
    #region Item
        Task<AddItem> GetItemById(int itemId);
        Task<ResponseResult> GetItems(int id,PaginationDetails paginationDetails);
        Task<ResponseResult> AddItem(AddItem newItem);
        Task<ResponseResult> UpdateItem(AddItem UpdateItem);
        Task<ResponseResult> DeleteItem(int itemId,int editorId);
        Task<ResponseResult> DeleteMultipleItems(int[] ids,int editorId);

    #endregion

    #region Modifier
        Task<ResponseResult> GetModifiers(int id,PaginationDetails paginationDetails);
        Task<ResponseResult> GetModifierByModifierId(int modifierId);
        Task<List<ModifierDetails>> GetModifiersByGroupId(int groupId);

        Task<ResponseResult> AddNewModifier(ModifierVM newModifer);
        Task<ResponseResult> EditModifier(ModifierVM editModifier);
        Task<ResponseResult> DeleteModifier(int itemId, int editorId);
        Task<ResponseResult> DeleteMultipleModifiers(int[] ids, int editorId);
        Task<List<ModifierDetails>> GetAllModifiers(PaginationDetails paginationDetails);
    #endregion
        Task<ResponseResult> DeleteModifierGroup(int modifierGroupId,int editorId);
        Task<ResponseResult> AddModifierGroup(ModifierGroupVM newGroup);
        Task<ResponseResult> UpdateModifierGroup(ModifierGroupVM updateGroup);
    }
}
