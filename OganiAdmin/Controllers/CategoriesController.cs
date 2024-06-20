using Microsoft.AspNetCore.Mvc;
using OganiAdmin.Models;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace OganiAdmin.Controllers
{
    public class CategoriesController : Controller
    {
        OganiContext data = new OganiContext();

        public IActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listCategory = data.Categories.AsNoTracking().OrderBy(x => x.CateId);
            PagedList<Category> list = new PagedList<Category>(listCategory, pageNumber, pageSize);
            return View(list);
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CateName = new SelectList(data.Categories.ToList(), "CateId", "CateName");
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {   

            if (ModelState.IsValid)
            {
                TempData["Message"] = "Added successfully!";
                data.Categories.Add(category);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit(int CateId)
        {
            ViewBag.CateName = new SelectList(data.Categories.ToList(), "CateId", "CateName");
            var category = data.Categories.Find(CateId);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                data.Entry(category).State = EntityState.Modified;
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(int CateId)
        {
            TempData["Message"] = "";
            var category = data.Categories.Find(CateId);
            if (category == null)
            {
                TempData["Message"] = "Xóa không thành công! Không tìm thấy danh mục.";
                return RedirectToAction("Index");
            }
            try
            {
                data.Categories.Remove(category);
                data.SaveChanges();
                TempData["Message"] = "Xóa thành công!";
            }
            catch (DbUpdateException ex)
            {
                
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    TempData["Message"] = "Không thể xóa danh mục này vì có sản phẩm liên quan.";
                }
                else
                {
                    TempData["Message"] = "Xóa không thành công! Đã xảy ra lỗi.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
