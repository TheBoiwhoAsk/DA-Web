namespace WebBanCa.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }         // thêm dấu hỏi
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }     // thêm dấu hỏi
    }

}
