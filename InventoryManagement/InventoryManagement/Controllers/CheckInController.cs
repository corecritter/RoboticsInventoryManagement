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
        public ActionResult Index(int? id)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index"});
            int result = (int)id;
            var school = db.Schools.Find(id);
            if (school == null)
            {
                TempData["error"] = "Cannot find selected school";
                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
            }
            var rentedItems = getRentedItems(result);
            if(rentedItems.Count == 0)
            {
                TempData["error"] = "Selected School does not have items to be checked in";
                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
            }
            IList<ItemTypes> rentedItemTypes = new List<ItemTypes>();
            IList<bool> rentedItemsTypesCheckboxes = new List<bool>();
            foreach (var item in rentedItems)
            {
                int index = rentedItemTypes.IndexOf(item.ItemType);
                if (index == -1)
                {
                    rentedItemTypes.Add(item.ItemType);
                    rentedItemsTypesCheckboxes.Add(false);
                }
            }
            CheckInItemTypeSelectViewModel vm = new CheckInItemTypeSelectViewModel
            {
                ItemTypesModel = rentedItemTypes.OrderBy(itemType => itemType.ItemName).ToList(),
                ItemTypesCheckboxes = rentedItemsTypesCheckboxes,
                SelectedSchoolId = result
            };
            return View(vm);
        }
        private IList<Items> getRentedItems(int id) {
            var school = db.Schools.Find(id);
            var rentedItems = db.Items.Where(item => item.CheckedOutSchoolId == id);//.OrderBy(item => item.ItemType.ItemName);
            var results = rentedItems.Where(item => item.CheckedInById == null).OrderBy(item => item.ItemType.ItemName).ToList();
            return results;
        }

        public ActionResult ItemTypesSubmit(CheckInItemTypeSelectViewModel vm)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null)
                return RedirectToAction("Index");
            int index = 0;
            IList<ItemTypes> selectedItemTypes = new List<ItemTypes>();
            IList<int> itemQuantities = new List<int>();
            foreach(bool isChecked in vm.ItemTypesCheckboxes)
            {
                if (isChecked)
                {
                    var itemType = db.ItemTypes.Find(vm.ItemTypesModel[index].ItemTypeId);
                    if (itemType == null)
                        return RedirectToAction("Index");
                    selectedItemTypes.Add(itemType);
                    var itemsRented = db.Items.Where(item => item.CheckedOutSchoolId == vm.SelectedSchoolId);
                    itemsRented = itemsRented.Where(item => item.ItemTypeId == itemType.ItemTypeId);
                    itemsRented = itemsRented.Where(item => item.CheckedInById == null);
                    int numRented = itemsRented.ToList().Count;
                    itemQuantities.Add(numRented);
                }
                index++;
            }
            CheckInQuantitySelectViewModel quantityVm = new CheckInQuantitySelectViewModel
            {
                SelectedItemTypesModel = selectedItemTypes,
                ItemQuantityFields = itemQuantities,
                SelectedSchoolId = vm.SelectedSchoolId
            };
            TempData["CheckInQuantitySelectViewModel"] = quantityVm;
            return RedirectToAction("QuantitySelect");
        }
        public ActionResult QuantitySelect()
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["CheckInQuantitySelectViewModel"] == null)
                return RedirectToAction("Index");
            CheckInQuantitySelectViewModel vm = (CheckInQuantitySelectViewModel)TempData["CheckInQuantitySelectViewModel"];
            return View(vm);
        }
        public ActionResult QuantitySubmit(CheckInQuantitySelectViewModel vm)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null)
                return RedirectToAction("Index");
            var school = db.Schools.Find(vm.SelectedSchoolId);
            if (school == null)
                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });

            IList<Items> itemsToReturn = new List<Items>();
            IList<string> itemDisplayString = new List<string>();
            IList<string> itemDisplayLabels = new List<string>();

            var rentedItems = getRentedItems(vm.SelectedSchoolId);
            int index = 0;
            foreach(int quantity in vm.ItemQuantityFields)
            {
                if(quantity > 0)
                {
                    var potentialItems = rentedItems.Where(item => item.ItemTypeId == vm.SelectedItemTypesModel[index].ItemTypeId).ToList();
                    if (potentialItems.Count == 0) {
                        return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
                    }
                    if(quantity <= potentialItems.Count)
                    {
                        itemDisplayString.Add(quantity + " x " + potentialItems[0].ItemType.ItemName);
                        if (potentialItems[0].ItemType.HasLabel)
                            itemDisplayLabels.Add(potentialItems[0].Label.LabelName);
                        else
                            itemDisplayLabels.Add("(No Label)");
                        for (int i=0; i<quantity; i++)
                        {
                            var item = db.Items.Find(potentialItems[i].ItemId);
                            if (item == null)
                                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
                            itemsToReturn.Add(item);
                        }
                    }
                    else
                    {
                        TempData["error"] = "Entered quantity is greater than number of items checked out";
                        return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
                    }
                }
                index++;
            }
            CheckInItemConfirmModel confirmVm = new CheckInItemConfirmModel
            {
                ItemDisplayString = itemDisplayString,
                ItemDisplayLabels = itemDisplayLabels,
                ItemsToReturn = itemsToReturn,
                SelectedSchoolId = vm.SelectedSchoolId
            };
            TempData["CheckInItemConfirmModel"] = confirmVm;
            return RedirectToAction("Confirm");
        }
        public ActionResult Confirm()
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["CheckInItemConfirmModel"] == null)
                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });

            CheckInItemConfirmModel vm = (CheckInItemConfirmModel)TempData["CheckInItemConfirmModel"];
            return View(vm);
        }


        //Check All Items being returned
        public ActionResult CheckInItems(CheckInItemConfirmModel vm)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null || Session["LoggedUserId"]==null)
                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
            string userName = (string)Session["LoggedUserID"];
            for(int i=0; i< vm.ItemsToReturn.Count; i++)
            {
                var item = db.Items.Find(vm.ItemsToReturn[i].ItemId);
                if (item == null)
                    return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
                if (item.CheckedInById != null)
                    return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
                item.CheckedInById = userName;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            //TempData["CheckInViewModel"] = vm;
            //return RedirectToAction("InventoryReturnReminder");
            return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
        }

        public ActionResult InventoryReturnReminder()
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["CheckInViewModel"] != null)
                return View((CheckInViewModel)TempData["CheckInViewModel"]);
            else
                return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InventoryReturnConfirm()
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
        }
    }
}