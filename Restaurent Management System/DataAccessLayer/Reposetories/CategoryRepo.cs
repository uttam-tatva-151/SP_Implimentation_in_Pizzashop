using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly AppDbContext _appDbContext;

        public CategoryRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        private readonly ResponseResult result = new();

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _appDbContext.Categories
                            .AsNoTracking()
                            .OrderBy(c => c.CategoryId)
                            .Where(a => a.Isactive == true)
                            .ToListAsync();
        }
        public async Task<Category?> GetCategoryByNameAsync(string categoryName)
        {

            return await _appDbContext.Categories.AsNoTracking().Where(c => c.CategoryName.ToLower() == categoryName.ToLower()).FirstOrDefaultAsync();

        }
        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {

            return await _appDbContext.Categories.AsNoTracking().Where(c => c.CategoryId == categoryId).FirstOrDefaultAsync() ?? throw new KeyNotFoundException(MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CATEGORY));

        }

        public async Task<ResponseResult> AddCategory(Category newCategory)
        {
            try
            {
                _appDbContext.Categories.Add(newCategory);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.CATEGORY);
                result.Status = ResponseStatus.Success;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> UpdateCategory(Category category)
        {
            try
            {
                _appDbContext.Categories.Update(category);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.CATEGORY);
                result.Status = ResponseStatus.Success;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return result;
            }
        }
    }
}
