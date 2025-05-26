using Microsoft.AspNetCore.Http;
using PMSData;

namespace PMSCore.ViewModel
{
    public class MenuDetails
    {
        public List<CategoryDetails> categories { get; set; } = new();
        public List<ItemDetails> items { get; set; } = new();
        public List<ModifierGropDetails> modifierGrops { get; set; } = new();
        public List<ModifierDetails> modifiers { get; set; } = new();

    }

    public class ModifierDetails
    {
        public int id { get; set; }
        public int groupId { get; set; }
        public string modifierName { get; set; } = null!;
        public decimal unitPrice { get; set; }
        public int quantity { get; set; }
        public string unitType { get; set; } = null!;
        public string? description { get; set; }

    }


    public class ModifierGropDetails
    {
        public int id { get; set; }
        public string modifierGroupName { get; set; } = null!;
        public string? description { get; set; }
        public int editorId { get; set; } = 0;
        public List<int>? modifiersIds = new();
    }


    public class CategoryDetails
    {
        public int id { get; set; } = 0;
        public string categoryName { get; set; } = null!;
        public string? description { get; set; }
        public int editorId { get; set; } = 0;
    }


    public class ItemDetails
    {
        public int id { get; set; }
        public int categoryId { get; set; } = 0;
        public string itemName { get; set; } = null!;
        public string itemType { get; set; } = null!;
        public decimal unitPrice { get; set; }
        public int quantity { get; set; }
        public string unitType { get; set; } = null!;
        public bool isAvailable { get; set; }
        public bool IsFavorite { get; set; }
        public string? Description { get; set; } 
        // public IFormFile? photo { get; set; }
        public string? photo { get; set; }

        public List<ItemModifierGroupsMappingVM> ModifierGroupsMappingVMs { get; set; } = new();
    }
    public class ItemModifierGroupsMappingVM
    {
        public int Id { get; set; }
        public string ModifierGroupName { get; set; } = null!;
        public int MinModifiers { get; set; } = 0;
        public int MaxModifiers { get; set; } = 1;
        public List<ModifierDetails> Modifiers { get; set; } = new();

    }
}