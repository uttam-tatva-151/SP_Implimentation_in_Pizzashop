using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PMSData
{
    [Table("error_logs")]
    public partial class ErrorLog
    {
        [Key]
        [Column("error_id")]
        public int ErrorId { get; set; }

        [Column("error_code")]
        public int ErrorCode { get; set; }

        [Column("error_short_message", TypeName = "character varying")]
        public string ErrorShortMessage { get; set; } = null!;

        [Column("createAt")]
        public TimeOnly CreateAt { get; set; }

        [Column("cretaeBy")]
        public int CretaeBy { get; set; }

        [Required]
        [Column("isActive")]
        public bool? IsActive { get; set; }

        [Column("actual_error")]
        public string ActualError { get; set; } = null!;

        [ForeignKey("CretaeBy")]
        [InverseProperty("ErrorLogs")]
        public virtual Userauthentication CretaeByNavigation { get; set; } = null!;
    }
}
