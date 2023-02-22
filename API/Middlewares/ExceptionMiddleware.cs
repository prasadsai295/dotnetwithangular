using API.Errors;

namespace API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next,
                                   ILogger<ExceptionMiddleware> logger,
                                   IHostEnvironment env
                                   )
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = _env.IsDevelopment() ? 
                    new ErrorResponseMessage(context.Response.StatusCode,ex.Message, ex.StackTrace.ToString())
                    :  new ErrorResponseMessage(context.Response.StatusCode,ex.Message, "Internal server error");

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}