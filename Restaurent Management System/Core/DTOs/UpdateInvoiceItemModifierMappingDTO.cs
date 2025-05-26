using NpgsqlTypes;

namespace PMSCore.DTOs;

[PgName("invoice_item_modifier_mapping_dto")]
public class UpdateInvoiceItemModifierMappingDTO
{
    [PgName("item_quantity")]
    public int ItemQuantity { get; set; }
    [PgName("prepared_items")]
    public int PreparedItems { get; set; }
    [PgName("order_id")]
    public int OrderId { get; set; }
    [PgName("special_instructions")]
    public string? SpecialInstructions { get; set; }
    [PgName("group_list_id")]
    public string GroupListId { get; set; } = null!;
    [PgName("update_by")]
    public int UpdateBy { get; set; }
}
