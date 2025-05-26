using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace PMSCore.ViewModel
{
    public class UpdateUser
    {
        public int UserId { get; set; } = 0;//hidden field
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string EmailId { get; set; } = null!;
        [Required(ErrorMessage = "FirstName is required.")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; } = null!;
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Please Select Role.")]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "Phone Number Is required")]
        [RegularExpression(@"^([0-9]{10})$",
                    ErrorMessage = "Phone Number Should in this format - (9874563210)")]

        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Please Select Status.")]
        public string Status { get; set; } = null!; 

        public string? Address { get; set; }
    [RegularExpression(@"^([0-9]{6})$",
                       ErrorMessage = "Zipcode Should in this format - (000000)")]
        public string ZipCode { get; set; } = null!;

    [DataType(DataType.Upload)]
        public IFormFile? Photo { get; set; }
         [Required(ErrorMessage = "Please Select Country")]
        public int ContryId { get; set; }
     [Required(ErrorMessage = "Please Select State")]
        public int StateId { get; set; }
     [Required(ErrorMessage = "Please Select City")]
        public int CityId { get; set; }

        public int EditorId { get; set; } = 0;
    }
}
