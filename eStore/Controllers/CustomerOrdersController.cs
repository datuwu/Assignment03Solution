using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using eStore.Helpers;
using System.Text;
using System.Text.Json;
using System.Net;

namespace eStore.Controllers
{
    public class CustomerOrdersController : Controller
    {
        private readonly EstoreContext _context;
        private readonly string OrderApiUrl = "";
        private readonly HttpClient client = null;

        public CustomerOrdersController(EstoreContext context)
        {
            client = new HttpClient();
            _context = context;
            OrderApiUrl = "https://localhost:7128/api/Orders";
        }

        // GET: CustomerOrders
        public async Task<IActionResult> Index()
        {
            return _context.Orders != null ?
                        View(await _context.Orders
              .Include(o => o.Member).ToListAsync()) :
                        Problem("Entity set 'EstoreContext.Orders'  is null.");
        }

        // GET: CustomerOrders/Details/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Product)
                .Include(o => o.Member)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Cart()
        {

            var order = SessionHelper.GetObjectFromJson<Order>(HttpContext.Session, "cart");
            if (order == null)
            {
                return Redirect("/CustomerProducts/Index");
            }

            return View(order);
        }

        public async Task<IActionResult> Order()
        {
            var order = SessionHelper.GetObjectFromJson<Order>(HttpContext.Session, "cart");
            foreach (var orderDetail in order.OrderDetails)
            {
                orderDetail.Product = null;
            }
            using StringContent jsonContent = new(
                        JsonSerializer.Serialize(order),
                        Encoding.UTF8,
                        "application/json");
            string json = await jsonContent.ReadAsStringAsync();
            HttpResponseMessage response = await client.PostAsync($"{OrderApiUrl}", jsonContent);

            if (response.StatusCode.Equals(HttpStatusCode.Created))
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Cart");
        }
        // GET: CustomerOrders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomerOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: CustomerOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: CustomerOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: CustomerOrders/Delete/5
        public async Task<IActionResult> Delete(int? orderId, int? productId)
        {
            var order = SessionHelper.GetObjectFromJson<Order>(HttpContext.Session, "cart");
            if (order == null)
            {
                return Redirect("/CustomerProducts/Index");
            }

            order.OrderDetails.Remove(order.OrderDetails.FirstOrDefault(o => o.ProductId == productId));
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", order);
            return Redirect("/CustomerOrders/Cart");
        }

        // POST: CustomerOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'EstoreContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
