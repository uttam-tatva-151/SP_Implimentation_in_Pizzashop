using NpgsqlTypes;

namespace PMSCore.DTOs;

[PgName("add_invoice_item_modifier_mapping")]
public class AddInvoiceItemModifierMappingInputDTO
{
    [PgName("item_id")]
    public int ItemId { get; set; }

    [PgName("modifier_id")]
    public int? ModifierId { get; set; }

    [PgName("invoice_id")]
    public int InvoiceId { get; set; }

    [PgName("item_price")]
    public decimal ItemPrice { get; set; }

    [PgName("modifier_price")]
    public decimal? ModifierPrice { get; set; }

    [PgName("item_tax_percentage")]
    public decimal? ItemTaxPercentage { get; set; }

    [PgName("order_id")]
    public int OrderId { get; set; }

    [PgName("special_instructions")]
    public string? SpecialInstructions { get; set; }

    [PgName("item_quantity")]
    public int ItemQuantity { get; set; }

    [PgName("prepared_items")]
    public int PreparedItems { get; set; }

    [PgName("createby")]
    public int CreateBy { get; set; }

    [PgName("modifiers_quantity")]
    public int? ModifiersQuantity { get; set; }

    [PgName("group_list_id")]
    public string? GroupListId { get; set; }
}
