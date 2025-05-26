using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMSCore.Beans;
using PMSServices.Interfaces;
using System.Security.Claims;

namespace PMSWebApp.Attributes
{
    public class AuthorizePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _moduleName;
        private readonly string _permissionType; // "can_view", "can_createandedit", "can_delete"

        public AuthorizePermissionAttribute(string moduleName, string permissionType)
        {
            _moduleName = moduleName;
            _permissionType = permissionType;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            ClaimsPrincipal user = context.HttpContext.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                RedirectToErrorPage(context, 401, "Unauthorized Access");
                return;
            }

            string? roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            int roleIdClaim = MapRoleIdToRoleName(roleClaim);
            if (string.IsNullOrEmpty(roleClaim) || roleIdClaim == 0)
            {
                RedirectToErrorPage(context, 403, "Forbidden: Invalid Role");
                return;
            }
            IRoleService? permissionService = context.HttpContext.RequestServices.GetService<IRoleService>();
            if (permissionService == null || !permissionService.HasPermission(roleIdClaim, _moduleName, _permissionType).Result)
            {
                RedirectToErrorPage(context, 403, "Forbidden: Insufficient Permissions");
                return;
            }
        }
        private static int MapRoleIdToRoleName(string? role)
        {

            return role switch
            {
                Constants.ADMIN_ROLE => 1,
                Constants.CHEF_ROLE => 2,
                Constants.ACCOUNT_MANAGER_ROLE => 3,
                _ => 0 // Default role
            };
        }
        // Helper method to redirect to custom error page
        private static void RedirectToErrorPage(AuthorizationFilterContext context, int statusCode, string message)
        {
            context.Result = new RedirectToActionResult("HttpStatusCodeHandler", "ErrorHandler", new { statusCode, message });
        }
    }

}


