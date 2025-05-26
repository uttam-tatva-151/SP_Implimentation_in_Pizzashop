using System.ComponentModel.DataAnnotations;

namespace PMSCore.ViewModel
{
    public class UpdatePassword
    {
        public string token { get; set; } = string.Empty;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{4,}$",
                ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [Required(ErrorMessage = "New password is required.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string confirmPassword { get; set; } = null!;
    }
}
