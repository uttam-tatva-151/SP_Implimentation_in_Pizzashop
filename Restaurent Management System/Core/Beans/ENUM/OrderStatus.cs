using System.ComponentModel.DataAnnotations;

namespace PMSCore.Beans.ENUM
{
    public enum OrderStatus
    {
        [Display(Name = "All Status")]
        All,

        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "In Progress")]
        InProgress,

        [Display(Name = "Served")]
        Served,

        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "Cancelled")]
        Cancelled,

        [Display(Name = "On Hold")]
        OnHold,

        [Display(Name = "Failed")]
        Failed
    }
}
