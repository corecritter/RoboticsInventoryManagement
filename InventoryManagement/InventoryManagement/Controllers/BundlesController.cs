using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InventoryManagement.Controllers
{
    public class BundlesController : Controller
    {
        private PackageContext db = new PackageContext();
        // GET: Packages
        public ActionResult Index()
        {
            //Checkbox for every school
            IList<bool> schools = new List<bool>();
            foreach (var school in db.Schools)
            {
                schools.Add(true); //Default to true (checked)
            }
            BundlesViewModel vm = new BundlesViewModel
            {
                Schools = db.Schools.ToList(),
                SchoolsCheckboxes = schools
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BundlesViewModel vm)
        {
            foreach (bool isChecked in vm.SchoolsCheckboxes)
            {

            }
            BundlesViewModel vm2 = new BundlesViewModel { SchoolsCheckboxes = vm.SchoolsCheckboxes };
            //return RedirectToAction("ItemTypesSelect", new RouteValueDictionary(new { checkedSchools = vm.SchoolsCheckboxes }));
            return RedirectToAction("ItemTypesSelect", new RouteValueDictionary(new { vm = vm2}));
        }

        public ActionResult ItemTypesSelect(BundlesViewModel vm)
        {
            IList<bool> itemTypes = new List<bool>();
            foreach (var itemType in db.ItemTypes)
            {
                itemTypes.Add(false); //Default to true (checked)
            }

            vm.ItemTypesCheckboxes = itemTypes;
            vm.ItemTypes = db.ItemTypes.ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemTypesSubmit(BundlesViewModel vm)
        {
            return null;
        }
    }
}