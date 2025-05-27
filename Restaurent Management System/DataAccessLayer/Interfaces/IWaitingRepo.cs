using PMSCore.Beans;
using PMSCore.DTOs;
namespace PMSData.Interfaces
{
    public interface IWaitingRepo
    {
        Task<ResponseResult> AddWaitingTokenAsync(WaitingTokenDTO token);
        Task<WaitingTokenDTO?> GetWaitingTokenByIdAsync(int tokenId);
        Task<List<WaitingTokenDTO>> GetWaitingTokensBySectionAsync(int sectionId);
        Task<ResponseResult> UpdateWaitingToken(WaitingTokenDTO waitingToken);
    }
}
