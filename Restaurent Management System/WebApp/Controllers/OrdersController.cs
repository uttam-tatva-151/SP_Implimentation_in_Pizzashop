using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Attributes;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrdersService _ordersService;
        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        ResponseResult result = new();

        [AuthorizePermission(Constants.ORDER_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> Order(PaginationDetails paginationDetails)
        {

            try
            {
                result = await _ordersService.GetOrderList(paginationDetails);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            IEnumerable<OrderDetailsVM> orderList =  result.Data as IEnumerable<OrderDetailsVM> ?? Enumerable.Empty<OrderDetailsVM>();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_ORDER_TABLE, orderList);
                return Json(new
                {
                    message = result.Message,
                    status = result.Status,
                    partialView = partialView,
                    paginationDetails = paginationDetails
                });
            }
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View((orderList, paginationDetails));
        }

        [HttpGet]
        [AuthorizePermission(Constants.ORDER_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            try
            {
                result = await _ordersService.GetOrderDetailsAsync(orderId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View(result.Data as OrderExportDetails);
        }
        [HttpGet]
        [AuthorizePermission(Constants.ORDER_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> ExportData(string orderSearch, string OrderStatus, string dateRange)
        {
            try
            {
                result = await _ordersService.ExportOrderList(orderSearch, OrderStatus, dateRange);
                byte[] fileContent = result.Data as byte[]?? Array.Empty<byte>();
                string currentDate = DateOnly.FromDateTime(DateTime.Now).ToString(Constants.DATE_FORMATE);
                string fileName = $"OrderSalesData_{currentDate}.xlsx";

                // Return the file as a response
                ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
                return File(fileContent, Constants.EXCEL_CONTENT_TYPE, fileName);
            }
            catch (Exception ex)
            {
                result.Message = ExceptionHelper.GetErrorMessage(ex);
                result.Status = ResponseStatus.Error;
                ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
                return RedirectToAction(Constants.ORDER_VIEW, Constants.ORDER_CONTROLLER);
            }
        }

        // [AuthorizePermission(Constants.ORDER_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> DownloadInvoice(int orderId)
        {
            try
            {
                result = await _ordersService.GetOrderDetailsAsync(orderId);
                OrderExportDetails invoice = result.Data as OrderExportDetails?? new();


                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_INVOICE, invoice);
                (byte[] pdfArray, string fileName) =  _ordersService.CreatePdf(invoice.InvoiceNo, partialView);


                return File(pdfArray, Constants.PDF_CONTENT_TYPE, fileName);
            }
            catch (Exception ex)
            {
                result.Message = ExceptionHelper.GetErrorMessage(ex);
                result.Status = ResponseStatus.Error;
                ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
                return RedirectToAction(Constants.ORDER_VIEW, Constants.ORDER_CONTROLLER);
            }
        }

    }
}
