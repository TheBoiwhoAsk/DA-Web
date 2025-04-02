using System.Text.Json.Serialization;

namespace WebBanCa.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int ProductId { get; set; }

    }
}

