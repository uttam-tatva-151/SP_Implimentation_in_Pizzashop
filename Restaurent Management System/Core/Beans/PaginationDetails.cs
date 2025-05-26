using PMSCore.Beans.ENUM;

namespace PMSCore.Beans
{
    public class PaginationDetails
    {
        public int PageSize { get; set; } = 5;
        public int PageNumber { get; set; } = 1;
        public string SortColumn { get; set; } = string.Empty;
        public string SortOrder { get; set; } = "asc";
        public string SearchQuery { get; set; } = string.Empty;
        public int TotalRecords { get; set; }

        public DateOnly FromDate { get; set; } = DateOnly.MinValue;
        public DateOnly ToDate { get; set; } = DateOnly.MaxValue;

        public OrderStatus OrderStatus { get; set; } = OrderStatus.All;
        public TimePeriod DateRange { get; set; } = TimePeriod.All;
    }

}
