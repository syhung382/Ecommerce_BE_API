

using System.ComponentModel.DataAnnotations;

namespace Ecommerce_BE_API.DbContext.Models.Requests
{
    public class MstUserFilter
    {
        public string? FullName { get; set; }
        public int? Status { get; set; }
        public string? TypeSort { get; set; }
        public bool? IsDesc { get; set; }
    }
    public class MstUserRegisterReq
    {

        [Required]
        public string Email { get; set; }

        public string? FullName { get; set; }


        [Required]
        public string UserName { get; set; } = null!;


        [Required]
        public string Password { get; set; } = null!;


        [Required]
        public int Gender { get; set; }
        public string? CodeInvite { get; set; }
    }

    public class LoginReq
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
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

    public class MstUserReq
    {
        public int? Id { get; set; }

        public string Email { get; set; } = null!;

        public string? FullName { get; set; }

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Avatar { get; set; }

        public int Gender { get; set; }

        public int Role { get; set; }

        public int? RoleAdmin { get; set; }
    }
}
