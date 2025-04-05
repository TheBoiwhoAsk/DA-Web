using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanCa.Extensions;
using WebBanCa.Models;
using WebBanCa.Repositories;

namespace WebBanCa.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShoppingCartApiController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NewUserModel> _userManager;

        public ShoppingCartApiController(ApplicationDbContext context, UserManager<NewUserModel> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                return NotFound(new { success = false, message = "Product not found." });

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            if (cart.Items.Any(i => i.ProductId == item.ProductId))
                return BadRequest(new { success = false, message = "Product already in cart." });

            cart.AddItem(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = item.Quantity,
                ImageUrl = product.ImageUrl ?? "/images/default.png"
            });

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Ok(new { success = true, cartCount = cart.Items.Sum(i => i.Quantity) });
        }

        [HttpPut("update")]
        public IActionResult UpdateQuantity([FromBody] CartItem item)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null)
                return NotFound(new { success = false, message = "Cart not found." });

            var product = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (product == null)
                return NotFound(new { success = false, message = "Product not in cart." });

            product.Quantity = item.Quantity;
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Ok(new { success = true, cartCount = cart.Items.Sum(i => i.Quantity) });
        }

        [HttpDelete("remove/{productId}")]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null)
                return NotFound(new { success = false, message = "Cart not found." });

            cart.RemoveItem(productId);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Ok(new { success = true, cartCount = cart.Items.Sum(i => i.Quantity) });
        }

        [HttpDelete("clear")]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return Ok(new { success = true });
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
                return BadRequest(new { success = false, message = "Cart is empty." });

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");

            return Ok(new { success = true, orderId = order.Id });
        }
    }
}
