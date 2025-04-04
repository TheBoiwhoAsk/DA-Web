using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanCa.Extensions;
using WebBanCa.Models;
using WebBanCa.Repositories;

namespace WebBanCa.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NewUserModel> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<NewUserModel> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            ViewBag.CartCount = cart.Items.Sum(i => i.Quantity); // Thêm ViewBag.CartCount
            var model = new ShoppingCart
            {
                Items = cart.Items,
                IsUserLoggedIn = User.Identity.IsAuthenticated
            };
            return View(model);
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            if (!cart.Items.Any())
            {
                TempData["Message"] = "Your cart is empty. Please add items to your cart before checking out.";
                return RedirectToAction("Index");
            }
            ViewBag.CartCount = cart.Items.Sum(i => i.Quantity); // Thêm ViewBag.CartCount
            ViewBag.Cart = cart;
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                TempData["Message"] = "Your cart is empty. Please add items to your cart before checking out.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

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
            return View("OrderCompleted", order.Id);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var product = await GetProductFromDatabase(productId);
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            if (cart.Items.Any(i => i.ProductId == productId))
            {
                return Json(new { success = false, message = "Bạn đã thêm sản phẩm này vào giỏ hàng rồi!" });
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ImageUrl = product.ImageUrl ?? "/images/default.png"
            };

            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Json(new { success = true, cartCount = cart.Items.Sum(i => i.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity < 1)
            {
                return Json(new { success = false, message = "Quantity must be at least 1." });
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null)
            {
                return Json(new { success = false, message = "Cart is empty." });
            }

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                return Json(new { success = false, message = "Product not found in cart." });
            }

            item.Quantity = quantity;
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Json(new { success = true, cartCount = cart.Items.Sum(i => i.Quantity) });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                cart.RemoveItem(productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return Json(new { success = true, cartCount = cart?.Items.Sum(i => i.Quantity) ?? 0 });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return Json(new { success = true, cartCount = 0 });
        }

        private async Task<Product> GetProductFromDatabase(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception($"Product with ID {productId} not found.");
            }
            if (string.IsNullOrEmpty(product.ImageUrl))
            {
                product.ImageUrl = "/images/default.png";
            }
            return product;
        }
    }
}