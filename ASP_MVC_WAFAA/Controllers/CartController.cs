using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Add this line
using ASP_MVC_WAFAA.Models;
using ASP_MVC_WAFAA.Data;
namespace ASP_MVC_WAFAA.Controllers
{
    // CartController.cs
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;

    public class CartController : Controller
    {
        private readonly ASP_MVC_WAFAAContext _context;

        public CartController(ASP_MVC_WAFAAContext context)
        {
            _context = context;
        }
        public IActionResult Cart()
        {
            var cart = HttpContext.Session.Get<Cart>("Cart") ?? new Cart();
            return View(cart.Items);
        }

        [HttpPost]
        public IActionResult AddToCart(int potteryId)
        {
            var pottery = _context.Pottery.Find(potteryId);

            if (pottery != null)
            {
                var cart = HttpContext.Session.Get<Cart>("Cart") ?? new Cart();
                cart.Items.Add(new CartItem
                {
                    PotteryId = pottery.Id,
                    PotteryTitle = pottery.Title,
                    PotteryPrice = pottery.Price,
                    Quantity = 1 // You can set the default quantity as needed
                });

                HttpContext.Session.Set("Cart", cart);

                return RedirectToAction("Index", "Potteries");
            }

            return NotFound();
        }
        // Display the shopping cart

        // Other cart-related actions (e.g., remove items, update quantities) can be added here
    }

}
