using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("customers")]
    [Index("EmailId", Name = "customers_email_id_key", IsUnique = true)]
    [Index("PhoneNumber", Name = "customers_phone_number_key", IsUnique = true)]
    [Index("EmailId", Name = "idx_customers_email")]
    public partial class Customer
    {
        [Key]
        [Column("cust_id")]
        public int CustId { get; set; }

        [Column("cust_name")]
        [StringLength(50)]
        public string CustName { get; set; } = null!;

        [Column("email_id")]
        [StringLength(100)]
        public string EmailId { get; set; } = null!;

        [Column("phone_number")]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Column("total_orders")]
        public int TotalOrders { get; set; }

        [Column("createat", TypeName = "timestamp without time zone")]
        public DateTime Createat { get; set; }

        [Column("modifyat", TypeName = "timestamp without time zone")]
        public DateTime? Modifyat { get; set; }

        [Column("iscontinued")]
        public bool Iscontinued { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [InverseProperty("Customer")]
        public virtual ICollection<WaitingList> WaitingLists { get; set; } = new List<WaitingList>();
    }
}
