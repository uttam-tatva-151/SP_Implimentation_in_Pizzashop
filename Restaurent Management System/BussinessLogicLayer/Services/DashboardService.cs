using PMSCore.Beans;
using PMSCore.Beans.ENUM;
using PMSCore.DTOs;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IWaitingRepo _waitingRepo;
        public DashboardService(IOrderRepo orderRepo, IWaitingRepo waitingRepo)
        {
            _orderRepo = orderRepo;
            _waitingRepo = waitingRepo;
        }

        readonly ResponseResult result = new();
        public async Task<ResponseResult> GetAnalytics(PaginationDetails paginationDetails)
        {
            try
            {
                DashboardVM dashboardView = new();
                paginationDetails.SortColumn = Constants.SORT_BY_DATE;
                List<Order> orderList = await _orderRepo.GetOrdersAsync(paginationDetails);
                List<WaitingTokenDTO> waitingList = await _waitingRepo.GetWaitingTokensBySectionAsync(0);

                dashboardView.TotalSales = CalculateTotalSales(orderList);
                dashboardView.TotalOrders = orderList.Count;
                dashboardView.AvgOrderValue = CalculateAvgOrderValue(orderList);
                dashboardView.AvgWaitingTime = CalculateAvgWaitingTime(waitingList);
                dashboardView.TopSellingItem = CalculateTopSellingItem(orderList);
                dashboardView.LeastSellingItem = CalculateLeastSellingItem(orderList);
                dashboardView.WaitingCustomerCount = waitingList.Count(w => w.IsActive);
                dashboardView.NewCustomerCount = CalculateNewCustomerCount(orderList);

                dashboardView.RevenueData = CalculateRevenueData(orderList, paginationDetails);
                dashboardView.CustomerGrowthData = CalculateCustomerGrowthData(orderList, paginationDetails);

                result.Data = dashboardView;
                result.Message = Constants.ANALYSIS_COMPLETED;
                result.Status = ResponseStatus.Success;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }


        private static List<RevenueDataPoint> CalculateRevenueData(List<Order> orderList, PaginationDetails paginationDetails)
        {
            List<string> xAxisLabels = new();
            Dictionary<string, decimal> revenueByRange = new();
            if (paginationDetails.FromDate != DateOnly.MinValue || paginationDetails.ToDate != DateOnly.MaxValue)
            {
                if (paginationDetails.ToDate == DateOnly.MaxValue) paginationDetails.ToDate = DateOnly.FromDateTime(DateTime.Now);
                if (paginationDetails.FromDate == DateOnly.MinValue) paginationDetails.FromDate = DateOnly.FromDateTime(orderList[0].Createat);
                xAxisLabels = GetXAxisLabelsBasedOnCustomRange(paginationDetails.FromDate.ToDateTime(TimeOnly.MinValue),
                                                                paginationDetails.ToDate.ToDateTime(TimeOnly.MaxValue)
                                                              );
                revenueByRange = GetGroupedDataForCustomRange(orderList,
                                                                paginationDetails.FromDate.ToDateTime(TimeOnly.MinValue),
                                                                paginationDetails.ToDate.ToDateTime(TimeOnly.MaxValue)
                                                              );
            }
            else
            {
                DateTime today = DateTime.Now;

                xAxisLabels = GetXAxisLabels(paginationDetails.DateRange, today);
                revenueByRange = GetGroupedDataForChart(orderList, paginationDetails.DateRange);
            }

            List<RevenueDataPoint> revenueData = xAxisLabels
                .Select(range => new RevenueDataPoint
                {
                    Range = range,
                    Revenue = revenueByRange.TryGetValue(range, out decimal value) ? value : 0M
                })
                .ToList();

            return revenueData;
        }
        private static List<string> GetXAxisLabelsBasedOnCustomRange(DateTime fromDate, DateTime toDate)
        {
            TimeSpan diff = toDate - fromDate;
            if (diff.TotalHours < 24)
            {
                return Enumerable.Range(0, (int)diff.TotalHours + 1)
                                .Select(i => fromDate.AddHours(i)
                                                    .ToString("HH:00"))
                                                    .ToList();
            }
            else if (diff.TotalDays < 7)
                return Enumerable.Range(0, (int)diff.TotalDays + 1)
                                .Select(i => fromDate.AddDays(i)
                                                    .ToString("MM-dd"))
                                                    .ToList();

            else if (diff.TotalDays < 32)
                return Enumerable.Range(0, (int)diff.TotalDays + 1)
                                .Select(i => fromDate.AddDays(i)
                                                    .ToString("MM-dd"))
                                                    .ToList();

            List<string> months = new();
            for (DateTime d = new(fromDate.Year, fromDate.Month, 1); d <= toDate; d = d.AddMonths(1))
                months.Add(d.ToString("yyyy-MM"));
            return months;
        }
        private static List<string> GetXAxisLabels(TimePeriod period, DateTime today)
        {
            return period switch
            {
                TimePeriod.LastSevenDays =>
                    Enumerable.Range(0, 7)
                        .Select(offset => today.AddDays(-6 + offset).ToString("dd"))
                        .ToList(),

                TimePeriod.LastThirtyDays =>
                    Enumerable.Range(0, 30)
                        .Select(offset => today.AddDays(-29 + offset).ToString("MM-dd"))
                        .ToList(),

                TimePeriod.CurrentMonth =>
                    Enumerable.Range(1, DateTime.DaysInMonth(today.Year, today.Month))
                        .Select(day => day.ToString("00"))
                        .ToList(),

                TimePeriod.All =>
                    Enumerable.Range(1, 12)
                        .Select(month => new DateTime(today.Year, month, 1).ToString("MMM"))
                        .ToList(),

                _ => new List<string>()
            };
        }
        private static Dictionary<string, decimal> GetGroupedDataForChart(List<Order> orderList, TimePeriod timePeriod)
        {
            Dictionary<TimePeriod, string> formats = new(){
                                                            { TimePeriod.LastSevenDays, "dd" },
                                                            { TimePeriod.LastThirtyDays, "MM-dd" },
                                                            { TimePeriod.CurrentMonth, "dd" },
                                                            { TimePeriod.All, "MMM" }
                                                        };

            string format = formats.ContainsKey(timePeriod) ? formats[timePeriod] : "MMM";

            return orderList
                .GroupBy(order => order.Createat.ToString(format))
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(order => order.PaymentDetails.First().TotalPrice ?? 0M)
                );
        }
        private static Dictionary<string, decimal> GetGroupedDataForCustomRange(List<Order> orderList, DateTime fromDate, DateTime toDate)
        {
            TimeSpan diff = toDate - fromDate;
            Func<Order, string> keySelector;

            if (diff.TotalHours < 24)
                keySelector = order => order.Createat.ToString("HH:00");
            else if (diff.TotalDays < 7)
                keySelector = order => order.Createat.ToString("MM-dd");
            else if (diff.TotalDays < 32)
                keySelector = order => order.Createat.ToString("MM-dd");
            else
                keySelector = order => order.Createat.ToString("yyyy-MM");

            return orderList
                .Where(order => order.Createat >= fromDate && order.Createat <= toDate)
                .GroupBy(keySelector)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(order => order.PaymentDetails.First().TotalPrice ?? 0M)
                );
        }

        private static Dictionary<string, int> GetCustomerGrowthByTimePeriod(List<Order> orderList, TimePeriod timePeriod)
        {
            Dictionary<TimePeriod, string> formats = new(){
                                                                { TimePeriod.LastSevenDays, "dd" },
                                                                { TimePeriod.LastThirtyDays, "MM-dd" },
                                                                { TimePeriod.CurrentMonth, "dd" },
                                                                { TimePeriod.All, "MMM" }
                                                            };

            string format = formats.ContainsKey(timePeriod) ? formats[timePeriod] : "MMM";

            return orderList
                .GroupBy(order => order.Createat.ToString(format))
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(order => order.CustomerId).Distinct().Count()
                );
        }

        private static Dictionary<string, int> GetCustomerGrowthByCustomRange(List<Order> orderList, DateTime fromDate, DateTime toDate)
        {
            TimeSpan diff = toDate - fromDate;
            Func<Order, string> keySelector;

            if (diff.TotalHours < 24)
                keySelector = order => order.Createat.ToString("HH:00");
            else if (diff.TotalDays < 7)
                keySelector = order => order.Createat.ToString("MM-dd");
            else if (diff.TotalDays < 32)
                keySelector = order => order.Createat.ToString("MM-dd");
            else
                keySelector = order => order.Createat.ToString("yyyy-MM");

            return orderList
                .Where(order => order.Createat >= fromDate && order.Createat <= toDate)
                .GroupBy(keySelector)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(order => order.CustomerId).Distinct().Count()
                );
        }

        private static List<CustomerGrowthDataPoint> CalculateCustomerGrowthData(List<Order> orderList, PaginationDetails paginationDetails)
        {
            List<string> xAxisLabels = new();
            Dictionary<string, int> customerGrowthByMonth = new();
            if (paginationDetails.FromDate != DateOnly.MinValue || paginationDetails.ToDate != DateOnly.MaxValue)
            {
                xAxisLabels = GetXAxisLabelsBasedOnCustomRange(paginationDetails.FromDate.ToDateTime(TimeOnly.MinValue),
                                                                paginationDetails.ToDate.ToDateTime(TimeOnly.MaxValue)
                                                              );
                customerGrowthByMonth = GetCustomerGrowthByCustomRange(orderList,
                                                                paginationDetails.FromDate.ToDateTime(TimeOnly.MinValue),
                                                                paginationDetails.ToDate.ToDateTime(TimeOnly.MaxValue)
                                                              );
            }
            else
            {
                DateTime today = DateTime.Now;

                xAxisLabels = GetXAxisLabels(paginationDetails.DateRange, today);
                customerGrowthByMonth = GetCustomerGrowthByTimePeriod(orderList, paginationDetails.DateRange);
            }

            List<CustomerGrowthDataPoint> customerGrowthData = xAxisLabels
                .Select(range => new CustomerGrowthDataPoint
                {
                    Range = range,
                    NewCustomers = customerGrowthByMonth.TryGetValue(range, out int value) ? value : 0
                })
                .ToList();

            return customerGrowthData;
        }
        private static int CalculateNewCustomerCount(List<Order> orderList)
        {
            HashSet<int> existingCustomerIds = new();
            int newCustomerCount = 0;

            foreach (Order order in orderList)
            {
                if (order.Customer != null)
                {
                    int customerId = order.Customer.CustId;

                    if (!existingCustomerIds.Contains(customerId))
                    {
                        existingCustomerIds.Add(customerId);
                        newCustomerCount++;
                    }
                }
            }

            return newCustomerCount;
        }

        private static List<ItemDetails> CalculateLeastSellingItem(List<Order> orderList)
        {
            Dictionary<int, int> itemSales = GetAllItemMappings(orderList)
                                    .Where(m => m != null)
                                    .GroupBy(m => m.ItemId)
                                    .ToDictionary(g => g.Key, g => g.Count());

            return itemSales.OrderBy(kvp => kvp.Value)
                            .Take(2)
                            .Select(kvp => new ItemDetails
                            {
                                id = kvp.Key,
                                itemName = GetAllItemMappings(orderList).First(m => m.ItemId == kvp.Key).Item.ItemName,
                                quantity = kvp.Value
                            })
                            .ToList();
        }
        private static List<ItemDetails> CalculateTopSellingItem(List<Order> orderList)
        {
            Dictionary<int, int> itemSales = GetAllItemMappings(orderList)
                                                        .Where(m => m != null)
                                                        .GroupBy(m => m.ItemId)
                                                        .ToDictionary(g => g.Key, g => g.Count());

            return itemSales.OrderByDescending(kvp => kvp.Value).Take(2)
                .Select(kvp => new ItemDetails
                {
                    id = kvp.Key,
                    itemName = GetAllItemMappings(orderList).First(m => m.ItemId == kvp.Key).Item.ItemName,
                    quantity = kvp.Value
                })
                .ToList();
        }
        private static List<InvoiceItemModifierMapping> GetAllItemMappings(List<Order> orderList)
        {
            List<InvoiceItemModifierMapping> allItemMappings = new();

            foreach (Order order in orderList)
            {
                if (order.InvoiceItemModifierMappings != null)
                {
                    allItemMappings.AddRange(order.InvoiceItemModifierMappings);
                }
            }

            return allItemMappings;
        }
        private static TimeSpan CalculateAvgWaitingTime(List<WaitingTokenDTO> orderList)
        {
            if (orderList == null || orderList.Count == 0)
                return TimeSpan.Zero;

            TimeSpan totalWaitingTime = TimeSpan.Zero;
            int count = 0;

            foreach (WaitingTokenDTO waitingToken in orderList)
            {

                TimeSpan waitingTime = (waitingToken.Modifyat ?? waitingToken.Createat) - waitingToken.Createat;

                totalWaitingTime += waitingTime;
                count++;
            }

            return count > 0 ? TimeSpan.FromTicks(totalWaitingTime.Ticks / count) : TimeSpan.Zero;
        }
        private static decimal CalculateAvgOrderValue(List<Order> orderList)
        {
            if (orderList == null || orderList.Count == 0)
                return 0;

            decimal totalOrderValue = CalculateTotalSales(orderList);
            int count = orderList.Count;
            return count > 0 ? Math.Round(totalOrderValue / count, 2) : 0;
        }
        private static decimal CalculateTotalSales(List<Order> orderList)
        {
            if (orderList == null || orderList.Count == 0)
                return 0;

            // Initialize a variable to track the total sales
            decimal totalSales = 0;

            foreach (Order order in orderList)
            {
                PaymentDetail? orderPaymentDetail = order.PaymentDetails.FirstOrDefault();

                if (orderPaymentDetail != null && orderPaymentDetail.TotalPrice.HasValue)
                {
                    totalSales += orderPaymentDetail.TotalPrice.Value;
                }
            }

            return totalSales;
        }
    }
}
