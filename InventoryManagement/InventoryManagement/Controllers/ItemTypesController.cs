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
using System.Web.Routing;

namespace InventoryManagement.Controllers
{
    public class ItemTypesController : Controller
    {
        private ItemContext db = new ItemContext();

        // GET: ItemTypes
        public ActionResult Index()
        {
            if(Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            //if (Session["isAdmin"] != null && (bool)Session["isAdmin"])
            //{
                var itemTypes = db.ItemTypes.OrderBy(item => item.ItemName).ToList();
                IList<int> itemTypesQuantities = new List<int>();
                foreach (var itemType in itemTypes)
                    itemTypesQuantities.Add(itemType.Item.Count);
                ItemTypesIndexViewModel vm = new ItemTypesIndexViewModel
                {
                    ItemTypesModel = itemTypes,
                    ItemQuantities = itemTypesQuantities
                };
                return View(vm);
        }

        // GET: ItemTypes/Create
        public ActionResult Create()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            IList<SelectListItem> inventoryLocations = db.InventoryLocations.Select(x => new SelectListItem
            {
                Text = x.InventoryLocationName,
                Value = x.InventoryLocationId.ToString()
            }).OrderBy(listItem => listItem.Text).ToList();
            inventoryLocations.Insert(0, new SelectListItem { Text = "", Value = null });
            ItemTypesQuantityModel vm = new ItemTypesQuantityModel
            {
                ItemType = new ItemTypes { HasLabel = true },
                Quantity = 0,
                InventoryLocations = inventoryLocations
            };
            return View(vm);
        }

