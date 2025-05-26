using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSCore.ViewModel
{
    public class ModifierVM
    {
        [Key]
        public int ModifierId { get; set; } = 0; // Default value

        [Required(ErrorMessage = "At least one Modifier Group ID is required.")]
        [MinLength(1, ErrorMessage = "You must specify at least one Modifier Group ID.")]
        public int[] ModifierGroupId { get; set; } = null!;

        [Required(ErrorMessage = "Modifier Name is required.")]
        [StringLength(100, ErrorMessage = "Modifier Name cannot exceed 100 characters.")]
        public string ModifierName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Unit Price is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Unit Price must be between 0.01 and 9,999,999.99.")]
        [Precision(7, 2)]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Type is required.")]
        [StringLength(50, ErrorMessage = "Unit Type cannot exceed 50 characters.")]
        public string UnitType { get; set; } = null!;

        [Required(ErrorMessage = "Editor ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Editor ID must be greater than 0.")]
        public int EditorId { get; set; } = 0;
    }
}