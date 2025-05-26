using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces
{
    public interface IRoleRepo
    {
        Task<PermissionDetails?> GetPermissionDetailsAsync(string roleName, string moduleName);
        Task<ResponseResult> GetPermissionList(string roleName);
        Task<ResponseResult> UpdatePermission(PermissionDetails permissionDetails);
        Task<bool> UserHasPermission(int roleId, string moduleName, string permissionType);
    }

}
