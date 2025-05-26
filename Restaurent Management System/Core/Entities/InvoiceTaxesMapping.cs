using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("invoice_taxes_mapping")]
    public partial class InvoiceTaxesMapping
    {
        [Key]
        [Column("invoice_tax_relationId")]
        public int InvoiceTaxRelationId { get; set; }

        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("tax_id")]
        public int TaxId { get; set; }

        [Column("invoice_tax_value")]
        [Precision(7, 2)]
        public decimal InvoiceTaxValue { get; set; }

        [Column("tax_type", TypeName = "character varying")]
        public string? TaxType { get; set; }

        [ForeignKey("InvoiceId")]
        [InverseProperty("InvoiceTaxesMappings")]
        public virtual Invoice Invoice { get; set; } = null!;

        [ForeignKey("TaxId")]
        [InverseProperty("InvoiceTaxesMappings")]
        public virtual Taxis Tax { get; set; } = null!;
    }
}
