using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSP.Models;

namespace OSP.Controllers
{
    public class DeliveryMethodsController : Controller
    {
        private readonly AppDbContext _context;

        public DeliveryMethodsController(AppDbContext context)
        {
            _context = context;
        }

        // Index: List all delivery methods
        public async Task<IActionResult> Index()
        {
            var deliveryMethods = await _context.DeliveryMethods.ToListAsync();
            return View(deliveryMethods);
        }

        // Create: Show the form
        public IActionResult Create()
        {
            return View();
        }

        // Create: Handle form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryMethod deliveryMethod)
        {
            if (ModelState.IsValid)
            {
                _context.DeliveryMethods.Add(deliveryMethod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deliveryMethod);
        }

        // Edit: Show the form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var deliveryMethod = await _context.DeliveryMethods.FindAsync(id);
            if (deliveryMethod == null) return NotFound();
            return View(deliveryMethod);
        }

        // Edit: Handle form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeliveryMethod deliveryMethod)
        {
            if (id != deliveryMethod.DeliveryMethodId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.DeliveryMethods.Update(deliveryMethod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DeliveryMethods.Any(e => e.DeliveryMethodId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(deliveryMethod);
        }

        // Delete: Confirm deletion
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var deliveryMethod = await _context.DeliveryMethods.FindAsync(id);
            if (deliveryMethod == null) return NotFound();
            return View(deliveryMethod);
        }

        // Delete: Handle deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deliveryMethod = await _context.DeliveryMethods.FindAsync(id);
            if (deliveryMethod != null)
            {
                _context.DeliveryMethods.Remove(deliveryMethod);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
