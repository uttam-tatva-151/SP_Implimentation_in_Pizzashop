using PMSCore.Beans;

namespace PMSData.Interfaces
{
    public interface ICategoryRepo
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<ResponseResult> AddCategory(Category newCategory);
        Task<Category?> GetCategoryByNameAsync(string categoryName);
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task<ResponseResult> UpdateCategory(Category newCategory);
        // Task<ResponseResult> DeleteCategoryByCategoryIdAsync(int id, int editorId);
    }
}
