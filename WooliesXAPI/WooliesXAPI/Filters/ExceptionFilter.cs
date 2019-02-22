using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WooliesXAPI.Filters
{
    /// <summary>
    /// The Exception filter.
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The on exception event.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void OnException(ExceptionContext context)
        {
            try
            {
                var controllerName = context.ActionDescriptor.Id;
                var actionName = context.ActionDescriptor.DisplayName;
                var controllerAction = string.Format("Controller: {0} with Action: {1} ", controllerName, actionName);
                var logId = Guid.NewGuid().ToString();

                if (context.Exception is NotImplementedException)
                {
                    var result = SetResponse(null, false, HttpStatusCode.NotImplemented, logId, "", "Error processing request");
                    context.Result = result;
                }
                else if (context.Exception is AggregateException)
                {

                    var aggregateException = (AggregateException)context.Exception;
                    var errorMessages = new StringBuilder();

                    foreach (var exception in aggregateException.InnerExceptions)
                    {
                        errorMessages.AppendLine(string.Format("Error: {0}", exception.Message));
                    }

                    logger.Error(string.Format("Log Identifier: {0} - Error in {1} with action {2}", logId, controllerName, controllerAction), context.Exception);
                    var result = SetResponse(null, false, HttpStatusCode.BadRequest, logId, errorMessages.ToString(), "Error processing request");
                    context.Result = result;
                }
                else if (context.Exception is Exception)
                {
                    logger.Error(string.Format("Log Identifier: {0} - Error in {1} with action {2}", logId, controllerName, controllerAction), context.Exception);
                    var result = SetResponse(null, false, HttpStatusCode.BadRequest, logId, context.Exception.Message, "Error processing request");
                    context.Result = result;
                }
            }
            catch (Exception ex)
            {
                var result = SetResponse(null, false, HttpStatusCode.BadRequest, Guid.NewGuid().ToString(), ex.Message, "Error processing request");
                context.Result = result;
            }

            base.OnException(context);
        }

        /// <summary>
        /// Sets the response.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="success">
        /// Sets if the response is a success.
        /// </param>
        /// <param name="statusCode">
        /// Sets the status code.
        /// </param>
        /// <param name="traceId">
        /// Sets the trace id.
        /// </param>
        /// <param name="errorMessage">
        /// Sets the error message.
        /// </param>
        /// <param name="displayError">
        /// Sets the display error.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        internal JsonResult SetResponse(object model, bool success, HttpStatusCode statusCode, string traceId = "", string errorMessage = "", string displayError = "")
        {
            var response = new
            {
                Data = model,
                Success = success,
                StatusCode = statusCode,
                ErrorMessage = errorMessage,
                DisplayError = displayError,
                TraceId = !string.IsNullOrEmpty(traceId) ? new Guid(traceId) : Guid.Empty
            };

            return new JsonResult(response);
        }
    }
}
