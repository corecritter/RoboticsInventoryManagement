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
        public ActionResult Login(string userName, string password)
        {
                //ViewBag.ReturnUrl = returnUrl;
                if (!userName.Equals(""))
                    return View("Test");
                else {
                    return null;
                }
        }
    }
}