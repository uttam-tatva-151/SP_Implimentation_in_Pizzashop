using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("items")]
    [Index("CategoryId", "ItemName", Name = "unique_item_name_per_category", IsUnique = true)]
    public partial class Item
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("item_name")]
        [StringLength(30)]
        public string ItemName { get; set; } = null!;

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

        [Column("item_type")]
        [StringLength(15)]
        public string ItemType { get; set; } = null!;

        [Required]
        [Column("IsDefaultTAX")]
        public bool? IsDefaultTax { get; set; }

        [Column("short_Code")]
        [StringLength(15)]
        public string? ShortCode { get; set; }

        [Required]
        [Column("is_available")]
        public bool? IsAvailable { get; set; }

        [Column("photo_data")]
        public byte[]? PhotoData { get; set; }

        [Column("tax_percentage")]
        [Precision(5, 3)]
        public decimal? TaxPercentage { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Items")]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey("Createby")]
        [InverseProperty("ItemCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [InverseProperty("Item")]
        public virtual ICollection<FavoritesItem> FavoritesItems { get; set; } = new List<FavoritesItem>();

        [InverseProperty("Item")]
        public virtual ICollection<InvoiceItemModifierMapping> InvoiceItemModifierMappings { get; set; } = new List<InvoiceItemModifierMapping>();

        [InverseProperty("Item")]
        public virtual ICollection<ItemModifierGroupsMapping> ItemModifierGroupsMappings { get; set; } = new List<ItemModifierGroupsMapping>();

        [ForeignKey("Modifyby")]
        [InverseProperty("ItemModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }
    }
}
