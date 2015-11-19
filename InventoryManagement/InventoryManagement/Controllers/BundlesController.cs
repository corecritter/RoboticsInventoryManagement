using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Database;
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
            if (ModelState.IsValid)
            {
                int index = 0;
                IList<int> schoolIds = new List<int>();
                foreach (bool isChecked in vm.SchoolsCheckboxes)
                {
                    if (isChecked)
                        schoolIds.Add(vm.Schools[index].SchoolId);
                    index++;
                }
                TempData["CheckedSchools"] = schoolIds;// vm.SchoolsCheckboxes;
                TempData["BundleName"] = vm.BundleName;
                return RedirectToAction("ItemTypesSelect", new RouteValueDictionary(new { action = "ItemTypesSelect" }));
            }

            return View();
        }
        public ActionResult ItemTypesSelect()
        {
            IList<bool> itemTypes = new List<bool>();
            foreach (var itemType in db.ItemTypes)
            {
                itemTypes.Add(false); //Default to true (checked)
            }
            BundlesViewModel vm = new BundlesViewModel
            {
                //SchoolsCheckboxes = (IList<bool>)TempData["CheckedSchools"],
                SelectedSchoolIds = (IList<int>)TempData["CheckedSchools"],
                BundleName = (string)TempData["BundleName"],
                ItemTypesCheckboxes = itemTypes,
                ItemTypes = db.ItemTypes.ToList()
            };
            TempData["CheckedSchools"] = null;
            TempData["BundleName"] = null;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemTypesSubmit(BundlesViewModel vm)
        {
            var itemTypes = db.ItemTypes.ToList();
            int numSchools = vm.SelectedSchoolIds.Count;
            foreach (int schoolId in vm.SelectedSchoolIds)
                for (int i = 0; i < vm.ItemTypesCheckboxes.Count; i++)
                {
                    if (vm.ItemTypesCheckboxes[i])
                    {
                        if (itemTypes[i].Item.Count >= numSchools)
                        {
                            var available = itemTypes[i].Item.Where(x => x.CheckedInById == null);
                            available = available.Where(x => x.CheckedOutById == null);
                            available = available.Where(x => x.BundleId == null);
                            int numAvailable = available.ToList().Count;
                            if (numAvailable >= numSchools)
                            {
                                var newBundle = new Bundles { BundleName = vm.BundleName };
                                db.Bundles.Add(newBundle);
                                db.SaveChanges();
                                int bundleId = newBundle.BundleId;

                                numSchools--;

                            }
                            else
                                return null; //Not enough items to match school demand
                        }
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
