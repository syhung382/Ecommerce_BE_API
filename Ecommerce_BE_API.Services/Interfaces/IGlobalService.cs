using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.Services.Interfaces
{
    public interface IGlobalService
    {
        Task<bool> CheckConnection();
    }
}
