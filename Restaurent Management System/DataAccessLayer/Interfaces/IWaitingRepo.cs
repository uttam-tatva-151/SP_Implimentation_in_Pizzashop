using PMSCore.Beans;
namespace PMSData.Interfaces
{
    public interface IWaitingRepo
    {
        Task<ResponseResult> AddWaitingTokenAsync(WaitingList token);
        Task<List<WaitingList>> GetAllWaitingTokensAsync(int v);
        Task<WaitingList?> GetWaitingTokenByIdAsync(int tokenId);

        Task<List<WaitingList>> GetWaitingTokensBySectionAsync(int sectionId);
        Task<ResponseResult> UpdateWaitingToken(WaitingList waitingToken);
    }
}
