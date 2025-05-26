using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces
{
    public interface IRoleService
    {
        Task<ResponseResult> GetPermissionList(string roleName);
        Task<bool> HasPermission(int roleId, string moduleName, string permissionType);
        Task<ResponseResult> UpdatePermissions(List<PermissionDetails> permissionDetailsList);
        Task<PermissionDetails?> GetPermission(string roleName, string moduleName);
        Task<List<PermissionDetails> > GetAllPermissions(string roleName);
    }

}
