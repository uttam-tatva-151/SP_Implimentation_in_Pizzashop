using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("categories")]
    public partial class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("category_name")]
        [StringLength(50)]
        public string CategoryName { get; set; } = null!;

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
        [InverseProperty("CategoryCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [InverseProperty("Category")]
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();

        [ForeignKey("Modifyby")]
        [InverseProperty("CategoryModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }
    }
}
