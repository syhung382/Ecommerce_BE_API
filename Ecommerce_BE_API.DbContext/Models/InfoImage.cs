using System;
using System.Collections.Generic;

namespace Ecommerce_BE_API.DbContext.Models;

public partial class InfoImage
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool DeleteFlag { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }
}
