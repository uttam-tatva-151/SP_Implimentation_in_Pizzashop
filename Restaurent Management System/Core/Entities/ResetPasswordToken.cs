using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("ResetPasswordToken")]
    public partial class ResetPasswordToken
    {
        [Key]
        [Column("TokenID")]
        public int TokenId { get; set; }

        public string ResetToken { get; set; } = null!;

        [Column("userID")]
        public int UserId { get; set; }

        [Required]
        public bool? IsContinue { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreateAt { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("ResetPasswordTokens")]
        public virtual Userauthentication User { get; set; } = null!;
    }
}
