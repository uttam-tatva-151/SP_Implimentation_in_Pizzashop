// using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace PMSCore.ViewModel
{
    public class UserProfileVM
    {
        public string EmailId { get; set; } = null!;

        [Required(ErrorMessage = "FirstName is required.")]
        public string FirstName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? LastName { get; set; }

        public string RoleName { get; set; } = null!;
        [RegularExpression(@"^([0-9]{10})$",
                    ErrorMessage = "Phone Number Should in this format - (9874563210)")]
        public string PhoneNumber { get; set; } = null!;

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
        public int editorId { get; set; } = 0;

        // Dropdown lists
        // public List<SelectListItem>? Countries { get; set; }
        // public List<SelectListItem>? States { get; set; }
        // public List<SelectListItem>? Cities { get; set; }
    }
}
