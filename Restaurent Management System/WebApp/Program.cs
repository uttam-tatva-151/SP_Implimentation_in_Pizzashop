using PMSWebApp.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices(configuration);
// builder.Services.AddSignalR();

WebApplication app = builder.Build();

// Configure middleware and error handling
if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithReExecute("/ErrorHandler/HttpStatusCodeHandler/{0}");
    app.UseExceptionHandler("/ErrorHandler/HttpStatusCodeHandler/500");
    app.UseHsts();
}
app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute( 
                    name: "default", 
                    pattern: "{controller=Login}/{action=Index}");


app.MapFallback(context =>
{
    string path = context.Request.Path.Value ?? string.Empty;

    // Avoid redirect loop if already on error page
    if (path != null && path.StartsWith("/ErrorHandler", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = 404;
        return Task.CompletedTask;
    }

    context.Response.Redirect("/ErrorHandler/HttpStatusCodeHandler/404");
    return Task.CompletedTask;
});

app.Run();