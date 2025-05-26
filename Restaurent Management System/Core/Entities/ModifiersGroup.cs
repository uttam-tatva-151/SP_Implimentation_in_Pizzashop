using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("modifiers_group")]
    [Index("MgName", Name = "modifiers_group_mg_name_key", IsUnique = true)]
    public partial class ModifiersGroup
    {
        [Key]
        [Column("mg_id")]
        public int MgId { get; set; }

        [Column("mg_name")]
        [StringLength(30)]
        public string MgName { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

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
        [InverseProperty("ModifiersGroupCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [InverseProperty("Mg")]
        public virtual ICollection<ItemModifierGroupsMapping> ItemModifierGroupsMappings { get; set; } = new List<ItemModifierGroupsMapping>();

        [InverseProperty("Group")]
        public virtual ICollection<ModifierModifierGroupRelation> ModifierModifierGroupRelations { get; set; } = new List<ModifierModifierGroupRelation>();

        [ForeignKey("Modifyby")]
        [InverseProperty("ModifiersGroupModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }
    }
}
