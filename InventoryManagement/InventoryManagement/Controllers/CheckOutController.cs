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
            CheckOutViewModel vm = new CheckOutViewModel { Schools = db.Schools.ToList() };
            return View(vm);
        }

        //Called When School Is selected, store SchoolId and Direct to Bundles Selection
        public ActionResult SelectSchoolOut(int id)
        {
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
            //var associatedBundles = from bundle in db.Bundles where bundle.SchoolId == ((int)TempData["SelectedSchoold"]) select bundle;
            if(TempData["SelectedSchoold"]==null)
                return RedirectToAction("Index");
            int selectedSchoolId = (int)TempData["SelectedSchoold"];
            var associatedBundles = db.Schools.Find(selectedSchoolId).Bundles;
            CheckOutViewModel vm = new CheckOutViewModel { Bundles = associatedBundles, SelectedSchoolId = selectedSchoolId  };
            return View(vm);
        }

        //Bundle is now selected, create model, store, redirect to ItemTypes Selection
        public ActionResult MakeBundleSelection(int bundleId, int schoolId)
        {
            IList<bool> itemTypeCheckboxes = new List<bool>();
            if (bundleId!=0)//A Bundle Has been Selected
            {
                var selectedBundle = db.Bundles.Find(bundleId);
                bool found;
                foreach(var ItemType in db.ItemTypes)
                {
                    //var available = itemTypes[i].Item.Where(x => x.CheckedInById == null); //Can't be checked in
                    found = false;
                    for(int i=0; i< selectedBundle.Items.Count; i++)
                    {
                        if (selectedBundle.Items[i].ItemTypeId == ItemType.ItemTypeId)
                        {
                            found = true;
                            itemTypeCheckboxes.Add(true);
                            break;
                        }   
                    }
                    if(!found)
                        itemTypeCheckboxes.Add(false);
                }
            }
            else //A bundle has not been selected
            {
                foreach (var ItemType in db.ItemTypes)
                {
                    itemTypeCheckboxes.Add(false);
                }
            }
            CheckOutViewModel vm = new CheckOutViewModel
            {
                ItemTypesModel = db.ItemTypes.ToList(),
                ItemTypesCheckboxes = itemTypeCheckboxes,
                SelectedSchoolId = schoolId,
                SelectedBundleId = bundleId
            };
            TempData["CheckOutViewModel"] = vm;
            return RedirectToAction("ItemTypesSelect");
        }

        //Select Desired Item Types to be Rented
        public ActionResult ItemTypesSelect()
        {
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
            int bundleId = vm.SelectedBundleId;
            var selectedBundle = db.Bundles.Find(bundleId);
            int currItemTypeIndex = 0;
            IList<Items> itemsToCheckOut = new List<Items>();
            foreach (bool isSelected in vm.ItemTypesCheckboxes)
            {
                if (isSelected)
                {
                    var currItemType = db.ItemTypes.Find(vm.ItemTypesModel[currItemTypeIndex].ItemTypeId);
                    if (bundleId > 0)
                        foreach (var item in selectedBundle.Items)
                        {
                            if (item.ItemTypeId == currItemType.ItemTypeId && item.BundleId == vm.SelectedBundleId && item.CheckedOutById == null)
                                itemsToCheckOut.Add(item);
                        }
                    else
                    {
                        var potentialItems = currItemType.Item.Where(item => item.CheckedOutById == null);
                        potentialItems = potentialItems.Where(item => item.BundleId == null);
                        if (potentialItems.ToList().Count > 0)
                            itemsToCheckOut.Add(potentialItems.First());
                    }
                }
                currItemTypeIndex++;
            }
            vm.ItemsToCheckOut = itemsToCheckOut;
            TempData["CheckOutViewModel"] = vm;
            return RedirectToAction("ItemsSelect");
        }


        //Show View With Items and Labels to hand out
        public ActionResult ItemsSelect()
        {
            return View((CheckOutViewModel)TempData["CheckOutViewModel"]);
        }

        //Save the Changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemsSubmit(CheckOutViewModel vm)
        {
            if (Session["LoggedUserID"] != null)
            {
                foreach (var item in vm.ItemsToCheckOut)
                {

                    if (item.CheckedOutById == null)
                    {
                        var dbItem = db.Items.Find(item.ItemId);
                        dbItem.CheckedOutById = (string)Session["LoggedUserID"];
                        dbItem.CheckedOutSchoolId = vm.SelectedSchoolId;
                        db.Entry(dbItem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitItemTypes(CheckOutViewModel vm)
        {
            int currIndex = 0;
            IList<int> selectedItemTypeIds = new List<int>();
            foreach(bool itemSelected in vm.ItemTypesCheckboxes)
            {
                if (itemSelected)
                {
                    selectedItemTypeIds.Add(vm.ItemTypesModel[currIndex].ItemTypeId);
                }
                currIndex++;
            }
            return null;
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
        //for check in items view
        public ActionResult CheckIn(int id)
        {
            return null;
        }
    }
}