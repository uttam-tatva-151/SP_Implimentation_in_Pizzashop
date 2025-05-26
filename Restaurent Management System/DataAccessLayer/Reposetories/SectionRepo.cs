using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class SectionRepo : ISectionRepo
    {
        private readonly AppDbContext _appDbContext;

        public SectionRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        readonly ResponseResult result = new();
        public async Task<List<Section>> GetAllSectonsAsync()
        {
            return await _appDbContext.Sections
                           .Include(s => s.WaitingLists)
                           .Include(s=>s.Tables)
                           .OrderBy(c => c.SectionId)
                           .Where(s => s.Isdeleted == false)
                           .ToListAsync();
        }
        public async Task<ResponseResult> AddSectionAsync(Section section)
        {
            try
            {
                int sectionCount = await _appDbContext.Sections.CountAsync();
                if (section == null)
                {
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.SECTION);
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    section.SectionId = sectionCount + 1;
                    _appDbContext.Sections.Add(section);
                    await _appDbContext.SaveChangesAsync();
                    result.Data = section.SectionId;
                    result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.SECTION);
                    result.Status = ResponseStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateSectionAsync(Section updateSection)
        {
            try
            {
                _appDbContext.Sections.Update(updateSection);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.SECTION);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<Section?> GetSectionAsync(int sectionId)
        {
            return await _appDbContext.Sections.Where(s => s.SectionId == sectionId && s.Isdeleted == false).FirstOrDefaultAsync();
        }
    }
}
