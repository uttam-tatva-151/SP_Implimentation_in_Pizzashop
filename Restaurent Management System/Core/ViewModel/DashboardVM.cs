namespace PMSCore.ViewModel
{
    public class DashboardVM
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; } 
        public decimal AvgOrderValue { get; set; } 
        public TimeSpan AvgWaitingTime { get; set; } 
        public List<ItemDetails> TopSellingItem { get; set; }  = new List<ItemDetails>();
        public List<ItemDetails> LeastSellingItem { get; set; } = new List<ItemDetails>();
        public int WaitingCustomerCount { get; set; }
        public int NewCustomerCount { get; set; } 
        public List<RevenueDataPoint> RevenueData { get; set; } = new List<RevenueDataPoint>();
        public List<CustomerGrowthDataPoint> CustomerGrowthData { get; set; } = new List<CustomerGrowthDataPoint>();
    }
    public class RevenueDataPoint
    {
        public string Range { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class CustomerGrowthDataPoint
    {
        public string Range { get; set; } = string.Empty;
        public int NewCustomers { get; set; }
    }
}
