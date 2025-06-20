using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.Services.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(IConfiguration config, ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public async Task WriteErrorLogAsync(Exception exception, HttpRequest request = null, string functionName = "", object requestObject = null)
        {
            try
            {
                var endpointName = CreateHeaderMsg(request, functionName);
                var msg = exception.Message;
                if (requestObject != null && (exception is SqlException || msg.Contains("Timeout")))
                {
                    var contentString = JsonConvert.SerializeObject(requestObject, Formatting.Indented);
                    msg = $"{msg}, RequestInfo: {contentString}";
                }

                _logger.LogError(exception, $"Error at {endpointName}, msg: {msg}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at WriteErrorLogAsync");
            }
        }

        public void WriteErrorLog(Exception exception, HttpRequest request = null, string functionName = "")
        {
            try
            {
                var endpointName = CreateHeaderMsg(request, functionName);
                _logger.LogError(exception, $"Error at {endpointName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at WriteErrorLog");
            }
        }

        public void WriteInfoLog(string logMessage, HttpRequest request = null, string functionName = "")
        {
            try
            {
                var endpointName = CreateHeaderMsg(request, functionName);
                _logger.LogInformation($"Info at {endpointName}, msg: {logMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at WriteInfoLog");
            }
        }

        public void WriteWarningLog(string logMessage, HttpRequest request = null, string functionName = "")
        {
            try { 
                var endpointName = CreateHeaderMsg(request, functionName);
            _logger.LogWarning($"Warning at {endpointName}, msg: {logMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at WriteWarningLog");
            }
        }

        #region "Private method"

        private string CreateHeaderMsg(HttpRequest request = null, string functionName = "")
        {
            var endpointName = (request != null) ? request.Path.Value : string.Empty;
            if (!string.IsNullOrEmpty(functionName)) endpointName += $" - func: {functionName}";
            return endpointName;
        }
        #endregion "End Private method"
    }
}
