
using PMSCore.Beans;

namespace PMSData.Interfaces
{
    public interface IFavoriteItemRepo
    {
        Task<ResponseResult> AddToFavoritesAsync(FavoritesItem favoriteItem);
        Task<FavoritesItem?> GetFavoriteItemById(int itemId, int editorId);
        Task<List<FavoritesItem>> GetFavoriteItems(string searchQuery);
        Task<ResponseResult> UpdateAtFavoritesAsync(FavoritesItem favoritesItem);
    }
}
