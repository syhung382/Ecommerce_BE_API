using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.Services.Logger
{
    public interface ILoggerService
    {
        Task WriteErrorLogAsync(Exception exception, HttpRequest request = null, string functionName = "", object requestObject = null);
        void WriteErrorLog(Exception exception, HttpRequest request = null, string functionName = "");
        void WriteInfoLog(string logMessage, HttpRequest request = null, string functionName = "");
        void WriteWarningLog(string logMessage, HttpRequest request = null, string functionName = "");
    }
}
