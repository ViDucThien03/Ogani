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
    public class HomeController : Controller
    {
        
        OganiContext data = new OganiContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authentication]
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult SearchForm(string strSearch, int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listProduct = data.Products.AsNoTracking();

            if (!string.IsNullOrEmpty(strSearch))
            {
                listProduct = listProduct.Where(x => x.ProductName.Contains(strSearch));
            }
            var orderedListProduct = listProduct.OrderBy(x => x.ProductName);

            var pagedProducts = orderedListProduct.ToPagedList(pageNumber, pageSize);

            return View("Products", pagedProducts);
        }

        [Route("Products")]
        public IActionResult Products(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listProduct = data.Products.AsNoTracking().OrderBy(x => x.ProductId);
            PagedList<Product> list = new PagedList<Product>(listProduct, pageNumber, pageSize);
            return View(list);
        }
        [Route("InsertProduct")]
        [HttpGet]
        public IActionResult InsertProduct()
        {
            var list = new SelectList(data.Categories.OrderBy(p => p.CateId).Select(p => p.CateId).ToList());
            ViewBag.CateId = list;
            return View();
        }
        [Route("InsertProduct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                data.Products.Add(product);
                data.SaveChanges();
                return RedirectToAction("Products");
            }
            return View(product);
        }
        [Route("EditProduct")]
        [HttpGet]
        public IActionResult EditProduct(int ProductID)
        {
            //ViewBag.CateId = new SelectList(data.Categories.ToList(), "CateID");
            var list = new SelectList(data.Categories.OrderBy(p => p.CateId).Select(p => p.CateId).ToList());
            ViewBag.CateId = list;
            var product = data.Products.Find(ProductID);
            return View(product);
        }
        [Route("EditProduct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                data.Entry(product).State = EntityState.Modified;
                data.SaveChanges();
                return RedirectToAction("Products", "Home");
            }
            return View(product);
        }
        [HttpGet]
        public IActionResult DeleteProduct(int ProductID)
        {
            TempData["Message"] = "";
            var product = data.Products.Find(ProductID);
            if (product == null)
            {
                TempData["Message"] = "Delete failed! No product found.";
                return RedirectToAction("Products");
            }
            try
            {
                data.Products.Remove(product);
                data.SaveChanges();
                TempData["Message"] = "Delete Successfully!";
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    TempData["Message"] = "This product cannot be deleted because there are related orders.";
                }
                else
                {
                    TempData["Message"] = "Delete failed! Error! An error occurred. Please try again later.";
                }
            }
            return RedirectToAction("Products");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
