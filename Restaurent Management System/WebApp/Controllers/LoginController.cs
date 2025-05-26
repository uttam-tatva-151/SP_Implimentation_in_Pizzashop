using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.DTOs;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Utilities;

public class LoginController : Controller
{
    private readonly IAuthService _bllAuthService;
    private readonly IJWTService _jwtService;

    public LoginController(IAuthService bllAuthService, IJWTService jwtService)
    {
        _bllAuthService = bllAuthService;
        _jwtService = jwtService;
    }

    private ResponseResult result = new();

    public async Task<IActionResult> Index()
    {
        try
        {
            string userToken = CookieHelper.GetCookieValue(Request, Constants.REFRESH_TOKEN) ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(userToken))
            {
                UserDto user = await _jwtService.ValidateAndGenerateTokenAsync(userToken);
                SetAuthData(Response, HttpContext, user.AccessToken, user.Email, user.UserName);
                return RedirectToAction(Constants.DASHBOARD_VIEW, Constants.HOME_CONTROLLER);
            }
        }
        catch (Exception ex)
        {
            return ExceptionHelper.RedirectToErrorPage(ex);
        }

        TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.LOGIN_LAYOUT;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        try
        {
            result = await _bllAuthService.LoginUser(loginRequest);
            if (result.Status == ResponseStatus.Success && result.Data is UserDto user)
            {
                string accessToken = await _jwtService.GenerateAccessToken(loginRequest.EmailId);

                if (loginRequest.IsRememberMe)
                {
                    string refreshToken =  _jwtService.GenerateRefreshToken();
                    await _jwtService.SaveRefreshToken(user.UserId, refreshToken);
                    CookieHelper.AppendCookie(Response, Constants.REFRESH_TOKEN, refreshToken, 30 * 24 * 60); // 1 month
                }

                SetAuthData(Response, HttpContext, accessToken, user.Email, user.UserName);

                ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
                return RedirectToAction(Constants.DASHBOARD_VIEW, Constants.HOME_CONTROLLER);
            }
        }
        catch (Exception ex)
        {
            result.Status = ResponseStatus.Error;
            result.Message = ExceptionHelper.GetErrorMessage(ex);
        }

        ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
        return RedirectToAction(Constants.LOGIN_VIEW, Constants.LOGIN_CONTROLLER);
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            if (!Request.Cookies.TryGetValue(Constants.REFRESH_TOKEN, out string? refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(MessageHelper.GetNotFoundMessage(Constants.REFRESH_TOKEN));
            }

            UserDto user = await _jwtService.ValidateAndGenerateTokenAsync(refreshToken);
            if (user == null)
            {
                return Unauthorized(MessageHelper.GetNotFoundMessage(Constants.SESSION_EMAIL));
            }

            string newAccessToken = await _jwtService.GenerateAccessToken(user.Email);
            CookieHelper.AppendCookie(Response, Constants.ACCESS_TOKEN, newAccessToken, 15); // 15 minutes

            return Ok(new { message = Constants.SUCCESS_TOKEN_REFRESHED });
        }
        catch (Exception ex)
        {
            return ExceptionHelper.RedirectToErrorPage(ex);
        }
    }

    public IActionResult ForgotPassword()
    {
        TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.LOGIN_LAYOUT;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendEmailLink(string email)
    {
        try
        {
            result = await _bllAuthService.SendForgotPassLink(email);
        }
        catch (Exception ex)
        {
            result.Status = ResponseStatus.Error;
            result.Message = ExceptionHelper.GetErrorMessage(ex);
        }
        ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
        return RedirectToAction(Constants.LOGIN_VIEW, Constants.LOGIN_CONTROLLER);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(UpdatePassword updatePassword)
    {
        try
        {
            result = await _bllAuthService.ResetPassword(updatePassword);
        }
        catch (Exception ex)
        {
            result.Status = ResponseStatus.Error;
            result.Message = ExceptionHelper.GetErrorMessage(ex);
        }
        ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
        return RedirectToAction(Constants.LOGIN_VIEW, Constants.LOGIN_CONTROLLER);
    }

    private static void SetAuthData(HttpResponse response, HttpContext context, string accessToken, string email, string userName)
    {
        // Set access token cookie
        CookieHelper.AppendCookie(response, Constants.ACCESS_TOKEN, accessToken, 15);

        // Set session values
        SessionHelper.SetString(context, Constants.SESSION_EMAIL, email);
        SessionHelper.SetString(context, Constants.SESSION_USERNAME, userName);
    }
}