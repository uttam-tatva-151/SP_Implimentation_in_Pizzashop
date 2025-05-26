using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("RefreshToken")]
    public partial class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "character varying")]
        public string Token { get; set; } = null!;

        public int UserId { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime ExpiryDate { get; set; }

        public bool? IsRevoked { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("RefreshTokens")]
        public virtual Userauthentication User { get; set; } = null!;
    }
}
