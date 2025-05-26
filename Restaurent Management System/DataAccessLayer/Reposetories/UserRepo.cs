using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _appDbContext;

        public UserRepo(AppDbContext context)
        {
            _appDbContext = context;
        }
        private readonly ResponseResult result = new();
        // Get IMG by Email Id

        public async Task<Userauthentication?> GetUserAuthenticationAsync(int userId)
        {
            return await _appDbContext.Userauthentications.FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<ResponseResult> GetUserdetailsForProfileAsync(string emailId)
        {
            try
            {
                IQueryable<Userdetail> query = _appDbContext.Userdetails
                                    .Include(u => u.User) // UserdetailUsers
                                    .Include(u => u.User.Role)
                                    .Where(u => u.User.EmailId == emailId && u.User.Iscontinued == true);

                Userdetail userData = await query.FirstOrDefaultAsync()
                    ?? throw new Exception(MessageHelper.GetNotFoundMessage(Constants.USER));

                result.Data = userData;
                result.Status = ResponseStatus.Success;
                result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.USER);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<byte[]> GetUserProfileImgByEmailAsyncAsByteStream(string email)
        {
            byte[] imgPath = await _appDbContext.Userdetails
                                            .Include(u => u.User)
                                            .Where(e => e.User.EmailId == email)
                                            .Select(i => i.Photo)
                                            .FirstOrDefaultAsync() ?? Array.Empty<byte>();
            return imgPath;
        }
        // Get User Name by email id 
        public async Task<string?> GetUserNameByEmailAsync(string email)
        {
            try
            {
                string? userName = await _appDbContext.Userauthentications
                                        .Where(u => u.EmailId == email)
                                        .Select(u => u.UserName)
                                        .FirstOrDefaultAsync();

                return userName;
            }
            catch 
            {
                throw;
            }
        }
        // Fetch user authentication details by email
        public async Task<Userdetail?> GetUserDataAsync(string email)
        {
            try
            {
                return await _appDbContext.Userdetails
                    .Include(u => u.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefaultAsync(u => u.User.EmailId == email && u.User.Iscontinued == true);
            }
            catch 
            {
                 throw;
            }
        }

        // Get user ID by email
        public async Task<int?> GetUserIdByEmailAsync(string email)
        {
            try
            {
                int userId = await _appDbContext.Userauthentications
                    .Where(u => u.EmailId == email)
                    .Select(u => (int?)u.UserId)
                    .FirstOrDefaultAsync() ?? 0;

                return userId;
            }
            catch
            {
                throw;
            }
        }

        // Get email by user ID
        public async Task<string?> GetEmailByUserIdAsync(int userId)
        {
            try
            {
                return await _appDbContext.Userauthentications
                    .Where(u => u.UserId == userId)
                    .Select(u => u.EmailId)
                    .FirstOrDefaultAsync();
            }
            catch  
            {
                 throw;
            }
        }

        // Check if an email already exists in the database
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            try
            {
                return await _appDbContext.Userauthentications
                    .AnyAsync(u => u.EmailId == email);
            }
            catch  
            {
                 throw;
            }
        }

        // Fetch complete user details by email
        public async Task<Userauthentication?> GetUserDetailsByEmailAsync(string email)
        {
            try
            {
                return await _appDbContext.Userauthentications
                    .Include(u => u.Userdetail)
                    .FirstOrDefaultAsync(u => u.EmailId == email);
            }
            catch  
            {
                 throw;
            }
        }

        // Get all users (excluding deleted users)
        public async Task<List<Userauthentication>> GetAllUsersAsync()
        {
            try
            {
                return await _appDbContext.Userauthentications
                    .Where(u => !u.Iscontinued ?? false)
                    .ToListAsync();
            }
            catch  
            {
                throw;
            }
        }
        // Get User Details byUser d
        public async Task<Userdetail?> GetUserDetailsByUserIdAsync(int id)
        {
            return await _appDbContext.Userdetails
                                    .Include(u => u.User)
                                    .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<ResponseResult> AddNewUserAsync(Userauthentication user)
        {
            try
            {
                if (user == null)
                {
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.USER);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                _appDbContext.Userauthentications.Add(user);
                await _appDbContext.SaveChangesAsync(); // Save to generate UserId
                result.Data = await _appDbContext.Userauthentications
                                        .Where(u => u.EmailId == user.EmailId)
                                        .Select(u => u.UserId)
                                        .FirstOrDefaultAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.USER);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> AddUserDataAsync(Userdetail userData)
        {
            try
            {
                if (userData == null)
                {
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.USER);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                _appDbContext.Userdetails.Add(userData);
                await _appDbContext.SaveChangesAsync(); // Save to generate UserId

                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.USER);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> GetUsers(PaginationDetails paginationDetails)
        {
            try
            {
                IQueryable<Userdetail> query = _appDbContext.Userdetails
                                    .Include(u => u.User.Role)
                                    .Where(u => u.Isactive == true && u.User.Iscontinued == true);

                if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
                {
                    query = query.Where(u => u.FirstName.ToLower().Contains(paginationDetails.SearchQuery) ||
                                             u.User.EmailId.ToLower().Contains(paginationDetails.SearchQuery) ||
                                             u.User.Role.RoleName.ToLower().Contains(paginationDetails.SearchQuery));
                }

                switch (paginationDetails.SortColumn.ToLower())
                {
                    case "name":
                        query = (paginationDetails.SortOrder == Constants.ASC_ORDER) ? query.OrderBy(e => e.FirstName) : query.OrderByDescending(e => e.FirstName);
                        break;
                    case "role":
                        query = (paginationDetails.SortOrder == Constants.ASC_ORDER) ? query.OrderBy(e => e.User.Role.RoleName) : query.OrderByDescending(e => e.User.Role.RoleName);
                        break;
                    default:
                        query = query.OrderBy(e => e.FirstName); // Default sorting
                        break;
                }
                paginationDetails.TotalRecords = await query.CountAsync(); // Count filtered results

                List<User> userList = await query
                                .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                                .Take(paginationDetails.PageSize)
                                .Select(u => new User
                                {
                                    Id = u.UserId,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    Email = u.User.EmailId,
                                    PhoneNumber = u.PhoneNumber,
                                    Role = u.User.Role.RoleName,
                                    Status = u.Status,
                                    imgData = u.Photo
                                })
                                .ToListAsync();

                result.Data = userList;
                result.Status = ResponseStatus.Success;
                result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.USER_LIST);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> GetUserDetailsAsync(int userId)
        {

            try
            {
                IQueryable<Userdetail> query = _appDbContext.Userdetails.Include(u => u.User).Where(u => u.UserId == userId);
                Userdetail? userdetail = await query.FirstOrDefaultAsync();

                if (userdetail != null)
                {
                    UpdateUser user = new()
                    {
                        UserId = userdetail.UserId,
                        EmailId = userdetail.User.EmailId,
                        FirstName = userdetail.FirstName,
                        LastName = userdetail.LastName,
                        UserName = userdetail.User.UserName,
                        RoleId = userdetail.User.RoleId,
                        Status = userdetail.Status,
                        PhoneNumber = userdetail.PhoneNumber,
                        Address = userdetail.Address,
                        ZipCode = userdetail.ZipCode,
                        // user.Photo = userdetail.PhotoData;
                        ContryId = userdetail.ContryId,
                        StateId = userdetail.StateId,
                        CityId = userdetail.CityId
                    };

                    result.Data = user;
                    result.Status = ResponseStatus.Success;
                    result.Message =MessageHelper.GetSuccessMessageForReadOperation(Constants.USER);
                }
                else
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.USER);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            return result;
        }
        public static byte[] ConvertImageToByteArray(IFormFile file)
        {
            using MemoryStream memoryStream = new();
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<Userauthentication?> GetUserByEmail(string email)
        {
            return await _appDbContext.Userauthentications.FirstOrDefaultAsync(u => u.EmailId == email);
        }

        public async Task<ResponseResult> UpdateUserAuthenticationAsync(Userauthentication user)
        {
            try
            {
                if (user == null)
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.USER);
                }
                else
                {
                    _appDbContext.Userauthentications.Update(user);
                    await _appDbContext.SaveChangesAsync();
                    result.Status = ResponseStatus.Success;
                    result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.USER);
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateUserDetailsAsync(Userdetail user)
        {
            try
            {
                if (user == null)
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.USER);
                }
                else
                {
                    _appDbContext.Userdetails.Update(user);
                    await _appDbContext.SaveChangesAsync();
                    result.Status = ResponseStatus.Success;
                    result.Message =  MessageHelper.GetSuccessMessageForUpdateOperation(Constants.USER);
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
    }
}
