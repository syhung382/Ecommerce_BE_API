
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
    }

    public class MstProductFilter
    {
        public Guid? CategoryId { get; set; }
        public string? Title { get; set; }
        public int? StartPrice { get; set; }
        public int? EndPrice { get; set; }
        public string? TypeSort { get; set; }
        public bool? IsDesc { get; set; }
    }
    
}
