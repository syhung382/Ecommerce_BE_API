
namespace Ecommerce_BE_API.DbContext.Models.Requests
{
    public class MstProductReq
    {
        public Guid CategoryId { get; set; }

        public Guid? DiscountId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Detail { get; set; }

        public string? Image { get; set; }

        public int Price { get; set; }

        public int? PriceSale { get; set; }

        public int Status { get; set; }
        public bool? DeleteFlag { get; set; }
        public List<Guid>? ListTagId { get; set; }
    }

    public class MstProductUpdateReq
    {
        public Guid Id { get; set; }

        public Guid CategoryId { get; set; }

        public Guid? DiscountId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Detail { get; set; }

        public string? Image { get; set; }

        public int Price { get; set; }

        public int? PriceSale { get; set; }

        public int Status { get; set; }

        public bool DeleteFlag { get; set; }
        public List<Guid>? ListTagId { get; set; }
    }
    public class MstProductRes
    {
        public Guid Id { get; set; }

        public Guid CategoryId { get; set; }

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
        public List<InfoProductTagRes>? ListTagRes { get; set; }
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
