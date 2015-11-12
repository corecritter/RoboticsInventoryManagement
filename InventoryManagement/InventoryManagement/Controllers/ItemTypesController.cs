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
            IQueryable<Items> query  = from item in db.Items
                        where item.ItemTypeId == itemTypes.ItemTypeId
                        select item;
            Items items = db.Items.Find(id);
            ViewBag.Items = items;
            //ViewData["Items"] = query.ToList<Items>();// query;
            //return View(context.Players.Include(player => player.Team).ToList());
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
            //ViewBag.Items = db.Items.Where(Items => Items.ItemTypeId == itemTypes.ItemTypeId);
            var itemTypeModel = itemTypes;
            var items = db.Items.Where(Items => Items.ItemTypeId == itemTypes.ItemTypeId);
            var itemModel = items.ToList();

            var inventoryLocations = db.InventoryLocations.Select(x => new SelectListItem
            {
                Text = x.InventoryLocationName,
                Value = x.InventoryLocationId.ToString()
            }).ToList();
            inventoryLocations.Add(new SelectListItem { Text = "", Value = null });
            var labels = db.Labels.Select(x => new SelectListItem
            {
                Text = x.LabelName,
                Value = x.LabelId.ToString()
            }).ToList();
            
            ItemTypesViewModel vm = new ItemTypesViewModel
            {
                ItemTypeModel = itemTypeModel,
                ItemsModel = itemModel,
                InventoryLocations = inventoryLocations,
                selectedValue = "0",
                Labels = labels,
                selectedLabel = "0"
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
        //public ActionResult Edit(ItemTypes itemTypes, IEnumerable<Items> items)
        public ActionResult Edit(ItemTypesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(viewModel.ItemTypeModel).State = EntityState.Modified;
                db.SaveChanges();

                //for(int i = 0; i<viewModel.ItemsModel.Count; i++)
                //{
                //    //db.Entry(((Items)viewModel.ItemsModel[i])).State = EntityState.Modified;
                //    //db.Entry(viewModel.ItemsModel[i]).State = EntityState.Modified;
                //    //var item = db.Items.First(x => x.ItemId == viewModel.ItemsModel[i].ItemId);
                //    //db.Items.Attach(viewModel.ItemsModel[i]);
                //    db.Entry(((Items)viewModel.ItemsModel[i])).State = EntityState.Modified;
                //    db.SaveChanges();
                //    string s = "";                    
                //}


                //db.SaveChanges();
                return RedirectToAction("Index");
                
            }
            //return View(vm);
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
