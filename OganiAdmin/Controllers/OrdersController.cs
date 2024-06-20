using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OganiAdmin.Models;
using X.PagedList;

namespace OganiAdmin.Controllers
{
    public class OrdersController : Controller
    {
        private OganiContext data = new OganiContext();
        public IActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listOrder = data.Orders.Include(o=>o.Cus).Include(o=>o.Ship).Include(o=>o.Payment).AsNoTracking().OrderBy(x=>x.OrderId);
            PagedList<Order> list = new PagedList<Order>(listOrder, pageNumber, pageSize);
            return View(list);
        }
        [HttpPost]
        public IActionResult UpdateShipState(int shipId, string newState)
        {
            var shipment = data.Shipments.Find(shipId);

            if (shipment != null)
            {
                shipment.ShipState = newState;
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult ShowBestSeller(int topN = 3)
        {
            var bestSellers = data.Products.Include(o => o.Cate)
                                           .OrderByDescending(p => p.SellProduct)
                                           .Take(topN)
                                           .ToList();
            return View(bestSellers);
        }
        public IActionResult ShowPoorlySeller(int topN = 3)
        {
            var bestSellers = data.Products.Include(o => o.Cate)
                                           .OrderBy(p => p.SellProduct)
                                           .Take(topN)
                                           .ToList();
            return View(bestSellers);
        }
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var order = await data.Orders.Include(s => s.Ship)
                                  .Include(p => p.Payment)
                                  .Include(c => c.Cus)
                                  .FirstOrDefaultAsync(o => o.OrderId == orderId);
            var orderItems = await data.OrderItems.Include(p => p.Product)
                                                  .Where(o => o.OrderId == orderId)
                                                  .ToListAsync();
            ViewData["Order"] = order;
            ViewData["OrderItems"] = orderItems;
            return View();
        }
    }
}
