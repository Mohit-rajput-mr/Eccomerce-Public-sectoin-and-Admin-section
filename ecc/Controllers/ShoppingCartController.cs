using Microsoft.AspNetCore.Mvc;
using OSP.Models;
using OSP.Extensions;


public class ShoppingCartController : Controller
{
    private readonly AppDbContext _context;

    public ShoppingCartController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<List<int>>("Cart") ?? new List<int>();
        var products = _context.Products.Where(p => cart.Contains(p.ProductId)).ToList();
        return View(products);
    }

    public IActionResult AddToCart(int id)
    {
        var cart = HttpContext.Session.GetObject<List<int>>("Cart") ?? new List<int>();
        if (!cart.Contains(id)) cart.Add(id);

        HttpContext.Session.SetObject("Cart", cart);
        return RedirectToAction("Index", "ShoppingCart");
    }
}
