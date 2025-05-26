using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("permissions")]
    public partial class Permission
    {
        [Key]
        [Column("p_id")]
        public int PId { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("module_id")]
        public int ModuleId { get; set; }

        [Column("can_createandedit")]
        public bool CanCreateandedit { get; set; }

        [Column("can_view")]
        public bool CanView { get; set; }

        [Column("can_delete")]
        public bool CanDelete { get; set; }

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
        [InverseProperty("PermissionCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [ForeignKey("Modifyby")]
        [InverseProperty("PermissionModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }

        [ForeignKey("ModuleId")]
        [InverseProperty("Permissions")]
        public virtual Module Module { get; set; } = null!;

        [ForeignKey("RoleId")]
        [InverseProperty("Permissions")]
        public virtual Role Role { get; set; } = null!;
    }
}
