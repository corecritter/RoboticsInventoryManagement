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
            IList<int> schoolIds = new List<int>();
            foreach (var school in db.Schools)
            {
                schools.Add(true); //Default to true (checked)
                schoolIds.Add(school.SchoolId);
            }
            BundlesViewModel vm = new BundlesViewModel
            {
                Schools = db.Schools.ToList(),
                SchoolsCheckboxes = schools
            };
            TempData["SchoolIds"] = schoolIds;// vm.SchoolsCheckboxes;
            return View(vm);
        }

        // POST: Bundles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BundlesViewModel vm)
        {
            if (ModelState.IsValid)
            {
                int index = 0;
                IList<int> schoolIds = (List<int>)TempData["SchoolIds"]; //List of school ids that existed when the form was created
                IList<int> checkedSchools = new List<int>();
                var labels = db.Labels.ToList();
                int numSelected = vm.SchoolsCheckboxes.Where(x => x).Count();
                //if (labels.Count < numSelected) //Not enough labels for the number of item types selected
                   // return null;
                foreach (bool isChecked in vm.SchoolsCheckboxes)
                {
                    if (isChecked)
                        checkedSchools.Add(schoolIds[index]);
                    index++;
                }
                TempData["SchoolIds"] = null;
                TempData["CheckedSchools"] = checkedSchools;// vm.SchoolsCheckboxes;
                TempData["BundleName"] = vm.BundleName;
                return RedirectToAction("ItemTypesSelect", new RouteValueDictionary(new { action = "ItemTypesSelect" }));
            }

            return View();
        }
        public ActionResult ItemTypesSelect()
        {
            IList<bool> itemTypes = new List<bool>(); //For all the checkboxes
            IList<int> itemTypesIds = new List<int>();//Store the ItemTypeIds in case someone else deletes one
            foreach (var itemType in db.ItemTypes)
            {
                itemTypes.Add(false); //Default to true (checked)
                itemTypesIds.Add(itemType.ItemTypeId);
            }
            BundlesViewModel vm = new BundlesViewModel
            {
                //SchoolsCheckboxes = (IList<bool>)TempData["CheckedSchools"],
                SelectedSchoolIds = (IList<int>)TempData["CheckedSchools"],
                BundleName = (string)TempData["BundleName"],
                ItemTypesCheckboxes = itemTypes,
                ItemTypes = db.ItemTypes.ToList()
            };
            TempData["ItemTypeIds"] = itemTypesIds;
            TempData["CheckedSchools"] = null;
            TempData["BundleName"] = null;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemTypesSubmit(BundlesViewModel vm)
        {
            var itemTypesIds = (IList<int>)TempData["ItemTypeIds"];
            var itemTypes = db.ItemTypes.ToList();
            var labels = db.Labels.ToList();
            List<Bundles> tempBundle = new List<Bundles>(); //Temporarily hold new bundles
            IList<Items> tempItems = new List<Items>();    //Temporarily hold new items
            int numSchools = vm.SelectedSchoolIds.Count;
            //For Every Selected School
            foreach (int schoolId in vm.SelectedSchoolIds)
            {
                bool canCreate = true;
                //For Eveery Selected Item Type
                for (int i = 0; i < vm.ItemTypesCheckboxes.Count; i++)
                {
                    if (vm.ItemTypesCheckboxes[i])
                    {
                        var available = itemTypes[i].Item.Where(x => x.CheckedInById == null); //Can't be checked in
                        available = available.Where(x => x.CheckedOutById == null);            //checked out
                        available = available.Where(x => x.BundleId == null);                  //or assigned to a bundle
                        var availableItems = available.ToList();
                        int numAvailable = availableItems.Count();
                        //Make sure there are enough items available for the number of schools
                        if (numAvailable >= numSchools)
                        {
                            for (int j = 0; j < numSchools; j++)
                            {
                                tempItems.Add(availableItems[j]);
                            }
                        }
                        //Stop The Process
                        else
                        {
                            canCreate = false; //Not enough items to match school demand
                            break;
                        }
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
