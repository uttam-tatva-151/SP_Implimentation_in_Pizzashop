using PMSCore.Beans;
using PMSCore.DTOs;

namespace PMSData.Interfaces
{
    public interface IInvoiceItemMappingRepo
    {
        Task<ResponseResult> AddMappingAsync(InvoiceItemModifierMapping newItemMapping);
        Task<ResponseResult> DeleteMappingsAsync(List<InvoiceItemModifierMapping> mappingsToDelete);
        Task<List<InvoiceItemModifierMapping>> GetItemsForInvoiceAsync(int orderId);
        Task<List<InvoiceItemModifierMapping>> GetItemsForKOTAsync(int orderId);
        Task<ResponseResult> UpdateItemMappingAsync(InvoiceItemModifierMapping invoiceItemModifierMapping);
        Task<ResponseResult> UpdateMultipleItemMappingsAsync(List<InvoiceItemModifierMapping> invoiceItemModifierMapping);
        Task<ResponseResult> UpdateInvoiceItemModifierMappingsAsync(List<UpdateInvoiceItemModifierMappingDTO> invoiceItemModifierMapping);
    }
}
