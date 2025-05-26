using PMSCore.Beans;

namespace PMSData.Interfaces
{
    public interface IModifierRepo
    {
        Task<ResponseResult> AddModifierAsync(Modifier newModifer);
        Task<ResponseResult> AddModifierGroupAsync(ModifiersGroup newGroup);
        Task<ResponseResult> DeleteModifierByModifierId(int modifierId);
        Task<ResponseResult> DeleteModifierGroupByModifierGroupId(int modifierGroupId);
        Task<ResponseResult> UpdateModifierAsync(Modifier updateModifer);
        Task<List<ModifierModifierGroupRelation>> GetModifiersRelationsByGroupid(int groupId);
        Task<ResponseResult> RemoveModifierAndGroupRelationsAsync(List<ModifierModifierGroupRelation> existingRelations);
        Task<ResponseResult> AddNewModifierAndGroupRelationsAsync(List<ModifierModifierGroupRelation> newGroupMapping);
        Task<List<ModifiersGroup>> GetAllModifierGroups();
        Task<ModifiersGroup?> GetModifierGroupById(int id);
        Task<ResponseResult> UpdateModifierGroupAsync(ModifiersGroup updateGroup);
        Task<List<Modifier>> GetAllModifiers(PaginationDetails paginationDetails);
        Task<Modifier?> GetModifierByModifierIdAsync(int modifierId);
        Task<string?> GetModifierNameByIdAsync(int modifierId);
        Task<List<Modifier>> GetModifiersByModifierGroupId(int id, PaginationDetails paginationDetails);
        Task<List<Modifier>> GetModifiersByModifierGroupIdAsync(int groupId);

        Task<int> GetModifierGroupIdByName(string groupName);
        Task<List<Modifier>> GetModifierListByIdsAsync(int[] ids);
        Task<ResponseResult> MassUpdateModifiersAsync(List<Modifier> existingModifiers);
        Task<Modifier?> GetModifierByNameAsync(string modifierName);
    }
}
