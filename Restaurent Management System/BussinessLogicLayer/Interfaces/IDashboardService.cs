using PMSCore.Beans;
using PMSCore.ViewModel;


namespace PMSServices.Interfaces
{
    public interface IDashboardService
    {
        Task<ResponseResult> GetAnalytics(PaginationDetails paginationDetails);
    }
}