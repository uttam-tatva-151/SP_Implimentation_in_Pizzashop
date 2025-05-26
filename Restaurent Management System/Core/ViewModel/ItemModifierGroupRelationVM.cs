namespace PMSCore.ViewModel
{
    public class ItemModifierGroupRelationVM
    {

        public int ItemId { get; set; } = 0;

        public string ItemName { get; set; } = null!;
        public decimal ItemPrice { get; set; } = 0.0m;
        public string? SpecialInstructions { get; set; } = string.Empty;

        public List<ModifierGroupHelper> Groups { get; set; } = new();

        public class ModifierGroupHelper
        {
            public int ItemModifierGroupMappingId { get; set; } = 0;
            public int ModifierGroupId { get; set; } = 0;
            public string ModifierGroupName { get; set; } = null!;
            public List<ModifiersHelper> Modifiers { get; set; } = new();
            public int MinRequired { get; set; } = 0;
            public int MaxRequired { get; set; } = 0;
            public class ModifiersHelper
            {
                public int ModifierId { get; set; } = 0;
                public string ModifierName { get; set; } = null!;
                public decimal UnitPrice { get; set; } = 0.0m;
                public int Quantity { get; set; } = 1;
                public string UnitType { get; set; } = null!;
            }

        }


    }
}
