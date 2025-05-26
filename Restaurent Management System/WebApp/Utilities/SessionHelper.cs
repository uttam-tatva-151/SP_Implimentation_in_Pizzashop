namespace PMSWebApp.Utilities
{
    public static class SessionHelper
    {

        public static void SetString(HttpContext httpContext, string key, string value)
        {
            httpContext.Session.SetString(key, value);
        }

        public static string? GetString(HttpContext httpContext, string key)
        {
            return httpContext.Session.GetString(key);
        }
        public static void Remove(HttpContext httpContext, string key)
        {
            httpContext.Session.Remove(key);
        }
        public static bool Contains(HttpContext httpContext, string key)
        {
            return httpContext.Session.GetString(key) != null;
        }
    }
}