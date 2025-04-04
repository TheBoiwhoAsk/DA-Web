using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebBanCa.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public NewUserModel NewUserModel { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
