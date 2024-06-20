using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OganiAdmin.Models;
using X.PagedList;

namespace OganiAdmin.Controllers
{
    public class ShipmentsController : Controller
    {
        

        public OganiContext data = new OganiContext();

        // GET: Shipments
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listShipment = data.Shipments.Include(c=>c.Cus).AsNoTracking().OrderBy(x => x.ShipId);
            PagedList<Shipment> list = new PagedList<Shipment>(listShipment, pageNumber, pageSize);
            return View(list);
        }

        // GET: Shipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await data.Shipments
                .Include(s => s.Cus)
                .FirstOrDefaultAsync(m => m.ShipId == id);
            if (shipment == null)
            {
                return NotFound();
            }

            return View(shipment);
        }

        // GET: Shipments/Create
        public IActionResult Create()
        {
            ViewData["CusId"] = new SelectList(data.Customers, "CusId", "CusId");
            return View();
        }

        // POST: Shipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShipId,ShipDate,ShipAddress,ShipPhone,ShipNote,ShipPrice,ShipMethod,ShipState,ShipCode,CusId")] Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                data.Add(shipment);
                await data.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CusId"] = new SelectList(data.Customers, "CusId", "CusId", shipment.CusId);
            return View(shipment);
        }

        // GET: Shipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await data.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }
            ViewData["CusId"] = new SelectList(data.Customers, "CusId", "CusId", shipment.CusId);
            return View(shipment);
        }

        // POST: Shipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShipId,ShipDate,ShipAddress,ShipPhone,ShipNote,ShipPrice,ShipMethod,ShipState,ShipCode,CusId")] Shipment shipment)
        {
            if (id != shipment.ShipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    data.Update(shipment);
                    await data.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipmentExists(shipment.ShipId))
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
            ViewData["CusId"] = new SelectList(data.Customers, "CusId", "CusId", shipment.CusId);
            return View(shipment);
        }

        // GET: Shipments/Delete/5
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            TempData["Message"] = "";
            var shipment = data.Shipments.Find(id);
            if (shipment == null)
            {
                TempData["Message"] = "Delete failed! No shipment found.";
                return RedirectToAction("Index");
            }
            try
            {
                data.Shipments.Remove(shipment);
                data.SaveChanges();
                TempData["Message"] = "Delete Successfully!";
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    TempData["Message"] = "This payment cannot be deleted because there are related products.";
                }
                else
                {
                    TempData["Message"] = "Delete failed! Error! An error occurred. Please try again later.";
                }
            }

            return RedirectToAction("Index");
        }

        // POST: Shipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipment = await data.Shipments.FindAsync(id);
            if (shipment != null)
            {
                data.Shipments.Remove(shipment);
            }

            await data.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipmentExists(int id)
        {
            return data.Shipments.Any(e => e.ShipId == id);
        }
    }
}
