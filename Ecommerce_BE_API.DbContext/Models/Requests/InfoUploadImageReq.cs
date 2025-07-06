
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_BE_API.DbContext.Models.Requests
{
    public class InfoUploadImageReq
    {
        [Required]
        public IFormFile File { get; set; }
    }

    public class InfoImageFilter
    {
        public int? UserId { get; set; }
        public string? TypeSort { get; set; }
        public bool? IsDesc { get; set; }
    }

    public class InfoImageReq
    {
        [Required]
        public string ImageUrl { get; set; } = null!;
    }
    public class ImageRes
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
    }
    public class ImageReq
    {
        public Guid? Id { get; set; }
        public string ImageUrl { get; set; }
    }
}
