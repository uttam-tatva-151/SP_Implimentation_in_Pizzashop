using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{
    public class ErrorHandlerController : Controller
    {
        [Route("ErrorHandler/HttpStatusCodeHandler/{statusCode:int?}/{message?}")]
        public IActionResult HttpStatusCodeHandler(int statusCode, string? message)
        {
            int finalStatusCode = statusCode > 0 ? statusCode : 500;
            Response.StatusCode = finalStatusCode;

            ViewData["StatusCode"] = finalStatusCode;
            ViewData["Message"] = string.IsNullOrWhiteSpace(message)
                ? ExceptionHelper.GetErrorMessage(finalStatusCode)
                : message;

            TempData[Constants.LAYOUT_VARIABLE_NAME] = string.Empty;

            return View("Error");
        }
    }
}