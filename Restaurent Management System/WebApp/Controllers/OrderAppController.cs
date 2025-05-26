using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Attributes;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{
    public class OrderAppController : Controller
    {
        private readonly IOrderAppService _orderAppService;

        public OrderAppController(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }
        ResponseResult result = new();
        [HttpGet]
        public async Task<IActionResult> TableView()
        {
            List<SectionDetails> sectionList = new();
            try
            {
                result = await _orderAppService.GetSectionList();
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
        public async Task<IActionResult> GetSectionView(int sectionId)
        {
            try
            {
                result = await _orderAppService.GetTableViews(sectionId);
                List<TableViewOrderAppVM> tableList = (List<TableViewOrderAppVM>)result.Data;
                return PartialView(Constants.PARTIAL_TABLE_VIEW_LIST_GRID, tableList);
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
                result = await _orderAppService.GetWaitingListBySectionId(sectionId);
                List<waitingTokenVM> waitingList = result.Data as List<waitingTokenVM> ?? new List<waitingTokenVM>();
                return PartialView(Constants.PARTIAL_WAITING_LIST_AT_ASSIGN_TABLE, waitingList);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignTable(waitingTokenVM tokenDetails)
        {

            try
            {
                result = await _orderAppService.AssignTable(tokenDetails);
                int orderId = (int)result.Data;
                return Json(new { orderId = orderId, message = result.Message, status = result.Status });
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
