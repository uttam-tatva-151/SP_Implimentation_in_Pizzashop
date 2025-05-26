using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace PMSWebApp.Extensions
{
    public static class ControllerHelperExtensions
    {
        /* 
            * Renders a partial view into a string.
            * This is useful when returning both a partial view and additional data (like pagination) in an AJAX request.
            * Extension methods for rendering a partial view as a string.
            * This allows us to return both HTML (partial view) and data (pagination, JSON, etc.) from a controller.
            * <returns>A string containing the rendered HTML of the partial view.</returns>
       */
        public static async Task<string> RenderPartialViewToString(this Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            StringWriter stringWriter = new();

            // Retrieve the view engine service from the dependency injection container
            ICompositeViewEngine viewEngine = controller.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();

            // Find the specified view within the current controller's context
            ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

            // If the view is not found, throw an exception to indicate the issue
            if (!viewResult.Success)
            {
                throw new FileNotFoundException($"View '{viewName}' not found.");
            }

            // Creating a ViewContexts for rendering the view
            ViewContext viewContext = new(
                controller.ControllerContext, // Current controller's execution context
                viewResult.View,              // The resolved view
                controller.ViewData,          // Model and data for the view
                controller.TempData,          // Temporary data storage (persists across requests)
                stringWriter,                 // Output destination for rendered HTML
                new HtmlHelperOptions()       // Helper options (default configuration)
            );


            // Render the view asynchronously into the StringWriter
            await viewResult.View.RenderAsync(viewContext);
            return stringWriter.ToString();

        }
    }
}
