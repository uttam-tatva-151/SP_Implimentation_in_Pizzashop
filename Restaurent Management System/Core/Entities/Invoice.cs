using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("invoices")]
    public partial class Invoice
    {
        [Key]
        [Column("invoiceId")]
        public int InvoiceId { get; set; }

        [Column("orderId")]
        public int OrderId { get; set; }

        [Column("invoiceNumber")]
        [StringLength(15)]
        public string InvoiceNumber { get; set; } = null!;

        [Column("createAt")]
        public TimeOnly CreateAt { get; set; }

        [Column("modifyAt")]
        public TimeOnly? ModifyAt { get; set; }

        [Column("createBy")]
        public int CreateBy { get; set; }

        [Column("modifyBy")]
        public int? ModifyBy { get; set; }

        [Required]
        [Column("isactive")]
        public bool? Isactive { get; set; }

        [ForeignKey("CreateBy")]
        [InverseProperty("InvoiceCreateByNavigations")]
        public virtual Userauthentication CreateByNavigation { get; set; } = null!;

        [InverseProperty("Invoice")]
        public virtual ICollection<InvoiceItemModifierMapping> InvoiceItemModifierMappings { get; set; } = new List<InvoiceItemModifierMapping>();

        [InverseProperty("Invoice")]
        public virtual ICollection<InvoiceTaxesMapping> InvoiceTaxesMappings { get; set; } = new List<InvoiceTaxesMapping>();

        [ForeignKey("ModifyBy")]
        [InverseProperty("InvoiceModifyByNavigations")]
        public virtual Userauthentication? ModifyByNavigation { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("Invoices")]
        public virtual Order Order { get; set; } = null!;
    }
}
