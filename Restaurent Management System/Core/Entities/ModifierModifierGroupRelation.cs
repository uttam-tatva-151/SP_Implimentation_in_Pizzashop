using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("modifier_modifier_group_relation")]
    public partial class ModifierModifierGroupRelation
    {
        [Key]
        [Column("relationId")]
        public int RelationId { get; set; }

        [Column("modifierId")]
        public int ModifierId { get; set; }

        [Column("groupId")]
        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        [InverseProperty("ModifierModifierGroupRelations")]
        public virtual ModifiersGroup Group { get; set; } = null!;

        [ForeignKey("ModifierId")]
        [InverseProperty("ModifierModifierGroupRelations")]
        public virtual Modifier Modifier { get; set; } = null!;
    }
}
