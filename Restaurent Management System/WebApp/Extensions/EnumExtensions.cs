using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PMSWebApp.Extensions
{
    public static class EnumExtensions
    {
        /* 
            * This method converts an enum type to a list of SelectListItem objects,
            * which can be used to populate dropdown lists in ASP.NET MVC views.
        */
        public static List<SelectListItem> ToSelectList<TEnum>(this TEnum enumType) where TEnum : Enum
        {
            /*
                * Get all the values of the enum type and cast them to TEnum.
            */
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            /*
                * Convert each enum value to a SelectListItem object.
                * The Value property is the string representation of the enum value.
                * The Text property is the display name of the enum value, which is obtained using the GetDisplayName method.
            */
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new SelectListItem
                       {
                           Value = e.ToString(),
                           Text = e.GetDisplayName()
                       }).ToList();
        }

        public static string GetDisplayName(this Enum enumValue)
            {
                DisplayAttribute? attribute = enumValue.GetType()
                                         .GetField(enumValue.ToString())
                                         ?.GetCustomAttribute<DisplayAttribute>();
                return attribute?.Name ?? enumValue.ToString();
            }
    }
}
