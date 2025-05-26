using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface IOrderAppService
    {
        Task<ResponseResult> AssignTable(waitingTokenVM tokenDetails);

        Task<ResponseResult> GetSectionList();
        Task<ResponseResult> GetTableViews(int sectionId);
        Task<ResponseResult> GetWaitingListBySectionId(int sectionId);
    }
}
