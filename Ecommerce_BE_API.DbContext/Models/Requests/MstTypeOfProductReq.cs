using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.DbContext.Models.Requests
{
    public class MstTypeOfProductReq
    {

        public string Title { get; set; } = null!;

        public int Status { get; set; }
        public bool? DeleteFlag { get; set; }
    }

    public class MstTypeOfProductFilter
    {
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? TypeSort { get; set; }
        public bool? IsDesc { get; set; }
    }
}
