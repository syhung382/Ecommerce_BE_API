using Ecommerce_BE_API.DbContext.Common;
using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.Services.Interfaces;
using Ecommerce_BE_API.Services.Logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.Services.Implements
{
    public class GlobalService : IGlobalService
    {
        private readonly ILoggerService _logger;
        private readonly IGenericDbContext<Ecommerce_BE_APIContext> _unitOfWork;

        public GlobalService(ILoggerService logger, IGenericDbContext<Ecommerce_BE_APIContext> unitOfWork, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public Task<bool> CheckConnection()
        {
            var connection = _unitOfWork.Database.CanConnectAsync();

            return connection;
        }
    }
}
