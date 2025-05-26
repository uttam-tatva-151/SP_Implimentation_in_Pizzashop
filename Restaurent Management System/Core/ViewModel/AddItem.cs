using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PMSCore.ViewModel
{
    public class AddItem
    {
        [Key]
        public int itemID { get; set; } = 0;

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Item name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description can be up to 500 characters.")]
        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Item type is required.")]

        public string ItemType { get; set; } = null!;
        [Required(ErrorMessage = "Unit type is required.")]
        [StringLength(50, ErrorMessage = "Unit type can be up to 50 characters.")]
        public string UnitType { get; set; } = null!;
        [Required(ErrorMessage = "Unit price is required.")]
        [Range(0.01, 100000, ErrorMessage = "Unit price must be between 0.01 and 100000.")]
        public decimal UnitPrice { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 100000, ErrorMessage = "Quantity must be non-negative and ateast 1.")]
        public int quantity { get; set; }
        [Required(ErrorMessage = "Tax percentage is required.")]
        [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100.")]
        public decimal TaxPercentage { get; set; }
        public bool IsAvailable { get; set; }
        public bool DefaultTax { get; set; }
        public string? ShortCode { get; set; } 
        public int EditorId { get; set; } = 0;
        [DataType(DataType.Upload)]

        public IFormFile? Photo { get; set; }
        public List<ItemModifierGroupRelation>? IMDetails { get; set; } = new();
    }

    public class ItemModifierGroupRelation
    {
        public int IMGid { get; set; } = 0;
        public int ItemId { get; set; }
        [Required(ErrorMessage = "Minimum modifiers is required.")]
        [Range(0, 100, ErrorMessage = "Minimum modifiers must be between 0 and 100.")]
        public int MinModifiers { get; set; }

        [Required(ErrorMessage = "Maximum modifiers is required.")]
        [Range(1, 100, ErrorMessage = "Maximum modifiers must be between 1 and 100.")]
        public int MaxModifiers { get; set; }
        public int MgId { get; set; }
        
    }
}