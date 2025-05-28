using Microsoft.Extensions.Options;
using PMSCore.Beans;
using PMSCore.DTOs.Configuration;
using PMSWebApp.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices(configuration);

WebApplication app = builder.Build();

// Configure middleware and error handling
if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithReExecute(Constants.ERROR_HANDLER_HTTP_STATUS_CODE_HANDLER_ROUTE);
    app.UseExceptionHandler(Constants.ERROR_HANDLER_HTTP_STATUS_CODE_500_ROUTE);
    app.UseHsts();
}
app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Bind RouteSettings from configuration in service collection extension and Retrieve the strongly-typed settings here
RouteSettings routeSettings = app.Services.GetRequiredService<IOptions<RouteSettings>>().Value;
app.MapControllerRoute(name: routeSettings.DefaultRouteName,
                        pattern: routeSettings.DefaultRoutePattern);

app.MapFallback(context =>
{
    string path = context.Request.Path.Value ?? string.Empty;

    // Avoid redirect loop if already on error page
    if (path != null && path.StartsWith(Constants.ERROR_HANDLER_ROUTE, StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = 404;
        return Task.CompletedTask;
    }
    context.Response.Redirect(Constants.ERROR_HANDLER_HTTP_STATUS_CODE_404_ROUTE);
    return Task.CompletedTask;
});

app.Run();