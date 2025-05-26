namespace PMSCore.ViewModel
{
    public class TableViewOrderAppVM
    {
        public int TableId { get; set; } = 0;
        public int OrderId { get; set; } = 0;
        public string TableName { get; set; } = null!;
        public int SectionId { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = null!;
        public int editorId { get; set; } = 0;
        public DateTime TableAssign { get; set; } 
        public Decimal OrderAmount { get; set; } =0;
    }
}
