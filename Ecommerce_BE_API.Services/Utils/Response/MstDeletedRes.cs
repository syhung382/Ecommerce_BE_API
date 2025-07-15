using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.Services.Utils.Response
{
    public class MstDeletedRes
    {
        public List<Guid> DeletedIds { get; set; } = new();
        public List<Guid> NotFoundIds { get; set; } = new();
    }
    public class MstDeletedIntRes
    {
        public List<int> DeletedIds { get; set; } = new();
        public List<int> NotFoundIds { get; set; } = new();
    }
}
