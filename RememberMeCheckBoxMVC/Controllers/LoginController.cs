using RememberMeCheckBoxMVC.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RememberMeCheckBoxMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginDBEntities db = new LoginDBEntities();
        // GET: Login
        public ActionResult Index()
        {
            // want to retrive saved cookies into username and password
            HttpCookie cookie = Request.Cookies["User"];
            if(cookie!=null)
            {
                ViewBag.username = cookie["username"].ToString();
                //DECRYPT PASSWORD.... FOLLOWING THREE LINE
                string EncryptPassword = cookie["password"].ToString();
                byte[] b = Convert.FromBase64String(EncryptPassword);
                string decryptPassword = ASCIIEncoding.ASCII.GetString(b);
                ViewBag.password = decryptPassword.ToString();

              //ViewBag.password = cookie["password"].ToString();
            }
            return View();
        }

        //when we will click on button login this method Index(User u) will call
        [HttpPost]
        public ActionResult Index(User u)
        {
            HttpCookie cookie = new HttpCookie("User");
            if (ModelState.IsValid==true)
            {
                if(u.RememberMe==true)
                {
                    
                    cookie["username"] = u.username;
                    //to Encript password following 2 line
                    byte[] b = ASCIIEncoding.ASCII.GetBytes(u.password);
                   string EncryptedPassword = Convert.ToBase64String(b);

                    cookie["password"] = EncryptedPassword;
                    //cookie["password"] = u.password;
                    cookie.Expires = DateTime.Now.AddDays(2);
                    HttpContext.Response.Cookies.Add(cookie);
                }
                else
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Response.Cookies.Add(cookie);
                }
                var row = db.Users.Where(model => model.username == u.username && model.password == u.password).FirstOrDefault();
                if(row!=null)
                {
                    Session["Username"] = u.username;
                    TempData["Message"] = "<script>alert('Login Successfull')</script>";
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    TempData["Message"] = "<script>alert('Login Failed')</script>";
                }
            }
            return View();
        }
    }
}