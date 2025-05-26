using System.Security;
using Microsoft.AspNetCore.Mvc;

namespace PMSWebApp.Utilities
{
    public static class ExceptionHelper
    {
        public static IActionResult RedirectToErrorPage(Exception ex)
        {
            // Default to 500 Internal Server Error
            int statusCode = 500;
            string message = "An unexpected error occurred.";

            // Handle specific exception types
            if (ex is UnauthorizedAccessException)
            {
                statusCode = 401; // Unauthorized
                message = GetErrorMessage(ex);
            }
            else if (ex is ArgumentException)
            {
                statusCode = 400; // Bad Request
                message = GetErrorMessage(ex);
            }
            else if (ex is NotImplementedException)
            {
                statusCode = 501; // Not Implemented
                message = GetErrorMessage(ex);
            }
            else if (ex is KeyNotFoundException)
            {
                statusCode = 404; // Not Found
                message = GetErrorMessage(ex);
            }
            else if (ex is InvalidOperationException)
            {
                statusCode = 409; // Conflict
                message = GetErrorMessage(ex);
            }
            else if (ex is TimeoutException)
            {
                statusCode = 408; // Request Timeout
                message = GetErrorMessage(ex);
            }
            else if (ex is NullReferenceException)
            {
                statusCode = 500; // Internal Server Error
                message = GetErrorMessage(ex);
            }
            else if (ex is FormatException)
            {
                statusCode = 400; // Bad Request
                message = GetErrorMessage(ex);
            }
            else if (ex is DivideByZeroException)
            {
                statusCode = 500; // Internal Server Error
                message = GetErrorMessage(ex);
            }
            else if (ex is SecurityException)
            {
                statusCode = 403; // Forbidden
                message = GetErrorMessage(ex);
            }
            else if (ex is FileNotFoundException)
            {
                statusCode = 404; // Not Found
                message = GetErrorMessage(ex);
            }
            else if (ex is DirectoryNotFoundException)
            {
                statusCode = 404; // Not Found
                message = GetErrorMessage(ex);
            }
            else if (ex is StackOverflowException)
            {
                statusCode = 500; // Internal Server Error
                message = GetErrorMessage(ex);
            }
            else if (ex is OutOfMemoryException)
            {
                statusCode = 500; // Internal Server Error
                message = GetErrorMessage(ex);
            }

 
            return new RedirectToRouteResult(new { Controller = "ErrorHandler", Action = "HttpStatusCodeHandler", statusCode, message });
        }
        public static string GetErrorMessage(Exception ex)
        {

            string message = "An unexpected error occurred.";

            // Handle specific exception types
            if (ex is UnauthorizedAccessException)
            {
                message = "Access denied.";
            }
            else if (ex is ArgumentException)
            {
                message = "Invalid input provided.";
            }
            else if (ex is NotImplementedException)
            {
                message = "Feature not implemented.";
            }
            else if (ex is KeyNotFoundException)
            {
                message = "Resource not found.";
            }
            else if (ex is InvalidOperationException)
            {
                message = "Operation cannot be performed.";
            }
            else if (ex is TimeoutException)
            {
                message = "The request timed out.";
            }
            else if (ex is NullReferenceException)
            {
                message = "A null reference occurred.";
            }
            else if (ex is FormatException)
            {
                message = "Invalid format.";
            }
            else if (ex is DivideByZeroException)
            {
                message = "Division by zero error.";
            }
            else if (ex is SecurityException)
            {
                message = "Action is forbidden.";
            }
            else if (ex is FileNotFoundException)
            {
                message = "File not found.";
            }
            else if (ex is DirectoryNotFoundException)
            {
                message = "Directory not found.";
            }
            else if (ex is StackOverflowException)
            {
                message = "A stack overflow occurred.";
            }
            else if (ex is OutOfMemoryException)
            {
                message = "The server ran out of memory.";
            }

            return message;
        }

        public static string GetErrorMessage(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request. Please check your input.",
            401 => "Unauthorized. Please log in first.",
            403 => "Forbidden. You do not have permission to access this resource.",
            404 => "The requested resource was not found.",
            405 => "Method Not Allowed. Please check the request method.",
            408 => "Request Timeout. The server timed out waiting for the request.",
            409 => "Conflict. A conflict occurred with the current state of the resource.",
            415 => "Unsupported Media Type. The format is not supported.",
            422 => "Unprocessable Entity. The request was well-formed but was unable to be followed due to semantic errors.",
            429 => "Too Many Requests. You have sent too many requests in a given amount of time.",
            500 => "Internal Server Error. Something went wrong on the server.",
            501 => "Not Implemented. The server does not support the functionality required.",
            502 => "Bad Gateway. Invalid response from the upstream server.",
            503 => "Service Unavailable. The server is temporarily unavailable.",
            504 => "Gateway Timeout. The server did not receive a timely response.",
            _   => "An unexpected error occurred."
        };
    }

    }
}
