using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (Session["LoggedUserName"] != null)
            {
                if ((bool)Session["isAdmin"])
                    return RedirectToAction("Index", new { Controller = "ItemTypes", Action = "Index" });
                else
                    return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
            }
            return View();
        }
    }
}