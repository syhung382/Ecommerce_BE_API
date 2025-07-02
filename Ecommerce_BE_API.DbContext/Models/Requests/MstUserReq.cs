

using System.ComponentModel.DataAnnotations;

namespace Ecommerce_BE_API.DbContext.Models.Requests
{
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
    }

    public class LoginReq
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
