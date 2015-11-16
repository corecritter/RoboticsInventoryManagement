using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Controllers
{
    public class PackagesController : Controller
    {
        private PackageContext db = new PackageContext();
        // GET: Packages
        public ActionResult Index()
        {
            IList<SelectListItem> schools = db.Schools.Select(x => new SelectListItem
            {
                Text = x.SchoolName,
                Value = x.SchoolId.ToString()
            }).ToList();
            schools.Insert(0, new SelectListItem { Text = "All Schools", Value = null });
            PackagesViewModel vm = new PackagesViewModel
            {
                Schools = schools
            };
            return View(vm);
        }

    }
}