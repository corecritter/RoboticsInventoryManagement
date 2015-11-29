using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Models;

namespace InventoryManagement.Controllers
{
    public class ItemStatusController : Controller
    {
        private ItemContext db = new ItemContext();
        public ActionResult Index()
        {
            var allItems = db.Items.Where(item => item.CheckedOutById != null);         //Items checked out or returned
            var checkedOutItems = allItems.Where(item => item.CheckedInById == null);  //Items checked out
            var pendingApprovalItems = allItems.Where(item => item.CheckedInById != null); //Items checked out and in
            ItemStatusViewModel vm = new ItemStatusViewModel
            {
                CheckedOutItems = checkedOutItems.ToList(),
                PendingApprovalItems = pendingApprovalItems.ToList()
            };
            return View(vm);
        }
    }
}