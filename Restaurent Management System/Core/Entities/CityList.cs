using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("city_list")]
    public partial class CityList
    {
        [Key]
        [Column("city_id")]
        public int CityId { get; set; }

        [Column("state_id")]
        public int StateId { get; set; }

        [Column("city_name")]
        [StringLength(50)]
        public string CityName { get; set; } = null!;

        [Column("cretateAT")]
        public TimeOnly CretateAt { get; set; }

        [Column("modifyAT")]
        public TimeOnly? ModifyAt { get; set; }

        [Column("createBY")]
        public int CreateBy { get; set; }

        [Column("modifyBY")]
        public int? ModifyBy { get; set; }

        [Required]
        [Column("isactive")]
        public bool? Isactive { get; set; }

        [ForeignKey("CreateBy")]
        [InverseProperty("CityListCreateByNavigations")]
        public virtual Userauthentication CreateByNavigation { get; set; } = null!;

        [ForeignKey("ModifyBy")]
        [InverseProperty("CityListModifyByNavigations")]
        public virtual Userauthentication? ModifyByNavigation { get; set; }

        [ForeignKey("StateId")]
        [InverseProperty("CityLists")]
        public virtual StateList State { get; set; } = null!;

        [InverseProperty("City")]
        public virtual ICollection<Userdetail> Userdetails { get; set; } = new List<Userdetail>();
    }
}
