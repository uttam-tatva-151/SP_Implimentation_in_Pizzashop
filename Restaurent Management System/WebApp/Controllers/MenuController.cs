using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Attributes;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        ResponseResult result = new();

        [AuthorizePermission(Constants.MENU_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> Menu(PaginationDetails paginationDetails)
        {

            try
            {
                result = await _menuService.GetDefaultMenu(paginationDetails);
                ViewBag.paginationDetails = paginationDetails;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            MenuDetails menu = (MenuDetails)result.Data;
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View(menu);
        }

        #region Category
        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> AddCategory(CategoryDetails newCategory)
        {
            try
            {
                result = await _menuService.AddCategory(newCategory);
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_CATEGORY_LIST_GRID, result.Data as List<CategoryDetails> ?? new List<CategoryDetails>());
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }

        }

        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> EditCategory(CategoryDetails updateCategory)
        {
            try
            {
                result = await _menuService.EditCategory(updateCategory);
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_CATEGORY_LIST_GRID, result.Data as List<CategoryDetails> ?? new List<CategoryDetails>());
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteCategory(int categoryId, int editorId)
        {
            try
            {
                result = await _menuService.DeleteCategory(categoryId, editorId);
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_CATEGORY_LIST_GRID, result.Data as List<CategoryDetails> ?? new List<CategoryDetails>());
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
        #endregion

        #region Item

        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> GetItems(int id, PaginationDetails paginationDetails)
        {
            try
            {

                result = await _menuService.GetItems(id, paginationDetails);
                ViewBag.paginationDetails = paginationDetails;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            List<ItemDetails> itemList = (List<ItemDetails>)result.Data as List<ItemDetails> ?? new List<ItemDetails>();
            return PartialView(Constants.PARTIAL_ITEM_LIST_GRID, itemList);
        }
        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> AddItem([FromForm] AddItem newItem)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["newItem.IMDetails"]))
                {
                    newItem.IMDetails = JsonConvert.DeserializeObject<List<ItemModifierGroupRelation>>(Request.Form["newItem.IMDetails"]);
                }
                result = await _menuService.AddItem(newItem);
                (List<ItemDetails> itemList, PaginationDetails paginationDetails) = ((List<ItemDetails>, PaginationDetails))result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_ITEM_LIST_GRID, itemList);
                return Json(new { partiview = partialView, message = result.Message, status = result.Status, paginaiton = paginationDetails });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }

        }
        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> UpdateItem([FromForm] AddItem editItem)
        {

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["editItem.IMDetails"]))
                {
                    editItem.IMDetails = JsonConvert.DeserializeObject<List<ItemModifierGroupRelation>>(Request.Form["editItem.IMDetails"]);
                }
                result = await _menuService.UpdateItem(editItem);
                List<ItemDetails> itemList = result.Data as List<ItemDetails> ?? new List<ItemDetails>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_ITEM_LIST_GRID, itemList);
                return Json(new { data = partialView, message = result.Message, status = result.Status });

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { status = result.Status, message = result.Message });
            }
        }
        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteItemById(int itemId, int editorId)
        {
            try
            {
                result = await _menuService.DeleteItem(itemId, editorId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return RedirectToAction(Constants.MENU_VIEW,Constants.MENU_CONTROLLER);
        }
        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteMultipleItems(int[] ids, int editorId)
        {
            try
            {
                result = await _menuService.DeleteMultipleItems(ids, editorId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return Json(new { message = result.Message, status = result.Status });
        }

        [HttpGet]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> GetItemById(int itemId)
        {
            try
            {
                AddItem item = await _menuService.GetItemById(itemId);
                return Json(new { success = true, data = item });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = ex.Message });
            }

        }
        #endregion

        #region ModifierGroup
        [HttpGet]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> GetAllExistingModifers(PaginationDetails paginationDetails)
        {
            try
            {
                List<ModifierDetails> modifiers = await _menuService.GetAllModifiers(paginationDetails);
                // return Json(new { success = true, data = modifiers });
                ViewBag.paginationDetails = paginationDetails;
                return PartialView(Constants.PARTIAL_ALL_READY_EXISTING_MODIFIERS, modifiers);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteModifierGroup(int modifierGroupId, int editorId)
        {
            try
            {
                result = await _menuService.DeleteModifierGroup(modifierGroupId, editorId);
                List<ModifierGropDetails> groupList = result.Data as List<ModifierGropDetails> ?? new List<ModifierGropDetails>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_MODIFIERS_GROUP_LIST_GRID, groupList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> AddModifierGroup(ModifierGroupVM modifierGroupDetails)
        {
            try
            {
                result = await _menuService.AddModifierGroup(modifierGroupDetails);
                List<ModifierGropDetails> groupList = result.Data as List<ModifierGropDetails> ?? new List<ModifierGropDetails>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_MODIFIERS_GROUP_LIST_GRID, groupList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> UpdateModifierGroup(ModifierGroupVM modifierGroupDetails)
        {
            try
            {
                result = await _menuService.UpdateModifierGroup(modifierGroupDetails);
                List<ModifierGropDetails> groupList = result.Data as List<ModifierGropDetails> ?? new List<ModifierGropDetails>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_MODIFIERS_GROUP_LIST_GRID, groupList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
        #endregion

        #region Modifier

        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> GetModifiersList(int modifierGroupId, PaginationDetails paginationDetails)
        {
            try
            {
                result = await _menuService.GetModifiers(modifierGroupId, paginationDetails);
                ViewBag.paginationDetails = paginationDetails;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            List<ModifierDetails> modifierList = result.Data as List<ModifierDetails> ?? new List<ModifierDetails>();
            return PartialView(Constants.PARTIAL_MODIFIERS_LIST_GRID, modifierList);
            // return Json(modifierList);
        }

        [AuthorizePermission(Constants.MENU_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> GetModifierById(int modifierId)
        {
            try
            {
                result = await _menuService.GetModifierByModifierId(modifierId);
                ModifierVM modifier = (ModifierVM)result.Data;
                return Json(new { data = modifier, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        [AuthorizePermission(Constants.MENU_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> GetModifiersByGroupId(int groupId)
        {
            try
            {
                List<ModifierDetails> modifiers = await _menuService.GetModifiersByGroupId(groupId);
                result.Data = modifiers;
                if (modifiers != null)
                {
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.MODIFIER);
                    result.Status = ResponseStatus.Success;
                }
                else
                {
                    result.Message = MessageHelper.GetNotFoundMessage(Constants.MODIFIER);
                    result.Status = ResponseStatus.NotFound;
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return Json(new { modifiers = result.Data as List<ModifierDetails>, message = result.Message, status = result.Status });
        }

        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> AddModifier(ModifierVM newModifier)
        {
            try
            {
                result = await _menuService.AddNewModifier(newModifier);
                List<ModifierDetails> modifierList = (List<ModifierDetails>)result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_MODIFIERS_LIST_GRID, modifierList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ExceptionHelper.GetErrorMessage(ex);
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }

        }

        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> UpdateModifier(ModifierVM editModifier)
        {
            try
            {
                result = await _menuService.EditModifier(editModifier);
                List<ModifierDetails> modifierList = (List<ModifierDetails>)result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_MODIFIERS_LIST_GRID, modifierList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        [AuthorizePermission(Constants.MENU_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteModifierById(int modifierId, int editorId)
        {
            try
            {
                result = await _menuService.DeleteModifier(modifierId, editorId);
                List<ModifierDetails> modifierList = (List<ModifierDetails>)result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_MODIFIERS_LIST_GRID, modifierList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        [HttpPost]
        [AuthorizePermission(Constants.MENU_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteMultipleModifiers(int[] modifierIds, int editorId)
        {
            try
            {
                result = await _menuService.DeleteMultipleModifiers(modifierIds, editorId);
                List<ModifierDetails> modifierList = (List<ModifierDetails>)result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_MODIFIERS_LIST_GRID, modifierList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
            }
        }

        [HttpPost]
        public IActionResult ShowModifierGroup(List<ModifierGropDetails> groupList)
        {
            return PartialView(Constants.PARTIAL_MODIFIERS_GROUP_LIST_GRID, groupList);
        }
        #endregion















    }
}
