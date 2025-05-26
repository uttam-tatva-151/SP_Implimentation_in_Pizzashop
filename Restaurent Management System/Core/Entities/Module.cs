using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("module")]
    [Index("ModuleName", Name = "module_module_name_key", IsUnique = true)]
    public partial class Module
    {
        [Key]
        [Column("module_id")]
        public int ModuleId { get; set; }

        [Column("module_name")]
        [StringLength(30)]
        public string ModuleName { get; set; } = null!;

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
        [InverseProperty("ModuleCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [ForeignKey("Modifyby")]
        [InverseProperty("ModuleModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }

        [InverseProperty("Module")]
        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
