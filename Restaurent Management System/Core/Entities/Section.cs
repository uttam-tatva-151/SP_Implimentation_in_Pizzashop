using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("sections")]
    public partial class Section
    {
        [Key]
        [Column("section_id")]
        public int SectionId { get; set; }

        [Column("section_name")]
        [StringLength(30)]
        public string SectionName { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("createdat", TypeName = "timestamp without time zone")]
        public DateTime Createdat { get; set; }

        [Column("modifiedat", TypeName = "timestamp without time zone")]
        public DateTime? Modifiedat { get; set; }

        [Column("createdby")]
        public int Createdby { get; set; }

        [Column("modifiedby")]
        public int? Modifiedby { get; set; }

        [Column("isdeleted")]
        public bool Isdeleted { get; set; }

        [ForeignKey("Createdby")]
        [InverseProperty("SectionCreatedbyNavigations")]
        public virtual Userauthentication CreatedbyNavigation { get; set; } = null!;

        [ForeignKey("Modifiedby")]
        [InverseProperty("SectionModifiedbyNavigations")]
        public virtual Userauthentication? ModifiedbyNavigation { get; set; }

        [InverseProperty("Section")]
        public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

        [InverseProperty("Section")]
        public virtual ICollection<WaitingList> WaitingLists { get; set; } = new List<WaitingList>();
    }
}
