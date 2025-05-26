using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("order_details")]
    public partial class OrderDetail
    {
        [Key]
        [Column("od_id")]
        public int OdId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("table_id")]
        public int[] TableId { get; set; } = null!;

        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("employee_id")]
        public int[]? EmployeeId { get; set; }

        [Column("feedback_id")]
        public int? FeedbackId { get; set; }

        [Column("createdat", TypeName = "timestamp without time zone")]
        public DateTime Createdat { get; set; }

        [Column("modifiedat", TypeName = "timestamp without time zone")]
        public DateTime? Modifiedat { get; set; }

        [Column("createdby")]
        public int Createdby { get; set; }

        [Column("modifiedby")]
        public int? Modifiedby { get; set; }

        [Required]
        [Column("iscontinued")]
        public bool? Iscontinued { get; set; }

        [ForeignKey("Createdby")]
        [InverseProperty("OrderDetailCreatedbyNavigations")]
        public virtual Userauthentication CreatedbyNavigation { get; set; } = null!;

        [ForeignKey("FeedbackId")]
        [InverseProperty("OrderDetails")]
        public virtual FeedbackForm? Feedback { get; set; }

        [ForeignKey("Modifiedby")]
        [InverseProperty("OrderDetailModifiedbyNavigations")]
        public virtual Userauthentication? ModifiedbyNavigation { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderDetails")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("PaymentId")]
        [InverseProperty("OrderDetails")]
        public virtual PaymentDetail Payment { get; set; } = null!;
    }
}
