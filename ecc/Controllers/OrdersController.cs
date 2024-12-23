using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSP.Models;

public class OrdersController : Controller
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

   public async Task<IActionResult> Index()
{
    var orders = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ToListAsync();
    return View(orders);
}


public IActionResult Create()
{
    ViewBag.Products = _context.Products.ToList(); // Pass products for dropdown
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("OrderId,OrderDate")] Order order, List<int> ProductIds, string Quantities)
{
    if (ModelState.IsValid)
    {
        // Convert OrderDate to UTC
        order.OrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);

        var quantityList = Quantities.Split(',').Select(int.Parse).ToList();

        for (int i = 0; i < ProductIds.Count; i++)
        {
            var orderDetail = new OrderDetail
            {
                Order = order,
                ProductId = ProductIds[i],
                Quantity = quantityList[i]
            };
            _context.OrderDetails.Add(orderDetail);
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    ViewBag.Products = _context.Products.ToList();
    return View(order);
}


    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(m => m.OrderId == id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
public IActionResult Delete(int id)
{
    var order = _context.Orders
        .Include(o => o.OrderDetails)
        .ThenInclude(d => d.Product)
        .FirstOrDefault(o => o.OrderId == id);

    if (order == null)
    {
        return NotFound();
    }

    return View(order);
}

[HttpPost, ActionName("Delete")]
public IActionResult DeleteConfirmed(int id)
{
    var order = _context.Orders
        .Include(o => o.OrderDetails)
        .FirstOrDefault(o => o.OrderId == id);

    if (order != null)
    {
        // Remove related order details first
        _context.OrderDetails.RemoveRange(order.OrderDetails);
        _context.Orders.Remove(order);
        _context.SaveChanges();
    }

    return RedirectToAction("Index");
}

}
