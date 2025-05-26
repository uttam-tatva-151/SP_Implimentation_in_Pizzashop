using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class FavoriteItemRepo : IFavoriteItemRepo
    {
        private readonly AppDbContext _appDbContext;

        public FavoriteItemRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        readonly ResponseResult result = new();
        public async Task<ResponseResult> AddToFavoritesAsync(FavoritesItem favoriteItem)
        {
            try{
                _appDbContext.FavoritesItems.Add(favoriteItem);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.ITEM);
                result.Status = ResponseStatus.Success;
            }catch(Exception ex){
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
                return result;
        }

        public Task<FavoritesItem?> GetFavoriteItemById(int itemId, int editorId)
        {
            try{
                return _appDbContext.FavoritesItems.Include(f=>f.Item).AsNoTracking().Where(i=>i.ItemId == itemId && i.Isactive==true).FirstOrDefaultAsync();
            }catch{
                throw;
            }
        }

        public async Task<List<FavoritesItem>> GetFavoriteItems(string searchQuery)
        {
            IQueryable<FavoritesItem> query = _appDbContext.FavoritesItems.Include(f=>f.Item).AsNoTracking();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(f => f.Item.ItemName.ToLower().Contains(searchQuery.ToLower()));
            }

            return await query.Where(i=>i.Isactive==true).ToListAsync();
        }

        public async Task<ResponseResult> UpdateAtFavoritesAsync(FavoritesItem favoritesItem)
        {
            try{
                _appDbContext.FavoritesItems.Update(favoritesItem);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.ITEM);
                result.Status = ResponseStatus.Success;
            }catch(Exception ex){
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
    }
}
