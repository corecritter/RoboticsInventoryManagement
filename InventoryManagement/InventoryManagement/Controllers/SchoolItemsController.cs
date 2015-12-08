using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Models;

namespace InventoryManagement.Controllers
{
    public class SchoolItemsController : Controller
    {
        private ItemContext db = new ItemContext();
        public ActionResult Index()
        {
            SchoolsItemsIndexViewModel vm = new SchoolsItemsIndexViewModel
            {
                Schools = db.Schools.OrderBy(school => school.SchoolName).ToList()
            };
            return View(vm);
        }
        public ActionResult ViewItems(int? id)
        {
            if(id==null)
                return RedirectToAction("Index");

            var selectedSchool = db.Schools.Find(id);
            if (selectedSchool == null)
                return RedirectToAction("Index");
            var rentedItems = db.Items.Where(item => item.CheckedOutSchoolId == id);
            rentedItems = rentedItems.Where(item => item.CheckedOutById != null);
            rentedItems = rentedItems.Where(item => item.CheckedInById == null);
            SchoolsItemsViewModel vm = new SchoolsItemsViewModel
            {
                CheckedOutItems = rentedItems.OrderBy(item => item.ItemType.ItemName).ToList()
            };
            return View(vm);
        }
    }
}