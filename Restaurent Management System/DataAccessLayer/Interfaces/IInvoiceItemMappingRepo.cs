using PMSCore.Beans;
using PMSCore.DTOs;

namespace PMSData.Interfaces
{
    public interface IInvoiceItemMappingRepo
    {
        Task<ResponseResult> UpdateMultipleItemMappingsAsync(List<InvoiceItemModifierMapping> invoiceItemModifierMapping);
        Task<List<InvoiceItemModifierMapping>> GetItemsForInvoiceAsync(int orderId);
        Task<ResponseResult> AddInvoiceItemModifierMappingsAsync(List<AddInvoiceItemModifierMappingInputDTO> mappings);
        Task<ResponseResult> UpdateInvoiceItemModifierMappingsAsync(List<UpdateInvoiceItemModifierMappingDTO> invoiceItemModifierMapping);
        Task<ResponseResult> DeleteInvoiceItemModifierMappingsAsync(List<UpdateInvoiceItemModifierMappingDTO> mappingsToDelete);
    }
}
