namespace Ecommerce_BE_API.WebApi.Models.Response
{
    public class GlobalRes
    {
        public string Status { get; set; }
        public MstService Service { get; set; }
        public DateTime ServerTime { get; set; }
        public long Uptime { get; set; }
    }
}
