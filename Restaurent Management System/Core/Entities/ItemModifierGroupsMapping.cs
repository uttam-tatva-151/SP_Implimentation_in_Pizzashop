using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("item_modifier_groups_mapping")]
    [Index("ItemId", "MgId", Name = "item_modifiers_item_id_m_id_key", IsUnique = true)]
    public partial class ItemModifierGroupsMapping
    {
        [Key]
        [Column("im_id")]
        public int ImId { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("mg_id")]
        public int MgId { get; set; }

        [Column("min_modifiers")]
        public int MinModifiers { get; set; }

        [Column("max_modifiers")]
        public int MaxModifiers { get; set; }

        [ForeignKey("ItemId")]
        [InverseProperty("ItemModifierGroupsMappings")]
        public virtual Item Item { get; set; } = null!;

        [ForeignKey("MgId")]
        [InverseProperty("ItemModifierGroupsMappings")]
        public virtual ModifiersGroup Mg { get; set; } = null!;
    }
}
