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
        public ActionResult Login()
        {
            return View();
        }
    }
}