        // POST: ItemTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemTypesQuantityModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (vm == null)
                return RedirectToAction("Index");
            int? invLocId = vm.SelectedInventoryLocation;
            var invLocation = db.InventoryLocations.Find(invLocId);
            db.ItemTypes.Add(vm.ItemType);
            db.SaveChanges();
            for (int i= 0; i < vm.Quantity; i++)
            {
                var newItem = new Items { ItemTypeId = vm.ItemType.ItemTypeId};
                if (invLocation != null)
                    newItem.InventoryLocationId = invLocId;
                db.Items.Add(newItem);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Add(int id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            var itemType = db.ItemTypes.Find(id);
            if (itemType == null)
                return RedirectToAction("Index");
            IList<SelectListItem> inventoryLocations = db.InventoryLocations.Select(x => new SelectListItem
            {
                Text = x.InventoryLocationName,
                Value = x.InventoryLocationId.ToString()
            }).OrderBy(listItem => listItem.Text).ToList();
            inventoryLocations.Insert(0, new SelectListItem { Text = "", Value = null });
            ItemTypesQuantityModel vm = new ItemTypesQuantityModel
            {
                ItemType = itemType,
                Quantity = 1,
                InventoryLocations = inventoryLocations
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItemQuantity(ItemTypesQuantityModel vm)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            //if (ModelState.IsValid)
            //{
            var itemType = db.ItemTypes.Find(vm.ItemType.ItemTypeId);
            if (itemType == null)
                return RedirectToAction("Index");
            int? invLocId = vm.SelectedInventoryLocation;
            var invLocation = db.InventoryLocations.Find(invLocId);
            for (int i=0; i<vm.Quantity; i++)
            {
                var item = new Items { ItemTypeId = itemType.ItemTypeId };
                if (invLocation != null)
                    item.InventoryLocationId = invLocId;
                db.Items.Add(item);
                db.SaveChanges();
            }
            //}
            return RedirectToAction("Index");
        }

        // GET: ItemTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Find the Item Type
            ItemTypes itemTypes = db.ItemTypes.Find(id);
            if (itemTypes == null)
            {
                return HttpNotFound();
            }
            var itemsModel = itemTypes.Item.OrderBy(item =>  item.InventoryLocation != null ? item.InventoryLocation.InventoryLocationName : "").ThenBy(item => item.Label !=null ? item.Label.LabelName : "").ToList();
            //Create List of Inventory Locations
            IList<SelectListItem> inventoryLocations = db.InventoryLocations.Select(x => new SelectListItem
            {
                Text = x.InventoryLocationName,
                Value = x.InventoryLocationId.ToString()
            }).OrderBy(listItem => listItem.Text).ToList();
            //Create List of Labels 
            IList<SelectListItem> labels = db.Labels.Select(x => new SelectListItem
            {
                Text = x.LabelName,
                Value = x.LabelId.ToString()
            }).OrderBy(listItem => listItem.Text).ToList();
            //Insert Empty value for no location
            inventoryLocations.Insert(0, new SelectListItem { Text = "", Value = null });
            labels.Insert(0, new SelectListItem { Text = "", Value = null });
            //Create List of Lists of SelectListItems (1 for every Item)
            IList<IEnumerable<SelectListItem>> invLocSelectLists = new List<IEnumerable<SelectListItem>>();
            IList<IEnumerable<SelectListItem>> labelSelectLists = new List<IEnumerable<SelectListItem>>();
            for (int i= 0; i < itemsModel.Count; i++)
            {
                var currId = itemsModel[i].InventoryLocationId;         //Current Id
                IList<SelectListItem> currList = new List<SelectListItem>();    //Create Copy of list
                foreach (var item in inventoryLocations)
                    currList.Add(new SelectListItem {Text= item.Text, Value = item.Value });
                var currLabelId = itemsModel[i].LabelId;
                IList<SelectListItem> currLabelList = new List<SelectListItem>();    //Create Copy of list
                foreach (var item in labels)
                    currLabelList.Add(new SelectListItem { Text = item.Text, Value = item.Value });


                //Find the index to be selected
                if (currId == null) 
                    currList[0].Selected = true;
                else
                    for (int j = 0; j < currList.Count; j++)
                        if (currId.ToString().Equals(currList[j].Value))
                        {
                            currList[j].Selected = true;
                            break;
                        }
                if(currLabelId == null)
                    currLabelList[0].Selected = true;
                else
                    for (int j = 0; j < currLabelList.Count; j++)
                        if (currLabelId.ToString().Equals(currLabelList[j].Value))
                        {
                            currLabelList[j].Selected = true;
                            break;
                        }

                invLocSelectLists.Add(currList); //Add to the collection
                labelSelectLists.Add(currLabelList);
            }

            ItemTypesViewModel vm = new ItemTypesViewModel
            {
                ItemTypeModel = itemTypes,
                ItemsModel = itemsModel,
                InventoryLocations = invLocSelectLists,
                Labels = labelSelectLists
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ItemTypesViewModel viewModel)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (ModelState.IsValid)
            {
                db.Entry(viewModel.ItemTypeModel).State = EntityState.Modified;
                db.SaveChanges();
                if (viewModel.ItemsModel != null)
                    for(int i = 0; i<viewModel.ItemsModel.Count; i++)
                    {
                        var dbItem = db.Items.Find(viewModel.ItemsModel[i].ItemId);
                        if (dbItem == null)
                            return RedirectToAction("Index");
                        dbItem.InventoryLocationId = viewModel.ItemsModel[i].InventoryLocationId;
                        dbItem.LabelId = viewModel.ItemsModel[i].LabelId;
                        db.Entry(dbItem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                return RedirectToAction("Index");
            }
            return null;
        }

        // GET: ItemTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
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

        public ActionResult DeleteItem(int? itemId)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (itemId == null)
            {
                return HttpNotFound();
            }
            Items item = db.Items.Find(itemId);
            int itemTypeId = item.ItemTypeId;
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Edit", new RouteValueDictionary(new { action = "Edit", id = itemTypeId}));
        }

        public ActionResult AddItem(int? itemTypeId)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            if (itemTypeId == null)
            {
                return HttpNotFound();
            }
            ItemTypes itemType = db.ItemTypes.Find(itemTypeId);
            if(itemType != null)
            {
                var newItem = new Items { ItemTypeId = (int)itemTypeId};
                db.Items.Add(newItem);
                db.SaveChanges();
            }
            return RedirectToAction("Edit", new RouteValueDictionary(new { action = "Edit", id = itemTypeId }));
        }

        // POST: ItemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            ItemTypes itemTypes = db.ItemTypes.Find(id);
            if (itemTypes == null)
                return RedirectToAction("Index");
            for(int i=0; i< itemTypes.Item.Count; i++)
            {
                var item = db.Items.Find(itemTypes.Item[i].ItemId);
                if (item == null)
                    return RedirectToAction("Index");
                db.Items.Remove(item);
                db.SaveChanges();
            }
            db.ItemTypes.Remove(itemTypes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreatePackages()
        {
            if (Session["isAdmin"] == null || !(bool)Session["isAdmin"])
                return RedirectToAction("Index", new { controller = "Home", action = "Index" });
            return RedirectToAction("Index", new { controller = "Bundles", action = "Index" });
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
