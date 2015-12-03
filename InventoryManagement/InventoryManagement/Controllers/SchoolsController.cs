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

namespace InventoryManagement.Controllers
{
    public class SchoolsController : Controller
    {
        private SchoolsContext db = new SchoolsContext();

        // GET: Schools
        public ActionResult Index()
        {
            return View(db.Schools.ToList().OrderBy(x => x.SchoolName));
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
            SchoolsViewModel vm = new SchoolsViewModel
            {
                SchoolModel = new Schools(),
                Labels = createLabelSelectList()
            };
            return View(vm);
        }

        // POST: Schools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SchoolsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var existingSchool = db.Schools.Where(school => school.SchoolName == vm.SchoolModel.SchoolName);
                var existingLabel = db.Schools.Where(school => school.LabelId == vm.SchoolModel.LabelId);
                if (existingSchool.ToList().Count > 0)
                { 
                    ModelState.AddModelError("", "School Name already exists");
                    vm.Labels = createLabelSelectList();
                    return View(vm);
                }
                else if (existingLabel.ToList().Count > 0)
                {
                    ModelState.AddModelError("", "Another School is already associated with the selected label");
                    vm.Labels = createLabelSelectList();
                    return View(vm);
                }
                db.Schools.Add(vm.SchoolModel);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
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

        private IList<SelectListItem> createLabelSelectList()
        {
            IList<SelectListItem> labels = db.Labels.Select(x => new SelectListItem
            {
                Text = x.LabelName,
                Value = x.LabelId.ToString()
            }).ToList();
            labels.Insert(0, new SelectListItem { Text = "", Value = null });
            return labels;
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
