using System;
using System.Collections.Generic;

namespace Ecommerce_BE_API.DbContext.Models;

public partial class MstUser
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Avatar { get; set; }

    public int IsActived { get; set; }

    public int Gender { get; set; }

    public int Role { get; set; }

    public int? RoleAdmin { get; set; }

    public int? InviteUserId { get; set; }

    public int? InviteUserCount { get; set; }

    public string? CodeInvite { get; set; }

    public int IsFirstLogin { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public bool DeleteFlag { get; set; }

    public int IsBanned { get; set; }

    public string? CurrentSession { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }
}
