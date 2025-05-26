using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface ITaxesAndFeesService
    {
        Task<ResponseResult> AddNewTax(TaxDetails taxDetails);
        Task<ResponseResult> DeleteTax(int taxId,int editorId);
        Task<ResponseResult> GetTaxes(PaginationDetails paginationDetails);
        Task<ResponseResult> UpdateTax(TaxDetails taxDetails);
    }
}
