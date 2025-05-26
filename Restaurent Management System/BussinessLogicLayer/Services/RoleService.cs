using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;


namespace PMSServices.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepo _roleRepo;

        public RoleService(IRoleRepo roleRepo)
        {
            _roleRepo = roleRepo;
        }

        ResponseResult result = new();

        public async Task<ResponseResult> GetPermissionList(string roleName)
        {
            try
            {
                result = await _roleRepo.GetPermissionList(roleName);
                List<Permission> permissionList = result.Data as List<Permission> ?? new();
                List<PermissionDetails> permissionDetails = ConvertToPermissionDetails(permissionList);
                result.Data = permissionDetails;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> UpdatePermissions(List<PermissionDetails> permissionDetailsList)
        {
            try
            {
                foreach (PermissionDetails permission in permissionDetailsList)
                {
                    result = await _roleRepo.UpdatePermission(permission);
                    if (result.Status == ResponseStatus.Error)
                    {
                        result.Message = MessageHelper.GetErrorMessageForUpdateOperation(permission.ModuleName) + permission.ModuleName;
                        result.Status = ResponseStatus.NotFound;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<bool> HasPermission(int roleId, string moduleName, string permissionType)
        {
            return await _roleRepo.UserHasPermission(roleId, moduleName, permissionType);
        }

        public async Task<PermissionDetails?> GetPermission(string roleName, string moduleName)
        {
            return await _roleRepo.GetPermissionDetailsAsync(roleName, moduleName);
        }

        public async Task<List<PermissionDetails>> GetAllPermissions(string roleName)
        {
            try
            {
                result = await _roleRepo.GetPermissionList(roleName);
                List<Permission> permissionList = result.Data as List<Permission> ?? new();
                List<PermissionDetails> permissionDetails = ConvertToPermissionDetails(permissionList);
                return permissionDetails;
            }
            catch
            {
                throw;
            }
        }
        private static List<PermissionDetails> ConvertToPermissionDetails(List<Permission> permissions)
        {
            return permissions.Select(permission => new PermissionDetails
            {
                PermissionId = permission.PId,
                ModuleId = permission.ModuleId,
                ModuleName = permission.Module?.ModuleName ?? string.Empty,
                RoleId = permission.RoleId,
                CanCreateandedit = permission.CanCreateandedit,
                CanView = permission.CanView,
                CanDelete = permission.CanDelete
            }).ToList();
        }
    }
}
