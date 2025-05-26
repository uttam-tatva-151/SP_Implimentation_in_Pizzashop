using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        ResponseResult result = new();
        public async Task<IActionResult> Customer(PaginationDetails paginationDetails){
            try{
                result = await _customerService.GetCustomerList(paginationDetails);
            }catch(Exception ex){
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            IEnumerable<CustomerDetails> cutomerList = (IEnumerable<CustomerDetails>)result.Data;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_CUSTOMER_TABLE_GRID,cutomerList);
                return Json(new
                {
                    partialView = partialView,
                    message = result.Message,
                    status = result.Status,
                    paginationDetails = paginationDetails,
                });
            }
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            ToasterHelper.SetToastMessage(TempData,result.Message,result.Status);
            return View((cutomerList, paginationDetails));
        
        }
        public async Task<IActionResult> ExportData(PaginationDetails paginationDetails)
        {
            try
            {
                result  = await _customerService.ExportCustomerDataAsync(paginationDetails);
            
                (byte[] fileContent, string contentType, string fileName) = ((byte[], string, string))result.Data;
                ToasterHelper.SetToastMessage(TempData,result.Message,result.Status);
                return File(fileContent, contentType, fileName);
            }
            catch (Exception ex)
            {
                result.Message = ExceptionHelper.GetErrorMessage(ex);
                result.Status = ResponseStatus.Error;
                ToasterHelper.SetToastMessage(TempData,result.Message,result.Status);
                return RedirectToAction(Constants.CUTOMER_VIEW, Constants.CUTOMER_CONTROLLER);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerHistory(int customerId)
        {
            try
            {
                result = await _customerService.GetCustomerHistoryAsync(customerId);
                CustomerHistory customerHistory = result.Data as CustomerHistory?? new CustomerHistory();
                return PartialView(Constants.PARTIAL_CUSTOMER_HISTORY_GRID,customerHistory);
            }
            catch (Exception ex)
            {
                result.Message = ExceptionHelper.GetErrorMessage(ex);
                result.Status = ResponseStatus.Error;            
                ToasterHelper.SetToastMessage(TempData,result.Message,result.Status);
                return RedirectToAction(Constants.CUTOMER_VIEW, Constants.CUTOMER_CONTROLLER);
            }
        }
    
        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(CustomerDetails customer){
            try{
                result = await _customerService.UpdateCustomerAsync(customer);
            }catch(Exception ex){
                result.Message = ExceptionHelper.GetErrorMessage(ex);
                result.Status = ResponseStatus.Error;
            }
            return Json(new {message = result.Message, status = result.Status});
        }
    }
}
