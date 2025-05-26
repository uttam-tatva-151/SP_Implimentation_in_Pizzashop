using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces
{
    public interface ISectionRepo
    {
        Task<List<Section>> GetAllSectonsAsync();
        Task<ResponseResult> AddSectionAsync(Section newSection);
        Task<ResponseResult> UpdateSectionAsync(Section updateSection);
        Task<Section?> GetSectionAsync(int sectionId);
    }
}
