using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.DbContext.Models.Requests
{
    public class InfoProductUpdateImageReq
    {
        public Guid? Id { get; set; }

        public string ImageUrl { get; set; } = null!;
    }
}
