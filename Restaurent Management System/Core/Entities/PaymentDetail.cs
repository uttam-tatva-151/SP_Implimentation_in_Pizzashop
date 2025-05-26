using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("payment_details")]
    [Index("OrderId", Name = "idx_payments_order_id")]
    public partial class PaymentDetail
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("payment_method")]
        [StringLength(20)]
        public string PaymentMethod { get; set; } = null!;

        [Column("actual_price")]
        [Precision(8, 2)]
        public decimal ActualPrice { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("createat", TypeName = "timestamp without time zone")]
        public DateTime Createat { get; set; }

        [Column("payment_status")]
        [StringLength(15)]
        public string PaymentStatus { get; set; } = null!;

        [Column("total_price")]
        [Precision(8, 2)]
        public decimal? TotalPrice { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("PaymentDetails")]
        public virtual Order Order { get; set; } = null!;

        [InverseProperty("Payment")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
