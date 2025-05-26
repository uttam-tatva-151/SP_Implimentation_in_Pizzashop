using PMSCore.DTOs;
using PMSCore.ViewModel;

namespace PMSServices.Utilities.Mappers;

public class InvoiceItemModifierMappingMapper
{
    public static List<UpdateInvoiceItemModifierMappingDTO> MapKOTItemsToUpdateInvoiceItemModifierMappingDTOList(List<KOTVM.KOTItemsVM> kotItems,int orderId,int editorId)
    {
        List<UpdateInvoiceItemModifierMappingDTO> mappings = new();
        foreach (KOTVM.KOTItemsVM item in kotItems)
        {
            mappings.Add(MapToDTO(item,orderId,editorId));
        }
        return mappings;
    }

    private static UpdateInvoiceItemModifierMappingDTO MapToDTO(KOTVM.KOTItemsVM kotItem,int orderId,int editorId)
    {
        return new UpdateInvoiceItemModifierMappingDTO
        {
            ItemQuantity = kotItem.quantity,
            PreparedItems = kotItem.preparedItems,
            SpecialInstructions = kotItem.itemComments,
            GroupListId = kotItem.UniqueGroupId,
            UpdateBy = editorId,
            OrderId = orderId
        };
    }
}
