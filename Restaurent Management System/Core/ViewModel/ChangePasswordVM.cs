using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PMSCore.ViewModel
{
    public class ChangePasswordVM
    {
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; } = null!;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{4,}$", 
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords are not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
