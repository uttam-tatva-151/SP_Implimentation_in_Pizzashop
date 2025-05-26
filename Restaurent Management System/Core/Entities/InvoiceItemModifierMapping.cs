using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("invoice_item_modifier_mapping")]
    public partial class InvoiceItemModifierMapping
    {
        [Key]
        [Column("invoice_item_modifier_id")]
        public int InvoiceItemModifierId { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("modifier_id")]
        public int? ModifierId { get; set; }

        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("item_price")]
        [Precision(7, 2)]
        public decimal ItemPrice { get; set; }

        [Column("modifier_price")]
        [Precision(7, 2)]
        public decimal? ModifierPrice { get; set; }

        [Column("item_tax_percentage")]
        [Precision(5, 3)]
        public decimal? ItemTaxPercentage { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("special_instructions")]
        public string? SpecialInstructions { get; set; }
         [Column("group_list_id")]
        public string? GroupListId { get; set; }

        [Column("item_quantity")]
        public int ItemQuantity { get; set; }

        [Column("prepared_items")]
        public int PreparedItems { get; set; }

        [Column("createby")]
        public int Createby { get; set; }

        [Column("createat", TypeName = "timestamp without time zone")]
        public DateTime Createat { get; set; }

        [Column("modifiers_quantity")]
        public int? ModifiersQuantity { get; set; }

        [ForeignKey("Createby")]
        [InverseProperty("InvoiceItemModifierMappings")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [ForeignKey("InvoiceId")]
        [InverseProperty("InvoiceItemModifierMappings")]
        public virtual Invoice Invoice { get; set; } = null!;

        [ForeignKey("ItemId")]
        [InverseProperty("InvoiceItemModifierMappings")]
        public virtual Item Item { get; set; } = null!;

        [ForeignKey("ModifierId")]
        [InverseProperty("InvoiceItemModifierMappings")]
        public virtual Modifier? Modifier { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("InvoiceItemModifierMappings")]
        public virtual Order Order { get; set; } = null!;
    }
}
