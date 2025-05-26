using System.ComponentModel.DataAnnotations;
namespace PMSCore.ViewModel
{
    public class AreaDetails
    {
        public List<SectionDetails> sections { get; set; } = new();
        public List<TableDetails> tables { get; set; } = new();
    }

    public class TableDetails
    {
        public int TableId { get; set; } = 0;
        public string TableName { get; set; } = null!;
        public int SectionId { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = null!;
        public int editorId { get; set; } = 0;
    }


    public class SectionDetails
    {
        public int SectionId { get; set; }=0;

        [Required(ErrorMessage = "Section Name is required.")]
        [StringLength(100, ErrorMessage = "Section Name can't be longer than 100 characters.")]
        public string SectionName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public string? Description { get; set; }

        // [Range(1, int.MaxValue, ErrorMessage = "Editor ID must be a positive number.")]
        public int editorId { get; set; } = 0;
        public int WaitingTokensInQueue { get; set; } = 0;
        public int Assigned {get; set; } = 0;
        public int Running { get; set; } = 0;
        public int Available { get; set; } = 0;

        public List<TableDetails> TableDetails { get; set; } = new();
    }
}
