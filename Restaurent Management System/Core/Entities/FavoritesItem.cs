using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("favorites_items")]
    public partial class FavoritesItem
    {
        [Key]
        [Column("fi_id")]
        public int FiId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("item_id")]
        public int? ItemId { get; set; }

        [Column("createdat", TypeName = "timestamp without time zone")]
        public DateTime Createdat { get; set; }

        [Required]
        [Column("isactive")]
        public bool? Isactive { get; set; }

        [ForeignKey("ItemId")]
        [InverseProperty("FavoritesItems")]
        public virtual Item? Item { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("FavoritesItems")]
        public virtual Userauthentication User { get; set; } = null!;
    }
}
