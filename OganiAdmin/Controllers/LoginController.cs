using Microsoft.AspNetCore.Mvc;
using OganiAdmin.Models;
namespace OganiAdmin.Controllers
{
    public class LoginController : Controller
    {
        OganiContext db = new OganiContext();
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") == null){
                return View();
            }
            else return RedirectToAction("Index","Home");
            return View();
        }
        [HttpPost]
        public IActionResult Login(Models.Customer cus)
        {
            if(HttpContext.Session.GetString("UserName")==null)
            {
                var u = db.Customers.Where(X=>X.CusEmail.Equals(cus.CusEmail)&&X.CusPassword.Equals(cus.CusPassword)).FirstOrDefault();
                if (u != null ) 
                {
                    HttpContext.Session.SetString("UserName", cus.CusEmail.ToString());
                    return RedirectToAction("Index", "Categories");
                }
                else
                {
                    // Thêm thông báo lỗi vào ModelState
                    ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu.");
                }
            }
            return View();
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login", "Login");
        }
    }
}
