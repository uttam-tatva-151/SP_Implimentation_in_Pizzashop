namespace PMSCore.ViewModel
{
    public class TaxDetails
    {
        public int TaxId { get; set; }
        public string TaxName { get; set; } = null!;
        public string TaxType { get; set; } = null!;
         public bool Isenabled { get; set; }
        public decimal TaxValue { get; set; }
         public bool Isdefault { get; set; }
        public int EditorId { get; set; } =0;
    }
}
