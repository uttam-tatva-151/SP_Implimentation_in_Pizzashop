using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMSData
{
    [Table("feedback_form")]
    public partial class FeedbackForm
    {
        [Key]
        [Column("feedback_id")]
        public int FeedbackId { get; set; }

        [Column("food_rating")]
        public short? FoodRating { get; set; }

        [Column("service_rating")]
        public short? ServiceRating { get; set; }

        [Column("ambiance_rating")]
        public short? AmbianceRating { get; set; }

        [Column("feedback_description")]
        public string? FeedbackDescription { get; set; }

        [Column("createdat", TypeName = "timestamp without time zone")]
        public DateTime Createdat { get; set; }

        [Column("is_continued")]
        public bool IsContinued { get; set; }

        [InverseProperty("Feedback")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
