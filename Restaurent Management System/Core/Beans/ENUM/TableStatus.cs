using System.ComponentModel.DataAnnotations;

namespace PMSCore.Beans.ENUM
{
    public enum TableStatus
    {
        [Display(Name = "Occupied")]
        Occupied,
        [Display(Name = "Available")]
        Available
    }
}
