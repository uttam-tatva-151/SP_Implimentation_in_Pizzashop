using Microsoft.Extensions.Logging.Abstractions;

namespace PMSCore.ViewModel
{
    public class ModifierGroupVM
    {
        public int groupId { get; set; } = 0;
        public string groupName { get; set;} = null!;
        public string? description { get; set; } 
        public int editorId { get; set; } = 0;
        public List<int> modifierIds { get; set; } = new List<int>();
    }
}
