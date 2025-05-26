using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Attributes;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{

    public class TaxesController : Controller
    {
        private readonly ITaxesAndFeesService _taxesAndFeesService;

        public TaxesController(ITaxesAndFeesService taxesAndFeesService)
        {
            _taxesAndFeesService = taxesAndFeesService;
        }

        ResponseResult result = new();
        [AuthorizePermission(Constants.TAX_AND_FEE_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> Taxes(PaginationDetails pagination)
        {
            try
            {
                result = await _taxesAndFeesService.GetTaxes(pagination);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_TAX_LIST, (List<TaxDetails>)result.Data);
                    return Json(new
                    {
                        message = result.Message,
                        status = result.Status,
                        partialView = partialView,
                        paginationDetails = pagination
                    });
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            @TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View(((List<TaxDetails>)result.Data, pagination));
        }


        [HttpPost]
        [AuthorizePermission(Constants.TAX_AND_FEE_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> AddNewTax(TaxDetails taxDetails)
        {
            try
            {
                result = await _taxesAndFeesService.AddNewTax(taxDetails);
                (List<TaxDetails> taxesList, PaginationDetails paginationDetails) = ((List<TaxDetails>, PaginationDetails))result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_TAX_LIST, taxesList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = paginationDetails });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status.ToString() });
            }
        }

        [HttpPost]
        [AuthorizePermission(Constants.TAX_AND_FEE_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> UpdateTax(TaxDetails taxDetails)
        {
            try
            {
                result = await _taxesAndFeesService.UpdateTax(taxDetails);
                (List<TaxDetails> taxesList, PaginationDetails paginationDetails) = ((List<TaxDetails>, PaginationDetails))result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_TAX_LIST, taxesList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = paginationDetails });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status.ToString() });
            }

        }

        [HttpPost]
        [AuthorizePermission(Constants.TAX_AND_FEE_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteTax(TaxDetails taxDetails)
        {
            try
            {
                result = await _taxesAndFeesService.DeleteTax(taxDetails.TaxId, taxDetails.EditorId);
                (List<TaxDetails> taxesList, PaginationDetails paginationDetails) = ((List<TaxDetails>, PaginationDetails))result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_TAX_LIST, taxesList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = paginationDetails });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status.ToString() });
            }
        }
    }
}
