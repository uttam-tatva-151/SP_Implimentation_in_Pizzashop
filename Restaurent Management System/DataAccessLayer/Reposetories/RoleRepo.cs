using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class RoleRepo : IRoleRepo
    {

        private readonly AppDbContext _context;

        public RoleRepo(AppDbContext context)
        {
            _context = context;
        }
        readonly ResponseResult result = new();

        public async Task<ResponseResult> GetPermissionList(string roleName)
        {
            try
            {
                if (roleName != null)
                {
                    IQueryable<Permission> query = _context.Permissions
                                        .Include(p => p.Role)
                                        .Include(p => p.Module)
                                        .Where(p => p.Role.RoleName == roleName);
                    List<Permission> permissionList =  await query.ToListAsync();
                    result.Data = permissionList;
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.PERMISSION_LIST);
                }
                else
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.PERMISSION_LIST);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> UpdatePermission(PermissionDetails permissionDetails)
{
    try
    {
        IQueryable<Permission> query = _context.Permissions.Where(x => x.PId == permissionDetails.PermissionId);
        Permission? permission = query.FirstOrDefault();
        if (permission != null)
        {
            permission.CanDelete = permissionDetails.CanDelete;
            permission.CanView = permissionDetails.CanView;
            permission.CanCreateandedit = permissionDetails.CanCreateandedit;

            permission.Modifyat = DateTime.Now;
            permission.Modifyby = permissionDetails.EditorId;

            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();
            result.Status = ResponseStatus.Success;
            result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.PERMISSION_LIST);
        }
        else
        {
            result.Status = ResponseStatus.Success;
            result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.PERMISSION_LIST);
        }
    }
    catch (Exception ex)
    {
        result.Message = ex.Message;
        result.Status = ResponseStatus.Error;
    }
    return result;
}

public async Task<bool> UserHasPermission(int roleId, string moduleName, string permissionType)
{
    Permission? permission = await _context.Permissions
        .Include(p => p.Module)
        .Where(p => p.RoleId == roleId && p.Module.ModuleName == moduleName)
        .FirstOrDefaultAsync();

    if (permission == null)
    {
        return false; // No permission found
    }

    return permissionType switch
    {
        Constants.VIEW_PERMISSION => permission.CanView,
        Constants.CREATE_AND_EDIT_PERMISSION => permission.CanCreateandedit,
        Constants.DELETE_PERMISSION => permission.CanDelete,
        _ => false
    };
}

public Task<PermissionDetails?> GetPermissionDetailsAsync(string roleName, string moduleName)
{
    return _context.Permissions
        .Include(p => p.Module)
        .Include(p => p.Role)
        .Where(p => p.Role.RoleName == roleName && p.Module.ModuleName == moduleName)
        .Select(p => new PermissionDetails
        {
            PermissionId = p.PId,
            RoleId = p.RoleId,
            ModuleId = p.ModuleId,
            CanCreateandedit = p.CanCreateandedit,
            CanDelete = p.CanDelete,
            CanView = p.CanView,
            ModuleName = p.Module.ModuleName
        })
        .FirstOrDefaultAsync();
}

    }
}
