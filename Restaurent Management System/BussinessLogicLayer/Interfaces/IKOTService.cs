using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface IKOTService
    {
         Task<ResponseResult> GetKOTs(string status,int categoryId);
         Task<ResponseResult> UpdateKOTItems(List<KOTVM.KOTItemsVM> kotItems, int orderId,int editorId);
    }
}
