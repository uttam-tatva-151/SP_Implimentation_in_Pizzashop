using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class ItemModifierRepo : IItemModifierRepo
    {
        private readonly AppDbContext _appDbContext;

        public ItemModifierRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();
        public async Task<ResponseResult> AddMultipleEntriesAsync(List<ItemModifierGroupsMapping> itemModifiers)
        {
            try
            {
                _appDbContext.ItemModifierGroupsMappings.AddRange(itemModifiers);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.MAPPING_RELATIONS);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<List<ItemModifierGroupsMapping>> GetItemModifierDetails(int itemId)
        {
            return await _appDbContext.ItemModifierGroupsMappings.AsNoTracking().Where(x => x.ItemId == itemId).ToListAsync();
        }

        public async Task<List<int>> GetRelationsByItemIdAsync(int itemId)
        {
            List<int> relationIds = await _appDbContext.ItemModifierGroupsMappings.AsNoTracking().Where(x => x.ItemId == itemId).Select(x => x.ImId).ToListAsync();
            return relationIds;
        }
        public async Task<ResponseResult> RemoveMultipleEntriesAsync(List<int> ids)
        {
            try
            {
                List<ItemModifierGroupsMapping> rowsToRemove = await _appDbContext.ItemModifierGroupsMappings
                                                                                                            .Where(x => ids.Contains(x.ImId))
                                                                                                            .ToListAsync();

                // Remove all at once
                if (rowsToRemove.Any())
                {
                    _appDbContext.ItemModifierGroupsMappings.RemoveRange(rowsToRemove);
                    await _appDbContext.SaveChangesAsync();
                }

                await _appDbContext.SaveChangesAsync();
                result.Status = ResponseStatus.Success;
                result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.MAPPING_RELATIONS);
            }
            catch (Exception ex)
            {
                result.Status = ResponseStatus.Error;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<List<ItemModifierGroupsMapping>> GetMappingsByItemIdAsync(int itemId)
        {
            return await _appDbContext.ItemModifierGroupsMappings.Include(m => m.Item).Include(m => m.Mg).AsNoTracking().Where(x => x.ItemId == itemId).ToListAsync();
        }

        public async Task<ResponseResult> UpdateMultipleEntriesAsync(List<ItemModifierGroupsMapping> toUpdate)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                // Because of i am facing error in UPDATE multiple entries , i wrote logic of update like this instead of tradition way.
                // Error is about tracking that mapping row's primary key somewhere else at same time.

                List<int> ids = new();
                foreach (ItemModifierGroupsMapping mapping in toUpdate)
                {
                    ids.Add(mapping.ImId);
                }
                List<ItemModifierGroupsMapping> existingEntities = await _appDbContext.ItemModifierGroupsMappings
                    .Where(x => ids.Contains(x.ImId))
                    .ToListAsync();
                foreach (ItemModifierGroupsMapping mapping in toUpdate)
                {
                    ItemModifierGroupsMapping? existingEntity = existingEntities.FirstOrDefault(x => x.ImId == mapping.ImId);

                    if (existingEntity != null)
                    {
                        _appDbContext.Entry(existingEntity).CurrentValues.SetValues(mapping);
                    }
                }

                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.MAPPING_RELATIONS);
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
