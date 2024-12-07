using System.Text;

namespace PrivatBank.Applicaion.Middlewares;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        httpContext.Request.EnableBuffering(); 

        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
        var request = httpContext.Request;
        string bodyContent = string.Empty;

        if (request.ContentLength > 0)
        {
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                bodyContent = await reader.ReadToEndAsync();
            }
        }

        var requestLog = $"Request: Method: {request.Method} Path: {request.Path} Query: {request.QueryString} ClientIp: {clientIp} Body: {bodyContent}";

        logger.LogInformation(requestLog);
        httpContext.Request.Body.Position = 0;

        await next(httpContext);
    }
}