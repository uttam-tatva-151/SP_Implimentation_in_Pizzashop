using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("waiting_list")]
    public partial class WaitingList
    {
        [Key]
        [Column("token_id")]
        public int TokenId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("priority_level")]
        public short PriorityLevel { get; set; }

        [Column("remarks")]
        public string? Remarks { get; set; }

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

        [Column("no_of_person")]
        public int NoOfPerson { get; set; }

        public int SectionId { get; set; }

        [ForeignKey("Createby")]
        [InverseProperty("WaitingListCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [ForeignKey("CustomerId")]
        [InverseProperty("WaitingLists")]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey("Modifyby")]
        [InverseProperty("WaitingListModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }

        [ForeignKey("SectionId")]
        [InverseProperty("WaitingLists")]
        public virtual Section Section { get; set; } = null!;
    }
}
