using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("orders")]
    [Index("Status", Name = "idx_orders_status")]
    public partial class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = null!;

        [Column("deliver_on_time")]
        public bool DeliverOnTime { get; set; }

        [Column("nu_of_persons")]
        public int NuOfPersons { get; set; }

        [Column("createat", TypeName = "timestamp without time zone")]
        public DateTime Createat { get; set; }

        [Column("modifyat", TypeName = "timestamp without time zone")]
        public DateTime? Modifyat { get; set; }

        [Column("createby")]
        public int Createby { get; set; }

        [Column("modifyby")]
        public int? Modifyby { get; set; }

        [Column("iscontinued")]
        public bool Iscontinued { get; set; }

        [Column("extra_comments")]
        public string? ExtraComments { get; set; }

        [ForeignKey("Createby")]
        [InverseProperty("OrderCreatebyNavigations")]
        public virtual Userauthentication CreatebyNavigation { get; set; } = null!;

        [ForeignKey("CustomerId")]
        [InverseProperty("Orders")]
        public virtual Customer Customer { get; set; } = null!;

        [InverseProperty("Order")]
        public virtual ICollection<InvoiceItemModifierMapping> InvoiceItemModifierMappings { get; set; } = new List<InvoiceItemModifierMapping>();

        [InverseProperty("Order")]
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        [ForeignKey("Modifyby")]
        [InverseProperty("OrderModifybyNavigations")]
        public virtual Userauthentication? ModifybyNavigation { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        [InverseProperty("Order")]
        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
    }
}
