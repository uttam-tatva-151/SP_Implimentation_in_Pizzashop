namespace PMSCore.ViewModel
{
    public class OrderDetailsVM
    {
        public int OrderId { get; set; }
        public DateOnly OrderDate { get; set; }
        public string CustomerName { get; set; }  = string.Empty;
        public string OrderStatus { get; set; }  = string.Empty;

        public string PaymentType { get; set; }  = string.Empty;
        public short? Rating { get; set; } = 0;
        public decimal TotalAmount { get; set; }
    }

    public class OrderExportDetails
    {
        public int EditorId { get; set; } = 0;
        public int OrderId { get; set; }
        public string Status { get; set; }  = string.Empty;
         public string? OrderInstruction { get; set; }  = string.Empty;
        public string InvoiceNo { get; set; }  = string.Empty;
        public DateTime PaidOn { get; set; }
        public DateTime PlacedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public TimeOnly OrderDuration { get; set; }
        public string Tables { get; set; }  = string.Empty;
        public string Section { get; set; }  = string.Empty;
        public string PaymentMethod { get; set; }  = string.Empty;
        public decimal SubTotal { get; set; }
        public CustomerDetails CustomerInfo { get; set; } = new();
        public List<OrderItemHelperModel> OrderItems { get; set; } = new List<OrderItemHelperModel>();
        public List<TaxDetailsHelperModel> taxDetails { get; set; } = new List<TaxDetailsHelperModel>();
        public decimal TotalAmountToPay { get; set; }
        public class OrderItemHelperModel
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TaxPerItem { get; set; } =0M;
            public int PreparedItems {get; set;} = 0;
            public decimal TotalPrice { get; set; }
            public string UniqueGroupId {get; set;} = string.Empty;
            public string? SpecialInstructions {get; set;} = string.Empty;
            public List<OrderModiferHelperModel>? Modifiers { get; set; }
            public class OrderModiferHelperModel
            {
                 public int ModifierId { get; set; } = 0;
                public string ModifierName { get; set; }  = string.Empty;
                public int ModiferQuantity { get; set; }
                public decimal ModifierPrice { get; set; }
                public decimal ModifierTotalPrice { get; set; }
            }
        }
        public class TaxDetailsHelperModel
        {
            public string TaxName { get; set; }  = string.Empty;
            public string? TaxType { get; set; }
            public decimal TaxValue { get; set; }
        }
    }

}
