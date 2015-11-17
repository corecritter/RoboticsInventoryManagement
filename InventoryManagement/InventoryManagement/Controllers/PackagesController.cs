using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InventoryManagement.Controllers
{
    public class PackagesController : Controller
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
            PackagesViewModel vm = new PackagesViewModel
            {
                Schools = db.Schools.ToList(),
                SchoolsCheckboxes = schools
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PackagesViewModel vm)
        {
            foreach (bool isChecked in vm.SchoolsCheckboxes)
            {

            }
            
            return RedirectToAction("ItemTypesSelect", new RouteValueDictionary(new { checkedSchools = vm.SchoolsCheckboxes }));
        }

        public ActionResult ItemTypesSelect(PackagesViewModel vm)
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
        public ActionResult ItemTypesSubmit(PackagesViewModel vm)
        {
            return null;
        }
    }
}