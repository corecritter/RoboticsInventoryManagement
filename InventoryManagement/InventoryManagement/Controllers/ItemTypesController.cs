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
    public class ItemTypesController : Controller
    {
        private ItemContext db = new ItemContext();

        // GET: ItemTypes
        public ActionResult Index()
        {
            return View(db.ItemTypes.ToList());
        }

        // GET: ItemTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemTypes itemTypes = db.ItemTypes.Find(id);
            if (itemTypes == null)
            {
                return HttpNotFound();
            }
            return View(itemTypes);
        }

        // GET: ItemTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemTypeId,ItemName,Quantity")] ItemTypes itemTypes)
        {
            if (ModelState.IsValid)
            {
                db.ItemTypes.Add(itemTypes);
                db.SaveChanges();
                for (int i= 0; i < itemTypes.Quantity; i++)
                {
                    var newItem = new Items { ItemTypeId = itemTypes.ItemTypeId };
                    db.Items.Add(newItem);
                    db.SaveChanges();
                }
                IQueryable < InventoryManagement.Database.Items > query =  from item in db.Items
                            where item.ItemTypeId == itemTypes.ItemTypeId
                            select item;

                //return View(query.ToList());
                return RedirectToAction("Index");
            }

            return View(itemTypes);
        }

        // GET: ItemTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemTypes itemTypes = db.ItemTypes.Find(id);
            if (itemTypes == null)
            {
                return HttpNotFound();
            }
            return View(itemTypes);
        }

        // POST: ItemTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemTypeId,ItemName,Quantity")] ItemTypes itemTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(itemTypes);
        }

        // GET: ItemTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemTypes itemTypes = db.ItemTypes.Find(id);
            if (itemTypes == null)
            {
                return HttpNotFound();
            }
            return View(itemTypes);
        }

        // POST: ItemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemTypes itemTypes = db.ItemTypes.Find(id);
            db.ItemTypes.Remove(itemTypes);
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
