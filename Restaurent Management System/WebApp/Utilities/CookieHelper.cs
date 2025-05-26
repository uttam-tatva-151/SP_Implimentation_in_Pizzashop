namespace PMSWebApp.Utilities
{
    public class CookieHelper
    {
        public static void AppendCookie(HttpResponse response, string key, string value, int expirationMinutes)
        {
            response.Cookies.Append(key, value, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
            });
        }
        public static string? GetCookieValue(HttpRequest request, string cookieName)
        {
            return request.Cookies.TryGetValue(cookieName, out var cookieValue) ? cookieValue : null;
        }
    }
}
