
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_BE_API.DbContext.Models.Requests
{

    public class MstProductReq
    {

        public Guid? Id { get; set; }


        [Required]
        public Guid CategoryId { get; set; }

        public Guid? DiscountId { get; set; }


        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Detail { get; set; }

        public string? Image { get; set; }


        [Required]
        public int Price { get; set; }

        public int? PriceSale { get; set; }


        [Required]
        public int Status { get; set; }

        public bool DeleteFlag { get; set; }
        public List<InfoProductTagRes>? ListTagRes { get; set; }
        public List<InfoProductUpdateImageReq>? listProductImage {  get; set; }
    }

    public class MstProductRes
    {
        public Guid Id { get; set; }

        public MstCategory? Category { get; set; }

        public Guid? DiscountId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Detail { get; set; }

        public string? Image { get; set; }

        public int Price { get; set; }

        public int? PriceSale { get; set; }

        public int Status { get; set; }

        public bool DeleteFlag { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
        public List<InfoProductTagRes>? ListProductTag { get; set; }
        public List<InfoProductImage>? listProductImage { get; set; }
    }

    public class MstProductFilter
    {
        public Guid? CategoryId { get; set; }
        public string? Title { get; set; }
        public int? StartPrice { get; set; }
        public int? EndPrice { get; set; }
        public int? Status { get; set; }
        public string? TypeSort { get; set; }
        public bool? IsDesc { get; set; }
    }
    
}
