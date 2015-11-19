using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InventoryManagement.Database;
using System.Web.Routing;

namespace InventoryManagement.Controllers
{
    public class ItemTypesController : Controller
    {
        private ItemContext db = new ItemContext();

        // GET: ItemTypes
        public ActionResult Index()
        {
            if(Session["isAdmin"]!=null && (bool)Session["isAdmin"])
                return View(db.ItemTypes.ToList());
            //return RedirectToAction("Index", new { controller = "Login", action = "Index" });
            //return RedirectToAction("Lockout");
            return View("No Access");
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
            IQueryable<Items> query  = from item in db.Items
                        where item.ItemTypeId == itemTypes.ItemTypeId
                        select item;
            Items items = db.Items.Find(id);
            ViewBag.Items = items;
            var itemTypeModel = itemTypes;
            var itemModel = items;
           
            return View(itemTypes);
        }
        public ActionResult test()
        {
            return PartialView();
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
            //Find the Item Type
            ItemTypes itemTypes = db.ItemTypes.Find(id);
            if (itemTypes == null)
            {
                return HttpNotFound();
            }
            var itemTypeModel = itemTypes;
            //Create List of Inventory Locations
            IList<SelectListItem> inventoryLocations = db.InventoryLocations.Select(x => new SelectListItem
            {
                Text = x.InventoryLocationName,
                Value = x.InventoryLocationId.ToString()
            }).ToList();
            //Create List of Labels 
            IList<SelectListItem> labels = db.Labels.Select(x => new SelectListItem
            {
                Text = x.LabelName,
                Value = x.LabelId.ToString()
            }).ToList();
            //Insert Empty value for no location
            inventoryLocations.Insert(0, new SelectListItem { Text = "", Value = null });
            labels.Insert(0, new SelectListItem { Text = "", Value = null });
            //Create List of Lists of SelectListItems (1 for every Item)
            IList<IEnumerable<SelectListItem>> invLocSelectLists = new List<IEnumerable<SelectListItem>>();
            IList<IEnumerable<SelectListItem>> labelSelectLists = new List<IEnumerable<SelectListItem>>();
            for (int i= 0; i < itemTypeModel.Item.Count; i++)
            {
                var currId = itemTypeModel.Item[i].InventoryLocationId;         //Current Id
                IList<SelectListItem> currList = new List<SelectListItem>();    //Create Copy of list
                foreach (var item in inventoryLocations)
                    currList.Add(new SelectListItem {Text= item.Text, Value = item.Value });
                var currLabelId = itemTypeModel.Item[i].LabelId;
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
                ItemTypeModel = itemTypeModel,
                InventoryLocations = invLocSelectLists,
                Labels = labelSelectLists
            };
            return View(vm);
        }

        // POST: ItemTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ItemTypeId,ItemName,Quantity")] ItemTypes itemTypes)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(itemTypes).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(itemTypes);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ItemTypesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(viewModel.ItemTypeModel).State = EntityState.Modified;
                db.SaveChanges();
                if (viewModel.ItemTypeModel.Item != null)
                    for(int i = 0; i<viewModel.ItemTypeModel.Item.Count; i++)
                    {
                        db.Entry((viewModel.ItemTypeModel.Item[i])).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                return RedirectToAction("Index");
            }
            return null;
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
        public ActionResult DeleteItem(int? itemId)
        {
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
            ItemTypes itemTypes = db.ItemTypes.Find(id);
            db.ItemTypes.Remove(itemTypes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult CreatePackages()
        {
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
