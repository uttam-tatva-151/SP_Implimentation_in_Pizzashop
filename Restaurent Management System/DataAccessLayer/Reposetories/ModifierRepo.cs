using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class ModifierRepo : IModifierRepo
    {

        private readonly AppDbContext _appDbContext;

        public ModifierRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();


        #region Modifier Group

        public async Task<int> GetModifierGroupIdByName(string groupName)
        {
            return await _appDbContext.ModifiersGroups.AsNoTracking().Where(m => m.MgName == groupName).Select(m => m.MgId).FirstOrDefaultAsync();
        }
        public async Task<ResponseResult> AddModifierGroupAsync(ModifiersGroup newGroup)
        {
            try
            {

                _appDbContext.ModifiersGroups.Add(newGroup);
                await _appDbContext.SaveChangesAsync();

                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.MODIFIER_GROUP);
                result.Status = ResponseStatus.Success;

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<List<ModifiersGroup>> GetAllModifierGroups()
        {
            return await _appDbContext.ModifiersGroups
                                    .OrderBy(g => g.MgId)
                                    .Where(g => g.Isactive == true)
                                    .ToListAsync();
        }
        public async Task<ResponseResult> UpdateModifierGroupAsync(ModifiersGroup modifiersGroup)
        {
            try
            {
                _appDbContext.ModifiersGroups.Update(modifiersGroup);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.MODIFIER_GROUP);;
                result.Status = ResponseStatus.Success;

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ModifiersGroup?> GetModifierGroupById(int id)
        {
            ModifiersGroup? modifierGroup = await _appDbContext.ModifiersGroups.FirstOrDefaultAsync(i => i.MgId == id && i.Isactive == true);
            return modifierGroup;
        }
        #endregion
        #region Modifier
        public async Task<ResponseResult> AddModifierAsync(Modifier newModifer)
        {
            try
            {
                _appDbContext.Modifiers.Add(newModifer);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.MODIFIER);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> UpdateModifierAsync(Modifier updateModifer)
        {
            try
            {
                _appDbContext.Modifiers.Update(updateModifer);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.MODIFIER);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            return result;
        }



        #endregion
        #region Modifier Group Relation

        public async Task<ResponseResult> AddNewModifierAndGroupRelationsAsync(List<ModifierModifierGroupRelation> newGroupMapping)
        {
            try
            {
                foreach (ModifierModifierGroupRelation newRow in newGroupMapping)
                {
                    _appDbContext.ModifierModifierGroupRelations.Add(newRow);
                }
                await _appDbContext.SaveChangesAsync();
                result.Message =MessageHelper.GetSuccessMessageForAddOperation(Constants.MAPPING_RELATIONS);;
                result.Status = ResponseStatus.Success;

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> RemoveModifierAndGroupRelationsAsync(List<ModifierModifierGroupRelation> existingRelations)
        {
            try
            {
                foreach (ModifierModifierGroupRelation existingRow in existingRelations)
                {
                    _appDbContext.ModifierModifierGroupRelations.Remove(existingRow);
                }
                await _appDbContext.SaveChangesAsync();
                result.Message =MessageHelper.GetSuccessMessageForDeleteOperation(Constants.MAPPING_RELATIONS);;
                result.Status = ResponseStatus.Success;

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<List<ModifierModifierGroupRelation>> GetModifiersRelationsByGroupid(int groupId)
        {
            return await _appDbContext.ModifierModifierGroupRelations.Where(m => m.GroupId == groupId).ToListAsync();
        }

        #endregion

        public async Task<ResponseResult> DeleteModifierGroupByModifierGroupId(int modifierGroupId)
        {
            ModifiersGroup? modifiersGroup = await _appDbContext.ModifiersGroups.FirstOrDefaultAsync(i => i.MgId == modifierGroupId);
            if (modifiersGroup == null)
            {
                result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.MODIFIER_GROUP);
                result.Status = ResponseStatus.NotFound;
                return result;
            }
            modifiersGroup.Isactive = false;
            await _appDbContext.SaveChangesAsync();
            // DeleteAllItemsByModifierGroupId(id);
            result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.MODIFIER_GROUP);;
            result.Status = ResponseStatus.Success;
            return result;
        }


        public async Task<List<Modifier>> GetModifiersByModifierGroupId(int id, PaginationDetails paginationDetails)
        {
            IQueryable<ModifierModifierGroupRelation> query = _appDbContext.ModifierModifierGroupRelations
                                        .AsNoTracking()
                                        .Include(m => m.Modifier)
                                        .Include(m => m.Group)
                                        .Where(g => g.GroupId == id && g.Modifier.Isactive == true);
            if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
            {
                query = query.Where(a => a.Modifier.MName.ToLower().Contains(paginationDetails.SearchQuery.ToLower()));
            }
            paginationDetails.TotalRecords = await query.CountAsync();
            return await query
                         .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                         .Take(paginationDetails.PageSize)
                         .Select(m => m.Modifier)
                         .ToListAsync();
        }
        public async Task<ResponseResult> DeleteModifierByModifierId(int modifierId)
        {
            Modifier? modifier = await _appDbContext.Modifiers.FirstOrDefaultAsync(i => i.MId == modifierId);
            if (modifier == null)
            {
                result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.MODIFIER);
                result.Status = ResponseStatus.NotFound;
                return result;
            }
            modifier.Isactive = false;
            _appDbContext.Modifiers.Update(modifier);
            await _appDbContext.SaveChangesAsync();
            result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.MODIFIER);;
            result.Status = ResponseStatus.Success;
            return result;
        }


        public async Task<Modifier?> GetModifierByModifierIdAsync(int modifierId)
        {
            return await _appDbContext.Modifiers.Include(m=>m.ModifierModifierGroupRelations).AsNoTracking().FirstOrDefaultAsync(i => i.MId == modifierId && i.Isactive == true);
        }
        public async Task<List<Modifier>> GetAllModifiers(PaginationDetails paginationDetails)
        {
            try
            {
                IQueryable<Modifier> query = _appDbContext.Modifiers.AsNoTracking().Where(m => m.Isactive == true);

                if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
                {
                    paginationDetails.SearchQuery = paginationDetails.SearchQuery.Trim().ToLower();
                    query = query.Where(u => u.MName.ToLower().Contains(paginationDetails.SearchQuery.ToLower()));
                }

                paginationDetails.TotalRecords = await query.CountAsync(); // Count filtered results

                return await query.Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                                    .Take(paginationDetails.PageSize)
                                    .ToListAsync();

            }
            catch 
            {
                throw;
            }
        }


        public async Task<string?> GetModifierNameByIdAsync(int modifierId)
        {

            string? modifierName = await _appDbContext.Modifiers.AsNoTracking().Where(x => x.MId == modifierId).Select(i => i.MName).FirstOrDefaultAsync();
            return modifierName;
        }

        public async Task<List<Modifier>> GetModifiersByModifierGroupIdAsync(int groupId)
        {
            return await _appDbContext.ModifierModifierGroupRelations
                                        .AsNoTracking()
                                        .Include(m => m.Modifier)
                                        .Where(m => m.GroupId == groupId && m.Modifier.Isactive == true)
                                        .Select(m => m.Modifier)
                                        .ToListAsync();
        }

        public async Task<List<ModifierModifierGroupRelation>> GetModifiersRelationsByModifierId(int groupId)
        {
            return await _appDbContext.ModifierModifierGroupRelations
                                        .AsNoTracking()
                                        .Include(m => m.Modifier)
                                        .Where(m => m.ModifierId == groupId && m.Modifier.Isactive == true)
                                        .ToListAsync();
        }

        public Task<List<Modifier>> GetModifierListByIdsAsync(int[] ids)
        {
            return _appDbContext.Modifiers.AsNoTracking().Where(m => ids.Contains(m.MId)).ToListAsync();
        }

        public async Task<ResponseResult> MassUpdateModifiersAsync(List<Modifier> existingModifiers)
        {
            try{
                foreach (Modifier existingRow in existingModifiers)
                {
                    _appDbContext.Modifiers.Update(existingRow);
                }
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.MODIFIER_LIST);
                result.Status = ResponseStatus.Success;
            }catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public Task<Modifier?> GetModifierByNameAsync(string modifierName)
        {
            return _appDbContext.Modifiers.AsNoTracking().FirstOrDefaultAsync(m => m.MName.ToLower() == modifierName.ToLower() && m.Isactive == true);
        }
    }

}
