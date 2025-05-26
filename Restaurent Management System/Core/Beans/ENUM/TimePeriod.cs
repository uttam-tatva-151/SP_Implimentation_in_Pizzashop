using System.ComponentModel.DataAnnotations;

namespace PMSCore.Beans.ENUM
{
    public enum TimePeriod
        {
            [Display(Name = "All Time")]
            All,

            [Display(Name = "Last 7 Days")]
            LastSevenDays,

            [Display(Name = "Last 30 Days")]
            LastThirtyDays,

            [Display(Name = "Current Month")]
            CurrentMonth,

        }
}