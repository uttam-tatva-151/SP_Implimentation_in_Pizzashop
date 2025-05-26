using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PMSCore.Beans;
using PMSCore.DTOs;
using PMSServices.Interfaces;
using PMSWebApp.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PMSWebApp.Extensions
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtConfig _jwtConfig;

        /* 
            * Middleware is instantiated once per application lifetime, 
            * but scoped services are created per request. 
            * Since IJWTService is scoped, it cannot be directly injected into the middleware constructor.*/

        public JwtMiddleware(RequestDelegate next, IOptions<JwtConfig> jwtConfig)
        {
            _next = next;
            _jwtConfig = jwtConfig.Value;
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                IJWTService jwtService = context.RequestServices.GetRequiredService<IJWTService>();
                string token = context.Request.Cookies[Constants.ACCESS_TOKEN] ?? string.Empty;
                string refreshToken = context.Request.Cookies[Constants.REFRESH_TOKEN] ?? string.Empty;

                if (!string.IsNullOrEmpty(token))
                {
                    ClaimsPrincipal principal = ValidateToken(token) ?? throw new SecurityTokenException(Constants.INVALID_ACCESS_TOKEN);

                    context.User = principal;
                }
                else if (!string.IsNullOrEmpty(refreshToken))
                {
                    bool isRefreshTokenValid = await jwtService.ValidateRefreshTokenAsync(refreshToken);
                    if (isRefreshTokenValid)
                    {
                        string newAccessToken = await jwtService.RefreshTokenAsync(refreshToken);

                        if (!string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(newAccessToken))
                        {
                            CookieHelper.AppendCookie(context.Response, Constants.ACCESS_TOKEN, newAccessToken, 15); // Access token expires in 15 minutes
                            CookieHelper.AppendCookie(context.Response, Constants.REFRESH_TOKEN, refreshToken, 30 * 24 * 60); // Refresh token expires in 30 days

                            ClaimsPrincipal principal = ValidateToken(newAccessToken) ?? throw new SecurityTokenException(Constants.INVALID_ACCESS_TOKEN);
                            if (principal != null)
                            {
                                context.User = principal;
                            }
                        }
                    }
                    else
                    {
                        
                        context.Response.Cookies.Delete(Constants.ACCESS_TOKEN);
                        context.Response.Cookies.Delete(Constants.REFRESH_TOKEN);
                        context.Response.Redirect("/" + Constants.LOGIN_CONTROLLER + "/" + Constants.LOGIN_VIEW);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                
                context.Response.Cookies.Delete(Constants.ACCESS_TOKEN);
                context.Response.Cookies.Delete(Constants.REFRESH_TOKEN);

                
                int statusCode = ex is SecurityTokenException ? StatusCodes.Status401Unauthorized : StatusCodes.Status500InternalServerError;
                string message = ex is SecurityTokenException ? Constants.INVALID_ACCESS_TOKEN : string.Empty;

                
                context.Response.Redirect($"/" + Constants.ERROR_CONTROLLER + "/" + Constants.ERROR_VIEW + "/"+statusCode+"/" + message);
                return;
            }

            await _next(context);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.UTF8.GetBytes(_jwtConfig.Key ?? string.Empty);

            try
            {
                TokenValidationParameters validationParameters = new()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidIssuer = _jwtConfig.Issuer,
                    ValidateAudience = false,
                    ValidAudience = _jwtConfig.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Strict expiration check
                };

                
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null ?? new ClaimsPrincipal();
            }
        }
    }
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            Claim? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Convert.ToInt32(userIdClaim.Value) : 0;
        }
    }
}
