using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Models;
using System.Data.Entity;

namespace InventoryManagement.Controllers
{
    public class CheckInController : Controller
    {
        private ItemContext db = new ItemContext();

        //Show All checked out items by the selected school
        public ActionResult Index(int id)
        {
            var rentedItems = db.Items.Where(item => item.CheckedOutSchoolId == id).ToList();
            IList<bool> rentedItemsCheckboxes = new List<bool>();
            foreach (var item in rentedItems)
                rentedItemsCheckboxes.Add(false);
            CheckInViewModel vm = new CheckInViewModel
            {
                RentedItems = rentedItems,
                RentedItemsCheckboxes = rentedItemsCheckboxes
            };
            return View(vm);
        }
        //Check All Items being returned
        public ActionResult CheckInItems(CheckInViewModel vm)
        {
            int currIndex = 0;
            foreach(bool selectedItem in vm.RentedItemsCheckboxes)
            {
                if (selectedItem)
                {
                    var dbItem = db.Items.Find(vm.RentedItems[currIndex].ItemId);
                    dbItem.CheckedInById = (string)Session["LoggedUserID"];
                    dbItem.CheckedOutSchoolId = null;
                    db.Entry(dbItem).State = EntityState.Modified;
                    db.SaveChanges();
                }
                   currIndex++;
            }
            TempData["CheckInViewModel"] = vm;
            return RedirectToAction("InventoryReturnReminder");
        }

        public ActionResult InventoryReturnReminder()
        {
            if (TempData["CheckInViewModel"] != null)
                return View((CheckInViewModel)TempData["CheckInViewModel"]);
            else
                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InventoryReturnConfirm()
        {
            return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
        }
    }
}