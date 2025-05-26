using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PMSCore.ViewModel
{
    public class NewUser
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string EmailId { get; set; } = null!;

        [Required(ErrorMessage = "FirstName is required.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; } = null!;
        public string? LastName { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{4,}$",
               ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Please Select Role.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Phone Number Is required")]
        [RegularExpression(@"^([0-9]{10})$",
                    ErrorMessage = "Phone Number Should in this format - (9874563210)")]
        public string PhoneNumber { get; set; } = null!;
        public string? Address { get; set; }
        //       @"^([\+]?33[-]?|[0])?[1-9][0-9]{8}$
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
