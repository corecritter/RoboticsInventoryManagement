using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using InventoryManagement.Database;
using System.Web.Routing;

namespace InventoryManagement.Controllers
{
    public class CheckOutController : Controller
    {
        private ItemContext db = new ItemContext();

        public ActionResult Index()
        {
            CheckOutViewModel vm = new CheckOutViewModel { Schools = db.Schools.ToList() };
            return View(vm);
            //return null;
        }

        public ActionResult CheckIn(int id)
        {
            return null;
        }
        public ActionResult CheckOut(int id)
        {

            return null;
        }

    }
}