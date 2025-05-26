using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("roles")]
    [Index("RoleName", Name = "roles_role_name_key", IsUnique = true)]
    [Index("RoleName", Name = "uq_role_name", IsUnique = true)]
    public partial class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("role_name")]
        [StringLength(30)]
        public string RoleName { get; set; } = null!;

        [Required]
        [Column("isactive")]
        public bool? Isactive { get; set; }

        [Column("createdat", TypeName = "timestamp without time zone")]
        public DateTime Createdat { get; set; }

        [Column("updatedat", TypeName = "timestamp without time zone")]
        public DateTime? Updatedat { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

        [InverseProperty("Role")]
        public virtual ICollection<Userauthentication> Userauthentications { get; set; } = new List<Userauthentication>();
    }
}
