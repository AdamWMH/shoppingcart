using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WooliesXAPI.Logging;

namespace WooliesXAPI.Filters
{
    public static class MessageLoggingExtensions
    {
        public static IApplicationBuilder UseMessageLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MessageLoggingMiddleware>();
        }
    }

    public class MessageLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected async Task UnauthorizedMessageAsync(string correlationId, string bearerToken, string requestUrl, string message)
        {
            await Task.Run(() => _logger.Auth($"Log Identifier: {correlationId} - Token: {bearerToken} -  Url: {requestUrl} - Request:\r\n{message}"));
        }

        protected async Task IncommingMessageAsync(string correlationId, string bearerToken, string requestUrl, string message)
        {
            await Task.Run(() => _logger.Request($"Log Identifier: {correlationId} - Token: {bearerToken} -  Url: {requestUrl}{(string.IsNullOrWhiteSpace(message) ? "" : $" - Request:\r\n{message}")}"));
        }

        protected async Task OutgoingMessageAsync(string correlationId, string bearerToken, string requestUrl, string message)
        {
            await Task.Run(() => _logger.Response($"Log Identifier: {correlationId} - Token: {bearerToken} -  Url: {requestUrl} - Response:\r\n{message}"));
        }

        public MessageLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var messageId = Guid.NewGuid().ToString();

            var request = await FormatRequest(messageId, context.Request);

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                await _next(context);
                var response = await FormatResponse(messageId, context.Request, context.Response);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(string messageId, HttpRequest request)
        {
            var body = request.Body;
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body = body;

            await IncommingMessageAsync(messageId, request.Headers["Authorization"], $"{request.Host}{request.Path} {request.QueryString}", bodyAsText);

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(string messageId, HttpRequest request, HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            await OutgoingMessageAsync(messageId, request.Headers["Authorization"], $"{request.Host}{request.Path} {request.QueryString}", text);

            return $"{response.StatusCode}: {text}";
        }
    }
}
