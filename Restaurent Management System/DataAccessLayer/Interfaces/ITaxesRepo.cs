using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces
{
    public interface ITaxesRepo
    {
        Task<ResponseResult> AddNewTaxAync(Taxis tax);
        Task<List<Taxis>> GetAllTaxesAsync(PaginationDetails paginationDetails);
        Task<ResponseResult> UpdateTaxAsync(Taxis tax);
        Task<Taxis> GetTaxByIdAsync(int taxId);
        Task<Taxis?> GetTaxByNameAsync(string taxName);
        Task<List<Taxis>> GetDefaultTaxesAsync();
        Task<ResponseResult> AddTaxisMappingAsync(List<InvoiceTaxesMapping> invoiceTaxMappingList);
        Task<List<InvoiceTaxesMapping>> GetTaxMappingsByInvoiceIdAsync(int invoiveId);
    }
}
