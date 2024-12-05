namespace PrivatBank.Applicaion.Middlewares;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
        var request = httpContext.Request;
        var requestLog = $"Request: {request.Method} {request.Path} {request.QueryString} {clientIp}";

        logger.LogInformation(requestLog);

        // TODO: add request body.

        await next(httpContext);

        var response = httpContext.Response;
        var responseLog = $"Response: {response.StatusCode}";
        logger.LogInformation(responseLog);
    }
}