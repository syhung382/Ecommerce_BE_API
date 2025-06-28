using System;
using System.Collections.Generic;

namespace Ecommerce_BE_API.DbContext.Models;

public partial class InfoProductTag
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid TagOfProductId { get; set; }

    public int Status { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }
}
