using System;
using System.Collections.Generic;

namespace Ecommerce_BE_API.DbContext.Models;

public partial class MstProduct
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

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }
}
