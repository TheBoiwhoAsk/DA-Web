using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Thêm chỉ thị này nếu chưa có
using WebBanCa.Models;
using System.Text.Json.Serialization; // Thêm chỉ thị này để tham chiếu đến lớp ProductImage

namespace WebBanCa.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Range(0.01, 1000000000.00)]
        public decimal Price { get; set; }
        public string Origin { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category?   Category { get; set; }
        public bool Status { get; set; }

        // Các thuộc tính hiện có
        public string? ImageUrl { get; set; } // Đường dẫn đến hình ảnh đại diện
        public List<ProductImage>? ProductImages { get; set; }

    }



}