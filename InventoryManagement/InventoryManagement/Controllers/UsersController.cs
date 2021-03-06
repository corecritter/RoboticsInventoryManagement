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
    public class UsersController : Controller
    {
        private UsersContext db = new UsersContext();

        // GET: Users
        public ActionResult Index()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["error"] != null)
            {
                ModelState.AddModelError("", (string)TempData["error"]);
            }
            return View(db.Users.OrderBy(user => user.UserName).ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,FirstName,LastName,Email,Password,PhoneNumber,isAdmin")] Users users)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.Where(user => user.UserName == users.UserName);
                if (existingUser.ToList().Count > 0)
                {
                    ModelState.AddModelError("", "User Name already exists");
                    return View(users);
                }
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserName,FirstName,LastName,Email,Password,PhoneNumber,isAdmin")] Users users)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            Users users = db.Users.Find(id);
            if (users == null)
                return RedirectToAction("Index");

            if (users.isAdmin)
            {
                int numAdmins = db.Users.Where(user => user.isAdmin).ToList().Count;
                if(numAdmins == 1)
                {
                    TempData["error"] = "Must Have at least 1 administrator user";
                    return RedirectToAction("Index");
                }
            }
            var associatedItemsOut = db.Items.Where(item => item.CheckedOutById == users.UserName).ToList();
            var associatedItemsIn = db.Items.Where(item => item.CheckedInById == users.UserName).ToList();
            for(int i=0; i<associatedItemsOut.Count; i++)
            {
                var item = db.Items.Find(associatedItemsOut[i].ItemId);
                if(item==null)
                    return RedirectToAction("Index");
                item.CheckedOutById = "Removed";
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            for (int i = 0; i < associatedItemsIn.Count; i++)
            {
                var item = db.Items.Find(associatedItemsIn[i].ItemId);
                if (item == null)
                    return RedirectToAction("Index");
                item.CheckedInById = "Removed";
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            string userName = (string)Session["LoggedUserId"];
            bool logOff = false;
            if (userName.Equals(users.UserName))
                logOff = true;
            db.Users.Remove(users);
            db.SaveChanges();
            if (logOff)
            {
                Session["LoggedUserName"] = null;
                Session["LoggedUserId"] = null;
                Session["isAdmin"] = null;
            }
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
