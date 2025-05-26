using PMSData;

namespace PMSCore.ViewModel
{
    public class KOTVM
    {
        public int OrderId { get; set; } 
        public string tableName { get; set; } = null!;
        public string orderStatus { get; set; } = null!;
        public string sectionName { get; set; } = null!;
        public List<KOTItemsVM> kotItems { get; set; } = new List<KOTItemsVM>();
        public string orderComments { get; set; } = "";
        public DateTime orderAt { get; set; }

        public class KOTItemsVM
        {
            public int categoryId { get; set; }
            public string categoryName { get; set; } = null!;
            public int itemId { get; set; }
            public string itemName { get; set; } = null!;
            public List<KOTModifiersVM> modifiers { get; set; } = new List<KOTModifiersVM>();
            public int quantity { get; set; }
            public int preparedItems { get; set; } 
            public string itemComments { get; set; } = "";
            public bool isPrepared { get; set; } = false;
            public string UniqueGroupId { get; set; } = string.Empty;
            public class KOTModifiersVM
            {
                public int modifierId { get; set; } = 0;
                public string modifierName { get; set; } = "";
            }
        }
    }
}
