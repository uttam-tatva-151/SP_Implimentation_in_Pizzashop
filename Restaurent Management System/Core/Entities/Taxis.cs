using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("taxes")]
    public partial class Taxis
    {
        [Key]
        [Column("tax_id")]
        public int TaxId { get; set; }

        [Column("tax_name")]
        [StringLength(30)]
        public string TaxName { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("tax_type")]
        [StringLength(15)]
        public string TaxType { get; set; } = null!;

        [Column("tax_value")]
        [Precision(7, 3)]
        public decimal TaxValue { get; set; }

        [Column("isdefault")]
        public bool Isdefault { get; set; }

        [Required]
        [Column("isenabled")]
        public bool? Isenabled { get; set; }

        [Column("createat", TypeName = "timestamp without time zone")]
        public DateTime Createat { get; set; }

        [Column("modifyat", TypeName = "timestamp without time zone")]
        public DateTime? Modifyat { get; set; }

        [Column("createby")]
        public int Createby { get; set; }

        [Column("modifyby")]
        public int? Modifyby { get; set; }

        [Column("iscontinued")]
        public bool Iscontinued { get; set; }

        [ForeignKey("Createby")]
        [InverseProperty("TaxisCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [InverseProperty("Tax")]
        public virtual ICollection<InvoiceTaxesMapping> InvoiceTaxesMappings { get; set; } = new List<InvoiceTaxesMapping>();

        [ForeignKey("Modifyby")]
        [InverseProperty("TaxisModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }
    }
}
