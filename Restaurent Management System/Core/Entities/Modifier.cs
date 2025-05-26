using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("modifiers")]
    public partial class Modifier
    {
        [Key]
        [Column("m_id")]
        public int MId { get; set; }

        [Column("m_name")]
        [StringLength(50)]
        public string MName { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("unit_price")]
        [Precision(7, 2)]
        public decimal UnitPrice { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_type")]
        [StringLength(10)]
        public string UnitType { get; set; } = null!;

        [Column("createat", TypeName = "timestamp without time zone")]
        public DateTime Createat { get; set; }

        [Column("modifyat", TypeName = "timestamp without time zone")]
        public DateTime? Modifyat { get; set; }

        [Column("createby")]
        public int Createby { get; set; }

        [Column("modifyby")]
        public int? Modifyby { get; set; }

        [Required]
        [Column("isactive")]
        public bool? Isactive { get; set; }

        [ForeignKey("Createby")]
        [InverseProperty("ModifierCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [InverseProperty("Modifier")]
        public virtual ICollection<InvoiceItemModifierMapping> InvoiceItemModifierMappings { get; set; } = new List<InvoiceItemModifierMapping>();

        [InverseProperty("Modifier")]
        public virtual ICollection<ModifierModifierGroupRelation> ModifierModifierGroupRelations { get; set; } = new List<ModifierModifierGroupRelation>();

        [ForeignKey("Modifyby")]
        [InverseProperty("ModifierModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }
    }
}
