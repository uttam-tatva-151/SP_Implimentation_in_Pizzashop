using System.ComponentModel.DataAnnotations;

namespace PMSCore.ViewModel
{
    public class CustomerDetails
    {
        public int CustomerId { get; set; } = 0;

        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer phone is required.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of persons is required.")]
        [Range(1, 100, ErrorMessage = "Number of persons must be between 1 and 50.")]
        public int NoOfPerson { get; set; } = 1;

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Invalid email address format.")]
        public string CustomerEmail { get; set; } =  string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Total orders must be 0 or greater.")]
        public int TotalOrders { get; set; } = 0;

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateOnly LastOrder { get; set; } = new DateOnly();
        public int EditorId {get; set;} = 0;
    }
}
