using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{
    public class WaitingListController : Controller
    {
        private readonly IWaitingListService _waitingListService;

        public WaitingListController(IWaitingListService waitingListService)
        {
            _waitingListService = waitingListService;
        }
        ResponseResult result = new();

        public async Task<IActionResult> WaitingList()
        {
            List<SectionDetails> sectionList = new();
            try
            {
                result = await _waitingListService.GetSectionList();
                sectionList = (List<SectionDetails>)result.Data;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.ORDER_APP_LAYOUT;
            return View(sectionList);
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetails(string emailId)
        {
            try
            {
                result = await _waitingListService.GetCustomerDetails(emailId);
                CustomerDetails customerDetails = (CustomerDetails)result.Data;
                return Json(new { customerDetails = customerDetails });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewToken(waitingTokenVM token)
        {
            try
            {
                result = await _waitingListService.AddWaitingToken(token);
                List<waitingTokenVM> waitingList = result.Data as List<waitingTokenVM> ?? new List<waitingTokenVM>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_WAITING_TOKEN, waitingList);
                return Json(new { message = result.Message, status = result.Status, partialView = partialView });

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateToken(waitingTokenVM token)
        {
            try
            {
                result = await _waitingListService.EditWaitingToken(token);
                List<waitingTokenVM> waitingList = result.Data as List<waitingTokenVM> ?? new List<waitingTokenVM>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_WAITING_TOKEN, waitingList);
                return Json(new { message = result.Message, status = result.Status, partialView = partialView });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteToken(int tokenId, int editorId)
        {
            try
            {
                result = await _waitingListService.DeleteWaitingToken(tokenId,editorId);
                List<waitingTokenVM> waitingList = result.Data as List<waitingTokenVM> ?? new List<waitingTokenVM>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_WAITING_TOKEN, waitingList);
                return Json(new { message = result.Message, status = result.Status, partialView = partialView });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWaitingList(int sectionId)
        {
            try
            {
                result = await _waitingListService.GetWaitingListBySectionId(sectionId);
                List<waitingTokenVM> waitingList = result.Data as List<waitingTokenVM> ?? new List<waitingTokenVM>();
                return PartialView(Constants.PARTIAL_WAITING_TOKEN, waitingList);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
    }
}
