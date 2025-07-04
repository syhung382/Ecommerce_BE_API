using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.DbContext.Models.Requests
{
    public class ResponseService<T>
    {
        public int status { get; set; }
        public T? response { get; set; }
    }
}
