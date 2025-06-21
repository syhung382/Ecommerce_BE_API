namespace Ecommerce_BE_API.WebApi.Models.Response
{
    public class UserLoginRes
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string? FullName { get; set; }

        public string UserName { get; set; } = null!;

        public string? Avatar { get; set; }

        public int Gender { get; set; }

        public int Role { get; set; }

        public int? RoleAdmin { get; set; }

        public int? InviteUserId { get; set; }

        public int? InviteUserCount { get; set; }

        public string? CodeInvite { get; set; }

        public int IsFirstLogin { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public string? CurrentSession { get; set; }
        public string Token { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
