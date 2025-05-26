using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Attributes;
using PMSWebApp.Utilities;


namespace PMSWebApp.Controllers
{
    public class RoleAndPermissions : Controller
    {
        private readonly IRoleService _roleService;

        public RoleAndPermissions(IRoleService roleService)
        {
            _roleService = roleService;
        }

        ResponseResult result = new();
        [AuthorizePermission(Constants.ROLE_AND_PERMISSION_MODULE, Constants.VIEW_PERMISSION)]
        public IActionResult Role()
        {
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View();
        }
        [HttpGet]
        [AuthorizePermission(Constants.ROLE_AND_PERMISSION_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> Permissions(string roleName)
        {

            ViewBag.RoleName = roleName;
            try
            {
                result = await _roleService.GetPermissionList(roleName);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            List<PermissionDetails> permissionList = (List<PermissionDetails>)result.Data;
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View(permissionList);
        }
        [AuthorizePermission(Constants.ROLE_AND_PERMISSION_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> UpdatePermissions(List<PermissionDetails> Permissions)
        {

            try
            {
                result = await _roleService.UpdatePermissions(Permissions);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            return RedirectToAction(Constants.PERMISSION_VIEW, Constants.ROLE_CONTROLLER);
        }

         public async Task<IActionResult> GetALlPermissions()
        {

            try
            {
                string roleName = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(roleName))
                    return Json(new { success = false, message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ROLE) });
                List<PermissionDetails> permissionDetails = await _roleService.GetAllPermissions(roleName);
                return Json(new { success = true, permissionDetails });
            }
            catch
            {
                return Json(new { success = false, message = Constants.GENERAL_ERROR});
            }
        }
    }
}
