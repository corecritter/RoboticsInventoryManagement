﻿using System;
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
    public class InventoryLocationsController : Controller
    {
        private InventoryContext db = new InventoryContext();

        // GET: InventoryLocations
        public ActionResult Index()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            return View(db.InventoryLocations.OrderBy(locxation => locxation.InventoryLocationName).ToList());
        }

        // GET: InventoryLocations/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryLocations inventoryLocations = db.InventoryLocations.Find(id);
            if (inventoryLocations == null)
            {
                return HttpNotFound();
            }
            return View(inventoryLocations);
        }

        // GET: InventoryLocations/Create
        public ActionResult Create()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            return View();
        }

        // POST: InventoryLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InventoryLocationId,InventoryLocationName")] InventoryLocations inventoryLocations)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (ModelState.IsValid)
            {
                db.InventoryLocations.Add(inventoryLocations);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inventoryLocations);
        }

        // GET: InventoryLocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryLocations inventoryLocations = db.InventoryLocations.Find(id);
            if (inventoryLocations == null)
            {
                return HttpNotFound();
            }
            return View(inventoryLocations);
        }

        // POST: InventoryLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InventoryLocationId,InventoryLocationName")] InventoryLocations inventoryLocations)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (ModelState.IsValid)
            {
                db.Entry(inventoryLocations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inventoryLocations);
        }

        // GET: InventoryLocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryLocations inventoryLocations = db.InventoryLocations.Find(id);
            if (inventoryLocations == null)
            {
                return HttpNotFound();
            }
            return View(inventoryLocations);
        }

        // POST: InventoryLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            InventoryLocations inventoryLocations = db.InventoryLocations.Find(id);
            db.InventoryLocations.Remove(inventoryLocations);
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
