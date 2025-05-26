using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("contry_list")]
    public partial class ContryList
    {
        [Key]
        [Column("contry_id")]
        public int ContryId { get; set; }

        [Column("contry_name")]
        [StringLength(50)]
        public string ContryName { get; set; } = null!;

        [Column("createAT")]
        public TimeOnly CreateAt { get; set; }

        [Column("modifyAt")]
        public TimeOnly? ModifyAt { get; set; }

        [Column("CreateBY")]
        public int CreateBy { get; set; }

        [Column("modifyBY")]
        public int? ModifyBy { get; set; }

        [Required]
        public bool? IsContinue { get; set; }

        [ForeignKey("CreateBy")]
        [InverseProperty("ContryListCreateByNavigations")]
        public virtual Userauthentication CreateByNavigation { get; set; } = null!;

        [ForeignKey("ModifyBy")]
        [InverseProperty("ContryListModifyByNavigations")]
        public virtual Userauthentication? ModifyByNavigation { get; set; }

        [InverseProperty("Contry")]
        public virtual ICollection<StateList> StateLists { get; set; } = new List<StateList>();

        [InverseProperty("Contry")]
        public virtual ICollection<Userdetail> Userdetails { get; set; } = new List<Userdetail>();
    }
}
