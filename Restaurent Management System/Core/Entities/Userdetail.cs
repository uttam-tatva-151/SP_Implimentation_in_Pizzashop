using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("userdetails")]
    public partial class Userdetail
    {
        [Key]
        [Column("user_details_id")]
        public int UserDetailsId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("first_name")]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Column("last_name")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Column("phone_number")]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Column("address")]
        [StringLength(50)]
        public string? Address { get; set; }

        [Column("zip_code")]
        [StringLength(10)]
        public string ZipCode { get; set; } = null!;

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = null!;

        [Column("createat", TypeName = "timestamp without time zone")]
        public DateTime Createat { get; set; }

        [Column("modifyat", TypeName = "timestamp without time zone")]
        public DateTime? Modifyat { get; set; }

        [Required]
        [Column("isactive")]
        public bool? Isactive { get; set; }

        [Column("contry_id")]
        public int ContryId { get; set; }

        [Column("state_id")]
        public int StateId { get; set; }

        [Column("city_id")]
        public int CityId { get; set; }

        [Column("createBY")]
        public int CreateBy { get; set; }

        [Column("modifyBY")]
        public int? ModifyBy { get; set; }

        [Column("photo")]
        public byte[]? Photo { get; set; }

        [ForeignKey("CityId")]
        [InverseProperty("Userdetails")]
        public virtual CityList City { get; set; } = null!;

        [ForeignKey("ContryId")]
        [InverseProperty("Userdetails")]
        public virtual ContryList Contry { get; set; } = null!;

        [ForeignKey("CreateBy")]
        [InverseProperty("UserdetailCreateByNavigations")]
        public virtual Userauthentication CreateByNavigation { get; set; } = null!;

        [ForeignKey("ModifyBy")]
        [InverseProperty("UserdetailModifyByNavigations")]
        public virtual Userauthentication? ModifyByNavigation { get; set; }

        [ForeignKey("StateId")]
        [InverseProperty("Userdetails")]
        public virtual StateList State { get; set; } = null!;

        [ForeignKey("UserId")]
        [InverseProperty("Userdetail")]
        public virtual Userauthentication User { get; set; } = null!;
    }
}
