using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.ProjectModel;
using OganiAdmin.Models;
using OganiAdmin.Models.Authentication;
using System.Diagnostics;
using X.PagedList;


namespace OganiAdmin.Controllers
{
    public class CustomerController: Controller
    {

        OganiContext data = new OganiContext();
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }
        public ActionResult SearchForm(string strSearch, int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listCustomer = data.Customers.AsNoTracking();

            if (!string.IsNullOrEmpty(strSearch))
            {
                listCustomer = listCustomer.Where(x => x.CusName.Contains(strSearch));
            }
            var orderedListCustomer = listCustomer.OrderBy(x => x.CusName);

            var pagedCustomers = orderedListCustomer.ToPagedList(pageNumber, pageSize);

            return View("Customers", pagedCustomers); 
        }

        [Authentication]
        [Route("Customers")]
        public IActionResult Customers(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listCustomer = data.Customers.AsNoTracking().OrderBy(x => x.CusId);
            PagedList<Customer> list = new PagedList<Customer>(listCustomer, pageNumber, pageSize);
            return View(list);
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("InsertCustomer")]
        [HttpGet]
        public IActionResult InsertCustomer()
        {
            return View();
        }
        [Route("InsertCustomer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                data.Customers.Add(customer);
                data.SaveChanges();
                return RedirectToAction("Customers");
            }
            return View(customer);
        }
        [Route("EditCustomer")]
        [HttpGet]
        public IActionResult EditCustomer(int CusId)
        {
            var customer = data.Customers.Find(CusId);
            return View(customer);
        }
        [Route("EditCustomer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(Customer customer)
        {
            if (ModelState.IsValid)
            {
                data.Entry(customer).State = EntityState.Modified;
                data.SaveChanges();
                return RedirectToAction("Customers", "Customer");
            }
            return View(customer);
        }
        [HttpGet]
        public IActionResult DeleteCustomer(int CusId)
        {
            TempData["Message"] = "";
            var customer = data.Customers.Find(CusId);
            if (customer == null)
            {
                TempData["Message"] = "Delete failed! No customer found.";
                return RedirectToAction("Customers");
            }
            try
            {
                data.Customers.Remove(customer);
                data.SaveChanges();
                TempData["Message"] = "Delete Successfully!";
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    TempData["Message"] = "This customer cannot be deleted because there are related orders.";
                }
                else
                {
                    TempData["Message"] = "Delete failed! Error! An error occurred. Please try again later.";
                }
            }
            return RedirectToAction("Customers");
        }
    }
}
