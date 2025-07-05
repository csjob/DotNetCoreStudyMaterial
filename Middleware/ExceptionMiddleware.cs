namespace DotNetCoreWebAPI.Middleware
{
    /// <summary>
    /// Custom middleware for handling errors
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next= next;
            _logger= logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Exception]: {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var error = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Oops! Something broke. Please try again later.",
                Detail = ex.Message
            };

            return context.Response.WriteAsJsonAsync(error);
        }

    }
}
