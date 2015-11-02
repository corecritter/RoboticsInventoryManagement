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
        UsersContext _db = new UsersContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(Users use)
        {
            using (UsersContext db = new UsersContext())
            {
                //IQueryable<db.Users> query;
                var user = db.Users.FirstOrDefault(u => u.UserName == use.UserName && u.Password == use.Password);
                
                if (user != null) //Valid credentials 
                {
                    Session["LoggedUserID"] = user.UserName;
                    Session["isAdmin"] = user.isAdmin;
                }
                else //Login UserName/PassWord doesn't exist
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(use);
                    //return View(use);
                    //return View("Test");
                }
                return AfterLogin();
                
            }
        }
        public ActionResult AfterLogin()
        {
            if (Session["LoggedUserId"] != null && (bool)Session["isAdmin"])
            {
                return RedirectToAction("Index", "ItemTypes");
            }
            else if (Session["LoggedUserId"] != null)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        //public ActionResult Login(string userName, string password)
        //{
        //    using(UsersContext db = new UsersContext())
        //    {
        //        //IQueryable<db.Users> query;
        //        var user = db.Users.FirstOrDefault(u => u.UserName == userName && u.Password == password);
        //        if (user == null) //Login UserName/PassWord doesn't exist
        //        {
        //            return null;
        //        }
        //        else //Valid credentials
        //        {
        //            Session["LogedUserID"] = user.UserName;
        //            Session["isAdmin"] = user.isAdmin;
        //            return View("Test");
        //        }
        //    }
        //}
    }
}