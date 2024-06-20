using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OganiAdmin.Models;
using X.PagedList;

namespace OganiAdmin.Controllers
{
    public class PaymentsController : Controller
    {
       
        public OganiContext data = new OganiContext();

        [Route("Payment")]
        public  IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1: page.Value;
            var listPayment = data.Payments.Include(c => c.Cus).AsNoTracking().OrderBy(x => x.PaymentId);
            PagedList<Payment> list = new PagedList<Payment>(listPayment, pageNumber, pageSize);
            return View( list);
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await data.Payments
                .Include(p => p.Cus)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            ViewData["CusId"] = new SelectList(data.Customers, "CusId", "CusId");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,PaymentDate,PaymentMethod,PaymentAmount,CusId")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                data.Add(payment);
                await data.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CusId"] = new SelectList(data.Customers, "CusId", "CusId", payment.CusId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await data.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["CusId"] = new SelectList(data.Customers, "CusId", "CusId", payment.CusId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,PaymentDate,PaymentMethod,PaymentAmount,CusId")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    data.Update(payment);
                    await data.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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
            ViewData["CusId"] = new SelectList(data.Customers, "CusId", "CusId", payment.CusId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        [HttpGet]
        public  IActionResult Delete(int? id)
        {
            TempData["Message"] = "";
            var payment = data.Payments.Find(id);
            if (payment == null)
            {
                TempData["Message"] = "Delete failed! No payment found.";
                return RedirectToAction("Index");
            }
            try
            {
                data.Payments.Remove(payment);
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

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await data.Payments.FindAsync(id);
            if (payment != null)
            {
                data.Payments.Remove(payment);
            }

            await data.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return data.Payments.Any(e => e.PaymentId == id);
        }
    }
}
