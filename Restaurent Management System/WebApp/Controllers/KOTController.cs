using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{

    public class KOTController : Controller
    {


        private readonly IKOTService _kotService;

        public KOTController(IKOTService kotService)
        {
            _kotService = kotService;
        }
        ResponseResult result = new();

        [HttpGet]
        public async Task<IActionResult> KOT(string status = Constants.ORDER_IN_PROGRESS, int categoryId = 0)
        {
            try
            {
                result = await _kotService.GetKOTs(status, categoryId);
                (List<KOTVM>, List<CategoryDetails>) data = ((List<KOTVM>, List<CategoryDetails>))result.Data;
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView(Constants.PARTIAL_KOTS_GRID, data.Item1);
                }
                TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.ORDER_APP_LAYOUT;
                return View(data);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.ORDER_APP_LAYOUT;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateKOT(List<KOTVM.KOTItemsVM> kotItems, int orderId,int editorId)
        {
            try
            {
                if (kotItems == null || kotItems.Count == 0)
                {
                    return Json(new { status = ResponseStatus.NotFound, message = MessageHelper.GetWarningMessageForNoSection(Constants.ITEM) });
                }

                result = await _kotService.UpdateKOTItems(kotItems, orderId, editorId);
                ;
                if (result.Status == ResponseStatus.Success)
                {
                    List<KOTVM> kotList = result.Data as List<KOTVM> ?? new List<KOTVM>();
                    string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_KOTS_GRID, kotList);
                    return Json(new { partialView = partialView, status = result.Status, message = result.Message });
                }
                else
                {
                    return Json(new { status = result.Status, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = ResponseStatus.Error,
                    message = ExceptionHelper.GetErrorMessage(ex)
                });

            }

        }

    }
}
