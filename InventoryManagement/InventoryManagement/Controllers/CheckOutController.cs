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
        }
        
        //for check in items view
        public ActionResult CheckIn(int id)
        {
            return null;
        }

        //for check out items view
        public ActionResult CheckOut(int id)
        {
            if (ModelState.IsValid)
            {
                CheckOutViewModel checkOutViewModel = new CheckOutViewModel { Items = db.Items.ToList() };
                return View(checkOutViewModel);
            }
            else
            {
                CheckOutViewModel vm = new CheckOutViewModel { Schools = db.Schools.ToList() };
                return View(vm);
            }
        }

        //to do the actual checking out, the second parameter is suppose to be the school id
        public ActionResult GiveThemItem(CheckOutViewModel vm, int id)
        {
            int index = 0;
            IList<int> itemIds = (List<int>)TempData["ItemIds"]; //all the items
            IList<int> checkedItems = new List<int>();
            int numSelected = vm.ItemsCheckBoxes.Where(x => x).Count();

            foreach (bool isChecked in vm.ItemsCheckBoxes)
            {
                if (isChecked)
                    checkedItems.Add(itemIds[index]);
                index++;
            }
            //now to add to each item the school id in the db

            return null;
        }

    }
}