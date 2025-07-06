using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.DbContext.Models.Requests
{
    internal class MstUserFilter
    {
    }

    public class MstUserRes
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string? FullName { get; set; }

        public string UserName { get; set; } = null!;

        public string? Avatar { get; set; }

        public int Gender { get; set; }
    }
}

