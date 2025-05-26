using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("tables")]
    public partial class Table
    {
        [Key]
        [Column("table_id")]
        public int TableId { get; set; }

        [Column("table_name")]
        [StringLength(20)]
        public string TableName { get; set; } = null!;

        [Column("section_id")]
        public int SectionId { get; set; }

        [Column("capacity")]
        public int Capacity { get; set; }

        [Column("status")]
        [StringLength(10)]
        public string Status { get; set; } = null!;

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
        [InverseProperty("TableCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [ForeignKey("Modifyby")]
        [InverseProperty("TableModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }

        [ForeignKey("SectionId")]
        [InverseProperty("Tables")]
        public virtual Section Section { get; set; } = null!;
    }
}
