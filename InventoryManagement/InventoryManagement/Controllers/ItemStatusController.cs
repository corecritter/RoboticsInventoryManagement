using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Models;
using System.Data.Entity;
using System.Web.UI.WebControls;

namespace InventoryManagement.Controllers
{
    public class ItemStatusController : Controller
    {
        private ItemContext db = new ItemContext();
        public ActionResult Index()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (TempData["error"] != null)
            {
                ModelState.AddModelError("", (string)TempData["error"]);
            }
            return View();
        }

        public ActionResult NoLabel()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            var noLabelItems = db.Items.Where(item => item.ItemType.HasLabel);
            noLabelItems = noLabelItems.Where(item => item.LabelId == null).OrderBy(item => item.ItemType.ItemName);
            if (noLabelItems.ToList().Count == 0)
            {
                TempData["error"] = "No items exist without a label";
                return RedirectToAction("Index");
            }
            IList<SelectListItem> labels = db.Labels.Select(x => new SelectListItem
            {
                Text = x.LabelName,
                Value = x.LabelId.ToString()
            }).ToList();
            labels.Insert(0, new SelectListItem { Text = "", Value = null });
            IList<IEnumerable<SelectListItem>> labelSelectLists = new List<IEnumerable<SelectListItem>>();
            foreach (var item in noLabelItems)
            {
                IList<SelectListItem> currLabelList = new List<SelectListItem>();    //Create Copy of list
                foreach (var label in labels)
                    currLabelList.Add(new SelectListItem { Text = label.Text, Value = label.Value });
                labelSelectLists.Add(currLabelList);
            }
            ItemsMissingLabelViewModel vm = new ItemsMissingLabelViewModel
            {
                ItemsMissingLabel = noLabelItems.ToList(),
                Labels = labelSelectLists
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitLabels(ItemsMissingLabelViewModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null)
                return RedirectToAction("Index");
            for (int i = 0; i < vm.ItemsMissingLabel.Count; i++)
            {
                var dbItem = db.Items.Find(vm.ItemsMissingLabel[i].ItemId);
                if (dbItem == null)
                {
                    TempData["error"] = "Could not find item";
                    return RedirectToAction("Index");
                }
                dbItem.LabelId = vm.ItemsMissingLabel[i].LabelId;
                db.Entry(dbItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult NoInventoryLocation()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            var noInventoryLocationItems = db.Items.Where(item => item.InventoryLocationId == null);
            if (noInventoryLocationItems.ToList().Count == 0)
            {
                TempData["error"] = "No items exist without an inventory location";
                return RedirectToAction("Index");
            }
            IList<SelectListItem> inventoryLocations = db.InventoryLocations.Select(x => new SelectListItem
            {
                Text = x.InventoryLocationName,
                Value = x.InventoryLocationId.ToString()
            }).ToList();
            inventoryLocations.Insert(0, new SelectListItem { Text = "", Value = null });
            IList<IEnumerable<SelectListItem>> InvLocSelectLists = new List<IEnumerable<SelectListItem>>();
            foreach (var item in noInventoryLocationItems)
            {
                IList<SelectListItem> currLabelList = new List<SelectListItem>();    //Create Copy of list
                foreach (var location in inventoryLocations)
                    currLabelList.Add(new SelectListItem { Text = location.Text, Value = location.Value });
                InvLocSelectLists.Add(currLabelList);
            }
            ItemsMissingInventoryLocationViewModel vm = new ItemsMissingInventoryLocationViewModel
            {
                ItemsMissingLocation = noInventoryLocationItems.OrderBy(item => item.ItemType.ItemName).ToList(),
                InventoryLocations = InvLocSelectLists
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitInventoryLocations(ItemsMissingInventoryLocationViewModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null)
                return RedirectToAction("Index");
            for (int i = 0; i < vm.ItemsMissingLocation.Count; i++)
            {
                var dbItem = db.Items.Find(vm.ItemsMissingLocation[i].ItemId);
                if (dbItem == null)
                {
                    TempData["error"] = "Could not find item";
                    return RedirectToAction("Index");
                }
                dbItem.InventoryLocationId = vm.ItemsMissingLocation[i].InventoryLocationId;
                db.Entry(dbItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult ItemsOut()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            var allItems = db.Items.Where(item => item.CheckedOutById != null);         //Items checked out or returned
            var checkedOutItems = allItems.Where(item => item.CheckedInById == null).OrderBy(item => item.ItemType.ItemName).ThenBy(item => item.Label!=null? item.Label.LabelName : "");  //Items checked out
            if (checkedOutItems.ToList().Count == 0)
            {
                TempData["error"] = "No items are currently checked out";
                return RedirectToAction("Index");
            }


            IList<SelectListItem> inventoryLocations = db.InventoryLocations.Select(x => new SelectListItem
            {
                Text = x.InventoryLocationName,
                Value = x.InventoryLocationId.ToString()
            }).OrderBy(listItem => listItem.Text).ToList();



            IList<bool> lostItems = new List<bool>();
            IList<bool> returnedItems = new List<bool>();
            IList<string> schoolDisplayStrings = new List<string>();
            foreach (var item in checkedOutItems)
            {
                int checkedOutId = (int)item.CheckedOutSchoolId;
                var school = db.Schools.Where(schoolMatch => schoolMatch.SchoolId == checkedOutId).ToList();
                if (school.Count>0)
                    schoolDisplayStrings.Add(school[0].SchoolName);
                lostItems.Add(false);
                returnedItems.Add(false);
            }
            ItemsOutViewModel vm = new ItemsOutViewModel
            {
                CheckedOutItems = checkedOutItems.ToList(),
                SchoolDisplayStrings = schoolDisplayStrings,
                ItemsLost = lostItems,
                ItemReturn = returnedItems
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemsOut(ItemsOutViewModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null || vm.ItemsLost == null || vm.ItemReturn == null)
                return RedirectToAction("Index");

            if(!CheckReturnedAndLost(vm.CheckedOutItems, vm.ItemsLost, vm.ItemReturn))
            {
                TempData["error"] = "An item cannot be marked as lost and returned";
                return RedirectToAction("Index");
            }
            if(!UpdateItems(vm.CheckedOutItems, vm.ItemsLost, vm.ItemReturn))
            {
                TempData["error"] = "Item not found, cannot continue";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        
        public ActionResult NotReturned()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            var allitems = db.Items.Where(item => item.CheckedOutById != null);
            allitems = allitems.Where(item => item.CheckedInById != null);
            allitems = allitems.Where(item => item.IsReturned == false);
            var notReturnedItems = allitems.OrderBy(item => item.ItemType.ItemName).ThenBy(item => item.InventoryLocation.InventoryLocationName).ThenBy(item => item.Label!=null? item.Label.LabelName : "").ToList();
            if (notReturnedItems.Count == 0)
            {
                TempData["error"] = "No items are currently checked in and not returned";
                return RedirectToAction("Index");
            }
            IList<bool> lostItem = new List<bool>();
            IList<bool> returnedItems = new List<bool>();
            for (int i=0; i< notReturnedItems.Count; i++)
            {
                lostItem.Add(false);
                returnedItems.Add(false);
            }
            ItemsNotReturnedViewModel vm = new ItemsNotReturnedViewModel
            {
                ItemsNotReturned = notReturnedItems,
                ItemsLost = lostItem,
                ItemReturn = returnedItems
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitNotReturned(ItemsNotReturnedViewModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null || vm.ItemsLost == null || vm.ItemReturn == null)
                return RedirectToAction("Index");
            if (!CheckReturnedAndLost(vm.ItemsNotReturned, vm.ItemsLost, vm.ItemReturn))
            {
                TempData["error"] = "An item cannot be marked as lost and returned";
                return RedirectToAction("Index");
            }
            if (!UpdateItems(vm.ItemsNotReturned, vm.ItemsLost, vm.ItemReturn))
            {
                TempData["error"] = "Item not found, cannot continue";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult ItemApproval()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            var allItems = db.Items.Where(item => item.CheckedOutById != null);         //Items checked out or returned
            var pendingApprovalItems = allItems.Where(item => item.CheckedInById != null); //Items checked out and in
            pendingApprovalItems = pendingApprovalItems.Where(item => item.IsReturned == true);
            if (pendingApprovalItems.ToList().Count == 0)
            {
                TempData["error"] = "No items currently pending approval";
                return RedirectToAction("Index");
            }
            IList<bool> itemsLost = new List<bool>();
            IList<bool> itemReturn = new List<bool>();

            foreach (var item in pendingApprovalItems)
            {
                itemReturn.Add(true);
                itemsLost.Add(false);
            }
            ItemsApproveViewModel vm = new ItemsApproveViewModel
            {
                PendingApprovalItems = pendingApprovalItems.OrderBy(item => item.InventoryLocation.InventoryLocationName).ThenBy(item => item.ItemType.ItemName).ThenBy(item => item.Label!=null? item.Label.LabelName : "").ToList(),
                ItemReturn = itemReturn,
                ItemsLost = itemsLost
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveItems(ItemsApproveViewModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null)
                return RedirectToAction("Index");

            if (!CheckReturnedAndLost(vm.PendingApprovalItems, vm.ItemsLost, vm.ItemReturn))
            {
                TempData["error"] = "An item cannot be marked as lost and returned";
                return RedirectToAction("Index");
            }
            if (!UpdateItems(vm.PendingApprovalItems, vm.ItemsLost, vm.ItemReturn))
            {
                TempData["error"] = "Item not found, cannot continue";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        private bool CheckReturnedAndLost(IList<Items> ItemIds, IList<bool> lost, IList<bool> returned)
        {
            int index = 0;
            foreach (bool isChecked in lost)
            {
                if (isChecked && returned[index])
                    return false;
                index++;
            }
            return true;
        }
        private bool UpdateItems(IList<Items> ItemIds, IList<bool> lost, IList<bool> returned)
        {
            int index = 0;
            foreach (bool isChecked in lost)
            {
                if (isChecked)
                {
                    var itemToRemove = db.Items.Find(ItemIds[index].ItemId);
                    if (itemToRemove == null)
                        return false;
                    db.Items.Remove(itemToRemove);
                    db.SaveChanges();
                }
                index++;
            }
            index = 0;
            foreach (bool isChecked in returned)
            {
                if (isChecked)
                {
                    var itemToReturn = db.Items.Find(ItemIds[index].ItemId);
                    if (itemToReturn == null)
                        return false;
                    itemToReturn.CheckedInById = null;
                    itemToReturn.CheckedOutById = null;
                    itemToReturn.CheckedOutSchoolId = null;
                    itemToReturn.IsReturned = false;
                    db.Entry(itemToReturn).State = EntityState.Modified;
                    db.SaveChanges();
                }
                index++;
            }
            return true;
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