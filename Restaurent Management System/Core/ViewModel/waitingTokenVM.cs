using System.ComponentModel.DataAnnotations;

namespace PMSCore.ViewModel
{
    public class waitingTokenVM
    {
        public int TokenId {get;  set;} = 0;
        public int CustomerId {get; set;}
        public DateTime? CreateAt {get;  set;} 
       [Required(ErrorMessage = "Customer Name is required.")]
        [StringLength(100, ErrorMessage = "Customer Name cannot be longer than 100 characters.")]
        public string CustomerName { get; set; } = "Guest";

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [StringLength(15, ErrorMessage = "Phone Number cannot exceed 15 characters.")]
        public string PhoneNumber { get; set; } = null!;

        [Range(1, 100, ErrorMessage = "Number of persons must be between 1 and 100.")]
        public int NoOfPersons { get; set; } = 1;
        public int SectionId {get;  set;} = 0;
        public List<int> TableIds {get; set;} = new();
        public int EditorId {get;  set;} = 0;

    }   
}
