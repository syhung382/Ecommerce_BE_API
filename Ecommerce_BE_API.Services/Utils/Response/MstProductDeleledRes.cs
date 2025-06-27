using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.Services.Utils.Response
{
    public class MstProductDelRes
    {
        public List<Guid> DeletedProductIds { get; set; } = new();
        public List<Guid> NotFoundProductIds { get; set; } = new();
    }
}
