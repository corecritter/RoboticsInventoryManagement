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
            return View(db.Bundles.ToList());
        }

        // GET: Bundles/Details/5
        public ActionResult Details(int? id)
        {
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

        // GET: Bundles/Create
        public ActionResult Create()
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

        // POST: Bundles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BundlesViewModel vm)
        {
            if (vm.SchoolsCheckboxes!=null)
            {
                //if (labels.Count < numSelected) //Not enough labels for the number of item types selected
                // return null;

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
            if (vm.ItemTypesCheckboxes != null)
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
            if (TempData["BundlesViewModel"] == null)
                return RedirectToAction("Index");
            //For every item type selected, display a quantity field
            BundlesViewModel vm = (BundlesViewModel)TempData["BundlesViewModel"];
            
            IList<int> itemQuantityFields = new List<int>();
            foreach(var selectedItemType in vm.SelectedItemTypes)
            {
                itemQuantityFields.Add(1); 
            }
            vm.ItemQuantityFields = itemQuantityFields; 
            return View(vm);
        }
        public ActionResult ItemQuantitySubmit(BundlesViewModel vm)
        {
            if (vm == null)
                return RedirectToAction("Index");
            var itemTypes = db.ItemTypes.ToList();
            var labels = db.Labels.ToList();
            List<Bundles> tempBundle = new List<Bundles>(); //Temporarily hold new bundles
            IList<Items> tempItems = new List<Items>();    //Temporarily hold new items
            int numSchools = vm.SelectedSchoolIds.Count;
            //For Every Selected School
            foreach (int schoolId in vm.SelectedSchoolIds)
            {
                bool canCreate = true;
                //For Every Selected Item Type
                for (int i = 0; i < vm.ItemTypes.Count; i++)
                {
                    var itemType = db.ItemTypes.Find(vm.ItemTypes[i].ItemTypeId);
                    if (itemType == null)
                        return RedirectToAction("Index");
                    var available = itemType.Item.Where(x => x.CheckedInById == null); //Can't be checked in
                    available = available.Where(x => x.CheckedOutById == null);            //checked out
                    available = available.Where(x => x.BundleId == null);                  //or assigned to a bundle
                    if(itemType.HasLabel)
                        available = available.Where(x => x.LabelId == schoolId);
                    var availableItems = available.ToList();
                    int numAvailable = availableItems.Count();
                    int desiredQuantity = vm.ItemQuantityFields[i];
                    if (desiredQuantity < 1)
                        return RedirectToAction("Index");
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
                    var newBundle = new Bundles { BundleName = vm.BundleName, SchoolId = schoolId };
                    tempBundle.Add(newBundle);
                }
                else
                    return null;
            }
            //Add all the temp bundles to the database
            for (int i = 0; i < tempBundle.Count; i++)
            {
                db.Bundles.Add(tempBundle[i]);
                db.SaveChanges();
            }
            //Associate Temporary Items with bundle id and save
            for (int i = 0; i < tempBundle.Count; i++)
            {
                int currIndex = 0;
                for (int j = i * numSchools; j < numSchools * (i + 1); j++)
                {
                    var dbItem = db.Items.Find(tempItems[j].ItemId);
                    dbItem.BundleId = tempBundle[currIndex].BundleId;
                    db.Entry(dbItem).State = EntityState.Modified;
                    db.SaveChanges();
                    currIndex++;
                }
            }
            return RedirectToAction("Index");
        }


        // GET: Bundles/Edit/5
        public ActionResult Edit(int? id)
        {
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
            if (ModelState.IsValid)
            {
                db.Entry(bundles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bundles);
        }

        // GET: Bundles/Delete/5
        public ActionResult Delete(int? id)
        {
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

        // POST: Bundles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bundles bundles = db.Bundles.Find(id);
            db.Bundles.Remove(bundles);
            db.SaveChanges();
            return RedirectToAction("Index");
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
