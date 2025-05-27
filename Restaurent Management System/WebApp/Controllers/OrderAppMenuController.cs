using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{

    public class OrderAppMenuController : Controller
    {
        private readonly IOrderAppMenuService _orderAppMenuService;

        public OrderAppMenuController(IOrderAppMenuService orderAppMenuService)
        {
            _orderAppMenuService = orderAppMenuService;
        }
        ResponseResult result = new();

        public async Task<IActionResult> OrderAppMenu()
        {
            List<CategoryDetails> categoryList = new();
            try
            {
                result = await _orderAppMenuService.GetCategoryList();
                categoryList = (List<CategoryDetails>)result.Data;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData,result.Message,result.Status);
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.ORDER_APP_LAYOUT;
            return View(categoryList);
        }

        public async Task<IActionResult> CustomersMenu() //for Static Menu load for Customer
        {
            MenuDetails menu = new();
            try
            {
                result = await _orderAppMenuService.GetDefaultMenu();
                menu = (MenuDetails)result.Data;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData,result.Message,result.Status);
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.ORDER_APP_LAYOUT;
            return View(menu);
        }

        public async Task<IActionResult> GetMenuItems(bool favoritesItem, int categoryId, string searchQuery)
        {

            try
            {
                int userId =  User.GetUserId();
                result = await _orderAppMenuService.GetMenuItems(favoritesItem, categoryId, searchQuery, userId);
                List<ItemDetails> itemList = (List<ItemDetails>)result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_ORDER_APP_MENU_ITEM_LIST_GRID, itemList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = ResponseStatus.Error.ToString() });
            }
        }

        public async Task<IActionResult> AddToFavorites(int itemId,int editorId)
        {
            try
            {
                result = await _orderAppMenuService.AddToFavorites(itemId,editorId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
                return Json(new { message = result.Message, status = result.Status });
        }
        public async Task<IActionResult> RemoveFromFavorites(int itemId, int editorId)
        {
            try
            {
                result = await _orderAppMenuService.RemoveFromFavorites(itemId, editorId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
                return Json(new { message = result.Message, status = result.Status });
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            try
            {
                result = await _orderAppMenuService.GetOrderDetailsAsync(orderId);
                OrderExportDetails orderDetils = result.Data as OrderExportDetails ?? new();
                return PartialView(Constants.PARTIAL_ORDER_APP_MENU_ORDER_PLACE, orderDetils);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItemMapping(int itemId)
        {
            try
            {
                result = await _orderAppMenuService.GetMenuItemMapping(itemId);
                ItemModifierGroupRelationVM itemModifierGroupRelationVM = result.Data as ItemModifierGroupRelationVM ?? new();
                return PartialView(Constants.PARTIAL_ORDER_APP_MENU_ITEM_MODIFIER_GROUP_RELATION_GRID, itemModifierGroupRelationVM);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(OrderExportDetails order)
        {
            try
            {
                result = await _orderAppMenuService.UpdateOrder(order);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
                return Json(new { message = result.Message, status = result.Status });

        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomerReview(CustomerReviewViewModel reviewData)
        {
            try
            {
                result = await _orderAppMenuService.AddCustomerFeedback(reviewData);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
                return Json(new { message = result.Message, status = result.Status });

        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(string orderStatus,int orderId ,int editorId)
        {
            try
            {
                result = await _orderAppMenuService.UpdateOrderStatus(orderStatus,orderId,editorId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
                return Json(new { message = result.Message, status = result.Status });

        }
    
    
    }
}
