using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface IWaitingListService
    {
         Task<ResponseResult> AddWaitingToken(waitingTokenVM token);
        Task<ResponseResult> DeleteWaitingToken(int tokenId, int editorId);
        Task<ResponseResult> EditWaitingToken(waitingTokenVM token);
        Task<ResponseResult> GetCustomerDetails(string emailId);
        Task<ResponseResult> GetSectionList();
        Task<ResponseResult> GetWaitingListBySectionId(int sectionId);
        Task<ResponseResult> RemoveWaitingToken(waitingTokenVM tokenDetails);
    }
}
