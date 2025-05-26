using PMSCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;

namespace PMSWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public HomeController( IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(PaginationDetails paginationDetails)
        {
            ResponseResult result = await _dashboardService.GetAnalytics(paginationDetails);
            DashboardVM dashboardView = result.Data as DashboardVM?? new();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_DASHBOARD_GRID, (dashboardView,paginationDetails));

                    return Json(new
                    {
                        partialView = partialView,
                        paginationDetails = paginationDetails,
                        message = result.Message,
                        status = result.Status.ToString()
                    });
                }

            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View((dashboardView, paginationDetails));
        }
    }
}
