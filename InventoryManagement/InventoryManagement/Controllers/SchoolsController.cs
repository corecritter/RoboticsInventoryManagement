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
    public class SchoolsController : Controller
    {
        private SchoolsContext db = new SchoolsContext();

        // GET: Schools
        public ActionResult Index()
        {
            return View(db.Schools.ToList());
        }

        // GET: Schools/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schools schools = db.Schools.Find(id);
            if (schools == null)
            {
                return HttpNotFound();
            }
            return View(schools);
        }

        // GET: Schools/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Schools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SchoolId,SchoolName,TeacherName,Email,Phone")] Schools schools)
        {
            if (ModelState.IsValid)
            {
                db.Schools.Add(schools);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(schools);
        }

        // GET: Schools/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schools schools = db.Schools.Find(id);
            if (schools == null)
            {
                return HttpNotFound();
            }
            return View(schools);
        }

        // POST: Schools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SchoolId,SchoolName,TeacherName,Email,Phone")] Schools schools)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schools).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(schools);
        }

        // GET: Schools/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schools schools = db.Schools.Find(id);
            if (schools == null)
            {
                return HttpNotFound();
            }
            return View(schools);
        }

        // POST: Schools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Schools schools = db.Schools.Find(id);
            db.Schools.Remove(schools);
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
