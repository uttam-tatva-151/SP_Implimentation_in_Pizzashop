using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSServices.Interfaces;
using PMSWebApp.Attributes;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;
namespace PMSWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICommonServices _commonServices;
        public UserController(IUserService userService, ICommonServices commonServices)
        {
            _userService = userService;
            _commonServices = commonServices;
        }
        ResponseResult result = new();
        [HttpGet]
        [AuthorizePermission(Constants.USERS_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> UserList(PaginationDetails paginationDetails)
        {
            IEnumerable<User> userList = new List<User>();
            try
            {
                result = await _userService.GetUsers(paginationDetails);
                userList = result.Data as IEnumerable<User> ?? new List<User>();
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_USER_GRID, userList);

                    return Json(new
                    {
                        partialView = partialView,
                        paginationDetails = paginationDetails,
                        message = result.Message,
                        status = result.Status.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);

            return View((userList, paginationDetails));
        }
        [AuthorizePermission(Constants.USERS_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> AddUser()
        {
            ResponseResult countryData = await _commonServices.GetCountryList();
            List<ContryList> countryList = countryData.Data as List<ContryList> ?? new List<ContryList>();
            ViewBag.CountryList = new SelectList(countryList, "ContryId", "ContryName");
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View();
        }
        [HttpPost]
        [AuthorizePermission(Constants.USERS_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> AddUser(NewUser newUser)
        {
            try
            {
                result = await _userService.AddUser(newUser);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            return RedirectToAction(Constants.ADD_USER_VIEW, Constants.USER_CONTROLLER);
        }

        [HttpGet]
        [AuthorizePermission(Constants.USERS_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> EditUser(int id)
        {
            try
            {
                if (id != 0)
                {
                    result = await _userService.GetUserDetails(id);
                }
                else
                {
                    result.Message = MessageHelper.GetNotFoundMessage(Constants.USER);
                    result.Status = ResponseStatus.NotFound;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ResponseResult countryData = await _commonServices.GetCountryList();
            List<ContryList> countryList = countryData.Data as List<ContryList> ?? new List<ContryList>();
            ViewBag.CountryList = new SelectList(countryList, "ContryId", "ContryName");
            UpdateUser user = (UpdateUser)result.Data;
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View(user);
        }
        [HttpPost]
        [AuthorizePermission(Constants.USERS_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> EditUser(UpdateUser updateUser)
        {
            try
            {
                result = await _userService.EditUser(updateUser);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            return RedirectToAction(Constants.USER_LIST_VIEW, Constants.USER_CONTROLLER);
        }

        [HttpPost]
        [AuthorizePermission(Constants.USERS_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteUser(int userId, int editor)
        {
            try
            {
                result = await _userService.DeleteUser(userId, editor);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            return RedirectToAction(Constants.USER_LIST_VIEW, Constants.USER_CONTROLLER);
        }

        // Helper Method
        public IActionResult GetImage(byte[] imgData)
        {
            return File(imgData, Constants.IMAGE_TYPE);
        }

    }
}
