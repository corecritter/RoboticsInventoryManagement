using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Database;

namespace InventoryManagement.Controllers
{
    public class LabelsController : Controller
    {
        private LabelContext db = new LabelContext();

        // GET: Labels
        public ActionResult Index()
        {
            return View(db.Labels.ToList());
        }

        // GET: Labels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Labels labels = db.Labels.Find(id);
            if (labels == null)
            {
                return HttpNotFound();
            }
            return View(labels);
        }

        // GET: Labels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Labels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LabelId,LabelName")] Labels labels)
        {
            if (ModelState.IsValid)
            {
                db.Labels.Add(labels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(labels);
        }

        // GET: Labels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Labels labels = db.Labels.Find(id);
            if (labels == null)
            {
                return HttpNotFound();
            }
            return View(labels);
        }

        // POST: Labels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LabelId,LabelName")] Labels labels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(labels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(labels);
        }

        // GET: Labels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Labels labels = db.Labels.Find(id);
            if (labels == null)
            {
                return HttpNotFound();
            }
            return View(labels);
        }

        // POST: Labels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Labels labels = db.Labels.Find(id);
            if (labels == null)
                return RedirectToAction("Index");
            var associatedSchools = db.Schools.Where(school => school.LabelId == id).ToList();
            var associatedItems = db.Items.Where(item => item.LabelId == id).ToList();
            var relatedBundles = db.Bundles.Where(bundle => bundle.SchoolId == id).ToList();
            for(int i=0; i< associatedSchools.Count; i++)
            {
                var school = db.Schools.Find(associatedSchools[i].SchoolId);
                if(school==null)
                    return RedirectToAction("Index");
                var relatedItems = db.Items.Where(item => item.CheckedOutSchoolId == school.SchoolId).ToList();
                for(int j=0; j< relatedItems.Count; j++)
                {
                    relatedItems[i].CheckedOutSchoolId = null;
                    db.Entry(relatedItems[i]).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            for (int i = 0; i < relatedBundles.Count; i++)
            {
                var bundle = db.Bundles.Find(relatedBundles[i].BundleId);
                if (bundle == null)
                    return RedirectToAction("Index");
                var relatedItems = db.Items.Where(item => item.BundleId == bundle.BundleId).ToList();
                for (int j = 0; j < relatedItems.Count; j++)
                {
                    relatedItems[i].BundleId = null;
                    db.Entry(relatedItems[i]).State = EntityState.Modified;
                    db.SaveChanges();
                }
                db.Bundles.Remove(bundle);
                db.SaveChanges();
            }
            for (int i = 0; i < associatedItems.Count; i++)
            {
                var item = db.Items.Find(associatedItems[i].ItemId);
                if(item==null)
                    return RedirectToAction("Index");
                associatedItems[i].LabelId = null;
                db.Entry(associatedItems[i]).State = EntityState.Modified;
                db.SaveChanges();
            }
            for (int i = 0; i < associatedSchools.Count; i++)
            {
                var school = db.Schools.Find(associatedSchools[i].SchoolId);
                if (school == null)
                    return RedirectToAction("Index");
                db.Schools.Remove(school);
                db.SaveChanges();
            }
            db.Labels.Remove(labels);
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
