

namespace Ecommerce_BE_API.DbContext.Models.Requests
{
    public class MstUserRegisterReq
    {
        public string Email { get; set; }

        public string? FullName { get; set; }

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int Gender { get; set; }
    }

    public class LoginReq
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
