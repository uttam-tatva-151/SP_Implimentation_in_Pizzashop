using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("state_list")]
    public partial class StateList
    {
        [Key]
        [Column("state_id")]
        public int StateId { get; set; }

        [Column("contry_id")]
        public int ContryId { get; set; }

        [Column("state_name")]
        [StringLength(50)]
        public string StateName { get; set; } = null!;

        [Column("createAT")]
        public TimeOnly CreateAt { get; set; }

        [Column("modifyAT")]
        public TimeOnly? ModifyAt { get; set; }

        [Column("createBY")]
        public int CreateBy { get; set; }

        [Column("modifyBY")]
        public int? ModifyBy { get; set; }

        [Required]
        public bool? IsContinue { get; set; }

        [InverseProperty("State")]
        public virtual ICollection<CityList> CityLists { get; set; } = new List<CityList>();

        [ForeignKey("ContryId")]
        [InverseProperty("StateLists")]
        public virtual ContryList Contry { get; set; } = null!;

        [ForeignKey("CreateBy")]
        [InverseProperty("StateListCreateByNavigations")]
        public virtual Userauthentication CreateByNavigation { get; set; } = null!;

        [ForeignKey("ModifyBy")]
        [InverseProperty("StateListModifyByNavigations")]
        public virtual Userauthentication? ModifyByNavigation { get; set; }

        [InverseProperty("State")]
        public virtual ICollection<Userdetail> Userdetails { get; set; } = new List<Userdetail>();
    }
}
