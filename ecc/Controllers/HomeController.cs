using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSP.Extensions;
using OSP.Models;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return RedirectToAction("PublicSection");
    }

    public IActionResult PublicSection(int? categoryId)
    {
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
        ViewBag.SelectedCategory = categoryId;

        var products = _context.Products.AsQueryable();
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        ViewBag.DeliveryMethods = _context.DeliveryMethods.ToList();

        return View(products.Include(p => p.Category).ToList());
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity, string deliveryOption)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
        if (product != null)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.DeliveryOption = deliveryOption;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl,
                    DeliveryOption = deliveryOption
                });
            }

            HttpContext.Session.SetObject("Cart", cart);
        }

        TempData["OrderAlert"] = "Product added to cart successfully!";
        return RedirectToAction("PublicSection");
    }

    public IActionResult Cart()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
        ViewBag.DeliveryMethods = _context.DeliveryMethods.ToList();
        return View(cart);
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
        var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);

        if (itemToRemove != null)
        {
            cart.Remove(itemToRemove);
            HttpContext.Session.SetObject("Cart", cart);
        }

        return RedirectToAction("Cart");
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(string DeliveryMethod, string DeliveryCountry, string DeliveryCity, string PostalCode)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
        if (cart != null && cart.Any())
        {
            var deliveryMethod = await _context.DeliveryMethods.FirstOrDefaultAsync(m => m.Name == DeliveryMethod);
            var deliveryCharge = deliveryMethod?.Charge ?? 0;

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                DeliveryMethod = DeliveryMethod,
                DeliveryCharge = deliveryCharge,
                DeliveryCountry = DeliveryCountry,
                DeliveryCity = DeliveryCity,
                PostalCode = PostalCode
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    DeliveryOption = item.DeliveryOption
                };

                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");

            TempData["OrderAlert"] = "Your order has been placed successfully!";
        }

        return RedirectToAction("PublicSection");
    }

    public IActionResult PrivateSection()
{
    if (HttpContext.Session.GetString("IsLoggedIn") != "true")
    {
        return RedirectToAction("Login"); // Redirect to login if not authenticated
    }

    return View();
}


    [HttpGet]
public IActionResult Login()
{
    return View();
}

[HttpPost]
public IActionResult Login(string username, string password)
{
    // Hardcoded credentials for simplicity (replace with a database check in production)
    if (username == "admin" && password == "password123")
    {
        HttpContext.Session.SetString("IsLoggedIn", "true"); // Store login status in session
        return RedirectToAction("PrivateSection");
    }

    ViewBag.Error = "Invalid username or password.";
    return View();
}

// Logout Action
public IActionResult Logout()
{
    HttpContext.Session.Remove("IsLoggedIn");
    return RedirectToAction("Login");
}

}
