using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Database;
using InventoryManagement.Models;
using System.Web.Routing;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Controllers
{
    public class BundlesController : Controller
    {
        private BundleContext db = new BundleContext();

        // GET: Bundles
        public ActionResult Index()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["error"] != null)
            {
                ModelState.AddModelError("", (string)TempData["error"]);
            }
            return View(db.Bundles.ToList().OrderBy(bundle => bundle.BundleName));
        }

        // GET: Bundles/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bundles selectedBundle = db.Bundles.Find(id);
            if (selectedBundle == null)
            {
                return HttpNotFound();
            }
            IList<ItemTypes> itemTypesSelected = new List<ItemTypes>();
            IList<Items> itemsToCheckOut = new List<Items>();
            IList<string> itemDisplayString = new List<string>();
            IList<string> itemDisplayLabels = new List<string>();
            foreach (var bundleItem in selectedBundle.Items)
            {
                if (itemTypesSelected.IndexOf(bundleItem.ItemType) == -1)
                {
                    itemTypesSelected.Add(bundleItem.ItemType);
                    var itemsOfType = selectedBundle.Items.Where(item => item.ItemTypeId == bundleItem.ItemTypeId).OrderBy(item => item.ItemType.ItemName).ToList();
                    itemDisplayString.Add(itemsOfType.Count + " x " + bundleItem.ItemType.ItemName);
                    if (bundleItem.ItemType.HasLabel)
                        itemDisplayLabels.Add(bundleItem.Label.LabelName);
                    else
                        itemDisplayLabels.Add("(No Label)");
                    foreach (var itemToCheckout in itemsOfType)
                        itemsToCheckOut.Add(itemToCheckout);
                }
            }
            BundlesDetailsViewModel vm = new BundlesDetailsViewModel
            {
                BundleModel = selectedBundle,
                ItemDisplayString = itemDisplayString,
                ItemDisplayLabels = itemDisplayLabels
            };
            return View(vm);
        }

        // GET: Bundles/Create
        public ActionResult Create()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
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

        // POST: Bundles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BundlesViewModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm!=null || vm.SchoolsCheckboxes!=null)
            {
                if (vm.BundleName == null || vm.BundleName.Trim().Equals(""))
                {
                    TempData["error"] = "Bundle Must Have a name";
                    return RedirectToAction("Index");
                }
                IList<int> checkedSchools = new List<int>();
                var labels = db.Labels.ToList();
                int numSelected = vm.SchoolsCheckboxes.Where(x => x).Count();

                int index = 0;
                foreach (bool isChecked in vm.SchoolsCheckboxes)
                {
                    if (isChecked)
                        checkedSchools.Add(vm.Schools[index].SchoolId);
                    index++;
                }
                vm.SelectedSchoolIds = checkedSchools;
                TempData["BundlesViewModel"] = vm;
                return RedirectToAction("ItemTypesSelect");
            }
            return RedirectToAction("Index");
        }
        public ActionResult ItemTypesSelect()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["BundlesViewModel"] != null)
            {
                IList<bool> itemTypes = new List<bool>(); //For all the checkboxes
                foreach (var itemType in db.ItemTypes)
                {
                    itemTypes.Add(false); //Default to true (checked)
                }
                BundlesViewModel vm = (BundlesViewModel)TempData["BundlesViewModel"];
                vm.ItemTypesCheckboxes = itemTypes;
                vm.ItemTypes = db.ItemTypes.OrderBy(itemType => itemType.ItemName).ToList();
                return View(vm);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemTypesSubmit(BundlesViewModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm!=null || vm.ItemTypesCheckboxes != null)
            {
                int numSelected = vm.ItemTypesCheckboxes.Where(x => x).ToList().Count;
                if (numSelected == 0)
                {
                    ModelState.AddModelError("", "At least one Item Type must be selected");
                    return View(vm);
                }
                List<ItemTypes> selectedItemTypes = new List<ItemTypes>();
                int index = 0;
                foreach (bool selectedItem in vm.ItemTypesCheckboxes)
                {
                    if (selectedItem)
                    {
                        selectedItemTypes.Add(vm.ItemTypes[index]);
                    }
                    index++;
                }
                vm.SelectedItemTypes = selectedItemTypes;
                TempData["BundlesViewModel"] = vm;
                return RedirectToAction("ItemQuantitySelect");
            }
            else
                return RedirectToAction("Index");

        }
        public ActionResult ItemQuantitySelect()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["BundlesViewModel"] == null)
                return RedirectToAction("Index");
            //For every item type selected, display a quantity field
            BundlesViewModel vm = (BundlesViewModel)TempData["BundlesViewModel"];
            
            IList<int> itemQuantityFields = new List<int>(); //Int for every quantity
            foreach(var selectedItemType in vm.SelectedItemTypes)
            {
                itemQuantityFields.Add(1); 
            }
            vm.ItemQuantityFields = itemQuantityFields; 
            return View(vm);
        }
        public ActionResult ItemQuantitySubmit(BundlesViewModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null)
                return RedirectToAction("Index");
            var itemTypes = db.ItemTypes.ToList();
            var labels = db.Labels.ToList();
            List<Bundles> tempBundle = new List<Bundles>(); //Temporarily hold new bundles
            IList<Items> tempItems = new List<Items>();    //Temporarily hold new items
            int numSchools = vm.SelectedSchoolIds.Count;

            //Create the Bundles
            //For Every Selected School
            foreach (int schoolId in vm.SelectedSchoolIds)
            {
                bool canCreate = true;
                //For Every Selected Item Type
                for (int i = 0; i < vm.SelectedItemTypes.Count; i++)
                {
                    var itemType = db.ItemTypes.Find(vm.SelectedItemTypes[i].ItemTypeId);
                    if (itemType == null)
                    {
                        TempData["error"] = "An Item Type has been removed";
                        return RedirectToAction("Index");
                    }
                    var available = itemType.Item.Where(x => x.CheckedInById == null); //Can't be checked in
                    available = available.Where(x => x.CheckedOutById == null);            //checked out
                    available = available.Where(x => x.BundleId == null);                  //or assigned to a bundle
                    if(itemType.HasLabel)
                        available = available.Where(x => x.LabelId == schoolId);
                    var availableItems = available.Where(item => tempItems.IndexOf(item) < 0).ToList();
                    int numAvailable = availableItems.Count();
                    int desiredQuantity = vm.ItemQuantityFields[i];
                    if (desiredQuantity < 1)
                    {
                        TempData["error"] = "Quantity must be at least 1";
                        return RedirectToAction("Index");
                    }
                    if(numAvailable >= desiredQuantity)
                    {
                        for (int j = 0; j < desiredQuantity; j++)
                        {
                            tempItems.Add(availableItems[j]);
                        }
                    }
                    //Stop The Process
                    else
                    {
                        canCreate = false; //Not enough items to match demand
                        break;
                    }
                }
                //Create Temporary Bundle
                if (canCreate)
                {
                    var school = db.Schools.Find(schoolId);
                    var newBundle = new Bundles { BundleName = vm.BundleName + " (" + school.SchoolName  +")", SchoolId = schoolId };
                    tempBundle.Add(newBundle);
                }
                else
                {
                    TempData["error"] = "Not enough items available to meet demand";
                    return RedirectToAction("Index");
                }
            }
            //Add all the temp bundles to the database
            for (int i = 0; i < tempBundle.Count; i++)
            {
                db.Bundles.Add(tempBundle[i]);
                db.SaveChanges();
            }
            //Associate Temporary Items with bundle id and save
            int currItemIndex = 0;
            for (int i = 0; i < tempBundle.Count; i++)  //For every bundle
            {
                for(int j=0; j<vm.SelectedItemTypes.Count; j++) //For every selected item type
                {
                    for (int k = 0; k < vm.ItemQuantityFields[j]; k++) //For every Selected Item type quantity
                    {
                        var dbItem = db.Items.Find(tempItems[currItemIndex].ItemId);
                        if (dbItem == null || dbItem.BundleId != null)
                        {
                            for (int m = 0; m < tempBundle.Count; m++)
                                DeleteBundle(tempBundle[m].BundleId);
                            TempData["error"] = "Item has been assigned to another bundle or it was removed";
                            return RedirectToAction("Index");
                        }
                        dbItem.BundleId = tempBundle[i].BundleId;
                        db.Entry(dbItem).State = EntityState.Modified;
                        db.SaveChanges();
                        currItemIndex++;
                    }
                }
            }
            return RedirectToAction("Index");
        }


        // GET: Bundles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bundles bundles = db.Bundles.Find(id);
            if (bundles == null)
            {
                return HttpNotFound();
            }
            return View(bundles);
        }

        // POST: Bundles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BundleId,BundleName")] Bundles bundles)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (ModelState.IsValid)
            {
                var bundle = db.Bundles.Find(bundles.BundleId);
                if (bundle == null)
                {
                    TempData["error"] = "Bundle has been previously removed";
                    return RedirectToAction("Index");
                }
                db.Entry(bundle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bundles);
        }

        // GET: Bundles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bundles bundle = db.Bundles.Find(id);
            if (bundle == null)
            {
                return HttpNotFound();
            }
            

            return View(bundle);
        }

        // POST: Bundles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!DeleteBundle(id))
                TempData["error"] = "Something Went wrong";
            return RedirectToAction("Index");
        }
        private bool DeleteBundle(int id)
        {
            Bundles bundles = db.Bundles.Find(id);
            if (bundles == null)
               return false;
            var associatedItems = db.Items.Where(item => item.BundleId == id).ToList();
            for (int i = 0; i < associatedItems.Count; i++)
            {
                var item = db.Items.Find(associatedItems[i].ItemId);
                if (item == null)
                    return false;
                item.BundleId = null;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            db.Bundles.Remove(bundles);
            db.SaveChanges();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
