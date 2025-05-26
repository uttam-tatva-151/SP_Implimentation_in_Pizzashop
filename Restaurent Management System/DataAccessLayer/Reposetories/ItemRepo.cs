using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class ItemRepo : IItemRepo
    {
        private readonly AppDbContext _appDbContext;

        public ItemRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();


        public async Task<List<Item>> GetItemsByCategoryId(int id, PaginationDetails paginationDetails)
        {
            IQueryable<Item> query = _appDbContext.Items.Include(i=>i.FavoritesItems).AsNoTracking().Where(a => a.Isactive == true);
            if (id > 0)
            {
                query = query.Where(a => a.CategoryId == id);
            }
            if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
            {
                query = query.Where(a => a.ItemName.ToLower().Contains(paginationDetails.SearchQuery.ToLower()));
            }
            paginationDetails.TotalRecords = await query.CountAsync(); // Count filtered results
            if (paginationDetails.PageSize == 0) paginationDetails.PageSize = paginationDetails.TotalRecords;
            return await query
                        .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                        .Take(paginationDetails.PageSize)
                        .ToListAsync();

        }

        public async Task<ResponseResult> AddItemAsync(Item newItem)
        {
            try
            {
                _appDbContext.Items.Add(newItem);
                await _appDbContext.SaveChangesAsync();
                result.Data = newItem.ItemId;
                result.Message =MessageHelper.GetSuccessMessageForUpdateOperation(Constants.ITEM);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> UpdateItemAsync(Item updateItem)
        {
            try
            {
                _appDbContext.Items.Update(updateItem);
                await _appDbContext.SaveChangesAsync();
                result.Message =MessageHelper.GetSuccessMessageForUpdateOperation(Constants.ITEM);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> MassUpdateItemsAsync(List<Item> items)
        {
            try
            {
                foreach (Item item in items)
                {
                    _appDbContext.Items.Update(item);
                }
                await _appDbContext.SaveChangesAsync();
                result.Message =MessageHelper.GetSuccessMessageForUpdateOperation(Constants.ITEM_LIST);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            return result;

        }






        public async Task<ResponseResult> DeleteItemByItemIdAsync(int itemId)
        {
            Item? item = await _appDbContext.Items.FirstOrDefaultAsync(i => i.ItemId == itemId);
            if (item == null)
            {
                result.Message =MessageHelper.GetWarningMessageForInvalidInput(Constants.ITEM);
                result.Status = ResponseStatus.NotFound;
                return result;
            }
            item.Isactive = false;
            await _appDbContext.SaveChangesAsync();
            result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.ITEM);
            result.Status = ResponseStatus.Success;
            return result;


        }



        public async Task DeleteAllItemsByCategoryIdAsync(int categoryId, int editorId)
        {
            List<Item> items = await _appDbContext.Items.Where(i => i.CategoryId == categoryId).ToListAsync();
            foreach (Item item in items)
            {
                item.Isactive = false;
                item.Modifyby = editorId;
                item.Modifyat = DateTime.Now;
                _appDbContext.Items.Update(item);
            }
            await _appDbContext.SaveChangesAsync();
        }




        public async Task<int> GetItemIdByItemName(string itemName)
        {

            int itemId = await _appDbContext.Items.Where(x => x.ItemName == itemName).Select(i => i.ItemId).FirstOrDefaultAsync();
            return itemId;
        }
        public async Task<string> GetItemNameByIdAsync(int itemId)
        {

            string? itemName = await _appDbContext.Items.Where(x => x.ItemId == itemId).Select(i => i.ItemName).FirstOrDefaultAsync();
            return itemName ?? string.Empty;
        }

        public async Task<Item?> GetItemById(int itemId)
        {
            Item? item = await _appDbContext.Items
                                    .Where(x => x.ItemId == itemId && x.Isactive == true)
                                    .Include(x => x.ItemModifierGroupsMappings) // Include Modifier Group Mappings
                                        .ThenInclude(mapping => mapping.Mg) // Include Modifier Group details
                                            .ThenInclude(group => group.ModifierModifierGroupRelations) // Include Modifier Group details
                                                .ThenInclude(modifier => modifier.Modifier) // Include Modifier details
                                    .FirstOrDefaultAsync();     
                return item;
        }
        public async Task<List<Item>> GetItemListByIds(int[] itemIds)
        {
            List<Item> items = await _appDbContext.Items.Where(x => itemIds.Contains(x.ItemId) && x.Isactive == true).ToListAsync();
            return items;
        }

        public async Task<Dictionary<int, decimal>> GetDefaultTaxesForItemsAsync(List<int> itemIds)
    {
        return await _appDbContext.Items.AsNoTracking()
            .Where(item => itemIds.Contains(item.ItemId))
            .ToDictionaryAsync(item => item.ItemId, item => item.TaxPercentage ?? 0);
    }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _appDbContext.Items.Include(i=>i.FavoritesItems).Where(i=>i.Isactive == true).ToListAsync();
        }
    }

}
