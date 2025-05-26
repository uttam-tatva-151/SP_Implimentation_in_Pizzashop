using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PMSCore.Beans;

namespace PMSWebApp.Utilities
{
    public class ToasterHelper
    {
       public static void SetToastMessage(ITempDataDictionary tempData, string message, ResponseStatus status)
        {
            tempData["ToastMessage"] = message;
            tempData["ToastStatus"] = status.ToString(); // Convert Enum to String
        }
    }
}
