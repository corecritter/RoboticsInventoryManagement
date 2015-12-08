using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Models;
using System.Data.Entity;

namespace InventoryManagement.Controllers
{
    public class ItemStatusController : Controller
    {
        private ItemContext db = new ItemContext();
        public ActionResult Index()
        {
            if (TempData["error"] != null)
            {
                ModelState.AddModelError("", (string)TempData["error"]);
            }
            return View();
        }

        public ActionResult NoLabel()
        {
            var noLabelItems = db.Items.Where(item => item.ItemType.HasLabel);
            noLabelItems = noLabelItems.Where(item => item.LabelId == null).OrderBy(item => item.ItemType.ItemName);
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
            if (vm == null)
                return RedirectToAction("Index");
            for (int i = 0; i < vm.ItemsMissingLabel.Count; i++)
            {
                var dbItem = db.Items.Find(vm.ItemsMissingLabel[i].ItemId);
                if (dbItem == null)
                {
                    TempData["error"] = "Could Not Find Item";
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
            var noInventoryLocationItems = db.Items.Where(item => item.InventoryLocationId == null);
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
            if (vm == null)
                return RedirectToAction("Index");
            for (int i = 0; i < vm.ItemsMissingLocation.Count; i++)
            {
                var dbItem = db.Items.Find(vm.ItemsMissingLocation[i].ItemId);
                if (dbItem == null)
                {
                    TempData["error"] = "Could Not Find Item";
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
            var allItems = db.Items.Where(item => item.CheckedOutById != null);         //Items checked out or returned
            var checkedOutItems = allItems.Where(item => item.CheckedInById == null).OrderBy(item => item.ItemType.ItemName);  //Items checked out
            IList<bool> lostItems = new List<bool>();
            foreach (var item in checkedOutItems)
                lostItems.Add(false);
            ItemsOutViewModel vm = new ItemsOutViewModel
            {
                CheckedOutItems = checkedOutItems.ToList(),
                ItemsLost = lostItems
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItems(ItemsOutViewModel vm)
        {
            if (vm == null)
                return RedirectToAction("Index");
            int index = 0;
            foreach(bool isChecked in vm.ItemsLost)
            {
                if (isChecked)
                {
                    var itemToRemove = db.Items.Find(vm.CheckedOutItems[index].ItemId);
                    if (itemToRemove == null)
                        return RedirectToAction("Index");
                    db.Items.Remove(itemToRemove);
                    db.SaveChanges();
                }
                index++;
            }
            return RedirectToAction("Index");
        }
        public ActionResult ItemApproval()
        {
            var allItems = db.Items.Where(item => item.CheckedOutById != null);         //Items checked out or returned
            var pendingApprovalItems = allItems.Where(item => item.CheckedInById != null); //Items checked out and in
            IList<bool> approveItems = new List<bool>();
            foreach (var item in pendingApprovalItems)
                approveItems.Add(true);
            ItemsApproveViewModel vm = new ItemsApproveViewModel
            {
                PendingApprovalItems = pendingApprovalItems.OrderBy(item => item.ItemType.ItemName).ToList(),
                ApproveCheckBoxes = approveItems
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveItems(ItemsApproveViewModel vm)
        {
            int index = 0;
            foreach (bool isChecked in vm.ApproveCheckBoxes)
            {
                if (isChecked)
                {
                    var itemToApprove = db.Items.Find(vm.PendingApprovalItems[index].ItemId);
                    if (itemToApprove == null)
                        return RedirectToAction("Index");
                    itemToApprove.CheckedOutById = null;
                    itemToApprove.CheckedInById = null;
                    itemToApprove.CheckedOutSchoolId = null;
                    itemToApprove.IsReturned = false;
                    db.Entry(itemToApprove).State = EntityState.Modified;
                    db.SaveChanges();
                }
                index++;
            }
            return RedirectToAction("Index");
        }
    }
}