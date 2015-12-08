using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Database;

namespace InventoryManagement.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        UsersContext db = new UsersContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Users use)
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == use.UserName && u.Password == use.Password);
            if (user != null) //Valid credentials 
            {
                Session["LoggedUserName"] = user.FirstName;
                Session["LoggedUserID"] = user.UserName;
                Session["isAdmin"] = user.isAdmin;
            }
            else //Login UserName/PassWord doesn't exist
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View("Index",use);

            }
            return AfterLogin();
        }
        public ActionResult AfterLogin()
        {
            if (Session["LoggedUserId"] != null && (bool)Session["isAdmin"])
            {
                return RedirectToAction("Index", "ItemTypes");
            }
            else if (Session["LoggedUserId"] != null)
            {
                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
            }
            return RedirectToAction("Index");
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session["LoggedUserID"] = null;
            Session["LoggedUserName"] = null;
            Session["isAdmin"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}