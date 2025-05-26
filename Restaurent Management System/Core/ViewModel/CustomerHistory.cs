namespace PMSCore.ViewModel
{
    public class CustomerHistory
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        private decimal _maxBill;
       public decimal MaxBill
        {
            get => _maxBill;
            set => _maxBill = Math.Round(value, 2); // Round to 2 decimal places
        }

        private decimal _avgBill;
        public decimal AvgBill
        {
            get => _avgBill;
            set => _avgBill = Math.Round(value, 3); // Round to 3 decimal places
        }        public DateTime FirstVisit { get; set; }
        public int Visits { get; set; }
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();

        public class OrderViewModel
        {
            public int OrderId { get; set; }
            public DateOnly OrderDate { get; set; }
            public string OrderType { get; set; } = "Dine In";
            public string OrderStatus { get; set; } = string.Empty;
            public string PaymentStatus { get; set; } = string.Empty;
            public int NumberOfItems { get; set; }
            public decimal Amount { get; set; }
        }
    }


}