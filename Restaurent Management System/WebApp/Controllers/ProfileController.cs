using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSServices.Interfaces;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{
    public class ProfileController : Controller
    {

        private readonly IProfileService _profileService;
        private readonly ICommonServices _commonServices;
        private readonly IUserService _userService;
        private readonly IJWTService _jwtService;
        public ProfileController(IProfileService profileService, ICommonServices commonServices, IUserService userService, IJWTService jwtService)
        {
            _profileService = profileService;
            _commonServices = commonServices;
            _userService = userService;
            _jwtService = jwtService;
        }
        ResponseResult result = new();

        public async Task<IActionResult> UserProfile()
        {
            try
            {
                string email = HttpContext.Session.GetString(Constants.SESSION_EMAIL) ?? string.Empty;
                if (email != null)
                {
                    result = await _profileService.GetProfileAsync(email);
                    UserProfileVM data = (UserProfileVM)result.Data;
                    ResponseResult countryData = await _commonServices.GetCountryList();
                    List<ContryList> countryList = countryData.Data as List<ContryList> ?? new List<ContryList>();
                    ViewBag.CountryList = new SelectList(countryList, "ContryId", "ContryName");
                    TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                result.Message = ExceptionHelper.GetErrorMessage(ex);
                result.Status = ResponseStatus.Error;
                ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            }
            return RedirectToAction(Constants.LOGIN_VIEW, Constants.LOGIN_CONTROLLER);
        }
        public IActionResult ChangePassword()
        {
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            try
            {
                result = await _profileService.ChangePassword(changePasswordVM);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            return View();
        }

        public async Task<IActionResult> UpdateProfile(UserProfileVM profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    result = await _profileService.UpdateProfileAsync(profile);
                }
                else
                {
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.USER);
                    result.Status = ResponseStatus.Error;
                }
            }
            catch (Exception ex)
            {
                result.Message = ExceptionHelper.GetErrorMessage(ex);
                result.Status = ResponseStatus.Error;
            }
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            return RedirectToAction(Constants.DASHBOARD_VIEW, Constants.HOME_CONTROLLER);

        }
        public IActionResult Logout()
        {
            if (Request.Cookies.TryGetValue(Constants.REFRESH_TOKEN, out string? refreshToken) && refreshToken != null)
            {
                _jwtService.RevokeRefreshToken(refreshToken);
            }

            Response.Cookies.Delete(Constants.ACCESS_TOKEN);
            Response.Cookies.Delete(Constants.REFRESH_TOKEN);
            HttpContext.Session.Remove(Constants.SESSION_EMAIL);
            HttpContext.Session.Remove(Constants.SESSION_USERNAME);
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            return RedirectToAction(Constants.LOGIN_VIEW, Constants.LOGIN_CONTROLLER);
        }

        // Helpers
        public async Task<IActionResult> GetStates(int countryId)
        {
            ResponseResult stateList = await _commonServices.GetStateList(countryId); // Await the method call if it's async

            if (stateList.Data != null)
            {
                return Json(stateList.Data); // Return state list in JSON format
            }

            return Json(new { success = false, message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.STATE) }); // Return a failure response
        }

        public async Task<IActionResult> GetCities(int stateId)
        {
            ResponseResult cityList = await _commonServices.GetCityList(stateId); // Await the method call if it's async

            if (cityList.Data != null)
            {
                return Json(cityList.Data); // Return city list in JSON format
            }

            return Json(new { success = false, message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.STATE) }); // Return a failure response
        }

        public async Task<IActionResult> GetUserPhoto(string email)
        {
            byte[] UserImg = await _userService.GetUserProfileImgByEmailAsByteStream(email);
            return File(UserImg, "image/jpeg");  // Return image as a file
        }
    }
}
