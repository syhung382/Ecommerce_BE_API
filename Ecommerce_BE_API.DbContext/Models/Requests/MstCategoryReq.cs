using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.DbContext.Models.Requests
{
    public partial class MstCategoryReq
    {
        public Guid? ParentId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Image { get; set; }


        [Required]
        public int Status { get; set; }
        public bool? DeleteFlag { get; set; }
    }

    public partial class MstCategoryFilter
    {
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? TypeSort { get; set; }
        public bool? IsDesc { get; set; }
    }
}
