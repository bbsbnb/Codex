using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    public AuditMiddleware(RequestDelegate next) { _next = next; }

    public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
    {
        var originalBody = context.Request.Body;
        var userId = context.User?.Identity?.Name ?? "0";

        using var memoryStream = new MemoryStream();
        if (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "DELETE")
        {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var path = context.Request.Path;
            if (body.Length < 2000)
            {
                await auditLogService.LogAsync(userId, context.Request.Method, path, null, body, context.Connection.RemoteIpAddress?.ToString() ?? "");
            }
            context.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(body));
        }

        await _next(context);
    }
}

public static class AuditMiddlewareExtensions
{
    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditMiddleware>();
    }
}