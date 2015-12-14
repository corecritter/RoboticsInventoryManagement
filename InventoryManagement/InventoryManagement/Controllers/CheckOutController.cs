using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using InventoryManagement.Database;
using InventoryManagement.Models;
using System.Web.Routing;

namespace InventoryManagement.Controllers
{
    public class CheckOutController : Controller
    {
        private ItemContext db = new ItemContext();

        //Display All Schools to Check In/ Check Out
        public ActionResult Index()
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["error"] != null)
            {
                ModelState.AddModelError("", (string)TempData["error"]);
            }
            CheckOutViewModel vm = new CheckOutViewModel { Schools = db.Schools.ToList() };
            return View(vm);
        }

        //Called When School Is selected, store SchoolId and Direct to Bundles Selection
        public ActionResult SelectSchoolOut(int id)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (ModelState.IsValid)
            {
                TempData["SelectedSchoold"] = id;
                return RedirectToAction("BundleSelection");//, new RouteValueDictionary(new { action = "BundlesSelection" }));
            }
            else
            {
                CheckOutViewModel vm = new CheckOutViewModel { Schools = db.Schools.ToList() };
                return View(vm);
            }
        }

        //Prepare BundleSelection page to Select a Bundle (if any)
        [HttpGet]
        public ActionResult BundleSelection()
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            //var associatedBundles = from bundle in db.Bundles where bundle.SchoolId == ((int)TempData["SelectedSchoold"]) select bundle;
            if (TempData["SelectedSchoold"] == null)
                return RedirectToAction("Index");
            int selectedSchoolId = (int)TempData["SelectedSchoold"];
            var associatedBundles = db.Schools.Find(selectedSchoolId).Bundles;
            CheckOutViewModel vm = new CheckOutViewModel { Bundles = associatedBundles, SelectedSchoolId = selectedSchoolId };
            return View(vm);
        }

        //Bundle is now selected, create model, store, redirect to ItemTypes Selection
        public ActionResult MakeBundleSelection(int bundleId, int schoolId)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            CheckOutViewModel vm = new CheckOutViewModel();
            IList<bool> itemTypeCheckboxes = new List<bool>();
            var itemTypes = db.ItemTypes.OrderBy(itemType => itemType.ItemName).ToList();// .ToList().OrderBy(itemType => itemType.ItemName);
            var selectedBundle = db.Bundles.Find(bundleId);
            if (selectedBundle != null)
            {
                IList<ItemTypes> itemTypesSelected = new List<ItemTypes>();
                IList<Items> itemsToCheckOut = new List<Items>();
                IList<string> itemDisplayString = new List<string>();
                IList<string> itemDisplayLabels = new List<string>();
                foreach (var bundleItem in selectedBundle.Items)
                {
                    if (bundleItem.CheckedOutById != null)
                    {
                        TempData["error"] = "All or part of bundle currently checked out";
                        return RedirectToAction("Index");
                    }
                    if (itemTypesSelected.IndexOf(bundleItem.ItemType) == -1)
                    {
                        itemTypesSelected.Add(bundleItem.ItemType);
                        var itemsOfType = selectedBundle.Items.Where(item => item.ItemTypeId == bundleItem.ItemTypeId).OrderBy(item => item.ItemType.ItemName).ToList();
                        itemDisplayString.Add(itemsOfType.Count + " x " + bundleItem.ItemType.ItemName);
                        if (bundleItem.ItemType.HasLabel)
                            itemDisplayLabels.Add(bundleItem.Label.LabelName);
                        else
                            itemDisplayLabels.Add("(No Label)");
                        foreach(var itemToCheckout in itemsOfType)
                            itemsToCheckOut.Add(itemToCheckout);
                    }
                }
                vm.ItemsToCheckOut = itemsToCheckOut;
                vm.ItemDisplayString = itemDisplayString;
                vm.ItemDisplayLabels = itemDisplayLabels;
                vm.SelectedSchoolId = schoolId;
                TempData["CheckOutViewModel"] = vm;
                return RedirectToAction("ItemsSelect");
                /*bool found;
                foreach (var ItemType in itemTypes){
                    //var available = itemTypes[i].Item.Where(x => x.CheckedInById == null); //Can't be checked in
                    found = false;
                    for (int i = 0; i < selectedBundle.Items.Count; i++){
                        if (selectedBundle.Items[i].ItemTypeId == ItemType.ItemTypeId){
                            found = true;
                            itemTypeCheckboxes.Add(true);
                            break;
                        }
                    }
                    if (!found)
                        itemTypeCheckboxes.Add(false);
                }*/
            }
            else //A bundle has not been selected or not found
            {
                foreach (var ItemType in db.ItemTypes)
                {
                    itemTypeCheckboxes.Add(false);
                }
            }
            vm.ItemTypesModel = itemTypes;
            vm.ItemTypesCheckboxes = itemTypeCheckboxes;
            vm.SelectedSchoolId = schoolId;
            vm.SelectedBundleId = bundleId;
            TempData["CheckOutViewModel"] = vm;
            return RedirectToAction("ItemTypesSelect");
        }

        //Select Desired Item Types to be Rented
        public ActionResult ItemTypesSelect()
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["CheckOutViewModel"] != null)
                return View((CheckOutViewModel)TempData["CheckOutViewModel"]);
            else
                return RedirectToAction("Index", "CheckOutController");
        }

        //Submit Desired Item Types
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemTypesSubmit(CheckOutViewModel vm)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null)
                return RedirectToAction("Index");
            IList<int> quantityFields = new List<int>();
            IList<ItemTypes> selectedItemTypes = new List<ItemTypes>();

            int currItemTypeIndex = 0;
            foreach (bool isSelected in vm.ItemTypesCheckboxes)
            {
                if (isSelected)
                {
                    var currItemType = db.ItemTypes.Find(vm.ItemTypesModel[currItemTypeIndex].ItemTypeId);
                    if (currItemType != null)
                    {
                        selectedItemTypes.Add(currItemType);
                        quantityFields.Add(1);
                    }
                }
                currItemTypeIndex++;
            }
            vm.SelectedItemTypesModel = selectedItemTypes;
            vm.ItemQuantityFields = quantityFields;
            TempData["CheckOutViewModel"] = vm;
            return RedirectToAction("QuantitySelect");
            
        }

        public ActionResult QuantitySelect()
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["CheckOutViewModel"] != null)
            {
                CheckOutViewModel vm = (CheckOutViewModel)TempData["CheckOutViewModel"];
                return View(vm);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuantitySubmit(CheckOutViewModel vm)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null)
                return RedirectToAction("Index");

            var selectedSchool = db.Schools.Find(vm.SelectedSchoolId);
            if (selectedSchool == null)
                return RedirectToAction("Index");
            IList<Items> itemsToCheckOut = new List<Items>();
            IList<string> itemDisplayString = new List<string>();
            IList<string> itemDisplayLabels = new List<string>();

            int currItemTypeIndex = 0;
            foreach (int quantity in vm.ItemQuantityFields)
            {
                if(quantity <= 0)
                {
                    TempData["error"] = "Quantity must be at least 1";
                    return RedirectToAction("Index");
                }
                var currItemType = db.ItemTypes.Find(vm.SelectedItemTypesModel[currItemTypeIndex].ItemTypeId);
                if (currItemType == null)
                    return RedirectToAction("Index");

                var potentialItems = currItemType.Item.Where(item => item.CheckedOutById == null);
                potentialItems = potentialItems.Where(item => item.CheckedInById == null);
                potentialItems = potentialItems.Where(item => item.BundleId == null);
                potentialItems = potentialItems.Where(item => item.CheckedOutSchoolId == null);
                potentialItems = potentialItems.Where(item => item.InventoryLocationId != null);
                if (currItemType.HasLabel) //Only filter by label if the Item Type has labels
                    potentialItems = potentialItems.Where(item => item.LabelId == selectedSchool.LabelId);

                var availableItems = potentialItems.OrderBy(item => item.ItemType.ItemName).ToList();
                if (availableItems.Count >= quantity)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        itemsToCheckOut.Add(availableItems[i]);
                    }
                    itemDisplayString.Add(quantity + " x " + availableItems[0].ItemType.ItemName);
                    if (currItemType.HasLabel)
                        itemDisplayLabels.Add(availableItems[0].Label.LabelName);
                    else
                        itemDisplayLabels.Add("(No Label)");
                }
                else
                { //Not enough Items to match requested quantity
                    TempData["error"] = "Not enough " + currItemType.ItemName + "s available";
                    return RedirectToAction("Index");
                }
                currItemTypeIndex++;
            }
            vm.ItemsToCheckOut = itemsToCheckOut;
            vm.ItemDisplayString = itemDisplayString;
            vm.ItemDisplayLabels = itemDisplayLabels;
            TempData["CheckOutViewModel"] = vm;
            return RedirectToAction("ItemsSelect");
        }
        //Show View With Items and Labels to hand out
        public ActionResult ItemsSelect()
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["CheckOutViewModel"]!=null)
                return View((CheckOutViewModel)TempData["CheckOutViewModel"]);
            return RedirectToAction("Index");
        }

        //Save the Changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemsSubmit(CheckOutViewModel vm)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (Session["LoggedUserID"] == null || vm == null)
                return RedirectToAction("Index");
            IList<Items> tocheckout = new List<Items>();
            string userName = (string)Session["LoggedUserID"];
            foreach (var item in vm.ItemsToCheckOut)
            {
                var dbItem = db.Items.Find(item.ItemId);
                if (dbItem != null && dbItem.CheckedOutById == null && dbItem.CheckedInById == null && dbItem.CheckedOutSchoolId == null)
                {
                    tocheckout.Add(dbItem);
                }
                else
                {
                    TempData["error"] = "An item selected has already been checked out. Please try again";
                    return RedirectToAction("Index");
                }

            }
            foreach(var item in tocheckout)
            {
                item.CheckedOutById = userName;
                item.CheckedOutSchoolId = vm.SelectedSchoolId;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Clicked Check In, redirect to CheckIn Controller, pass school Id
        public ActionResult CheckIn(int id)
        {
            if (Session["isAdmin"] == null)
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            return RedirectToAction("Index", new { Controller="CheckIn", Action="Index", id=id });
        }
    }
}