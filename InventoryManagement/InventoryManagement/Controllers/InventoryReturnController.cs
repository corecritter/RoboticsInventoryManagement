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
    public class InventoryReturnController : Controller
    {
        private ItemContext db = new ItemContext();
        public ActionResult Index()
        {
            if(Session["LoggedUserId"] == null)
                return RedirectToAction("Index", new { Controller = "Login", Action = "Index" });
            var itemsToReturn = GetItemsToReturn((string)Session["LoggedUserId"]);
            IList<ItemTypes> itemTypesToReturn = new List<ItemTypes>();
            IList<string> itemDisplayString = new List<string>();
            IList<string> itemDisplayInventoryLocation = new List<string>();
            IList<string> itemDisplayLabel = new List<string>();
            IList<bool> returnItemCheckBoxes = new List<bool>();
            IList<int> returnItemQuantities = new List<int>();

            for (int i=0; i<itemsToReturn.Count; i++)
            {
                var currItemType = db.ItemTypes.Find(itemsToReturn[i].ItemTypeId);
                if (currItemType == null)
                    return null;
                int index = itemTypesToReturn.IndexOf(currItemType);
                if (index == -1)
                {
                    itemTypesToReturn.Add(currItemType);
                    var itemsOfType = itemsToReturn.Where(item => item.ItemTypeId == currItemType.ItemTypeId).ToList();
                    int currInventoryLocationId = (int)itemsToReturn[i].InventoryLocationId;
                    int? currLabelId = itemsToReturn[i].LabelId;
                    int currCount = 0;
                    int currIndex = 0;
                    foreach(var item in itemsOfType)
                    {
                        currCount++;
                        //Inventory Location Match, Label doesn't match
                        if (item.InventoryLocationId == currInventoryLocationId && item.LabelId != currLabelId)
                        {
                            itemDisplayString.Add(currCount + " x " + currItemType.ItemName);
                            if (currLabelId != null)
                                itemDisplayLabel.Add(db.Labels.Find(currLabelId).LabelName);
                            else
                                itemDisplayLabel.Add("(No Label)");
                            
                            itemDisplayInventoryLocation.Add(db.InventoryLocations.Find(currInventoryLocationId).InventoryLocationName);
                            returnItemCheckBoxes.Add(false);
                            returnItemQuantities.Add(currCount);
                            currLabelId = item.LabelId;
                            currCount = 0;
                        }
                        //Inventory Location doesn't match, label matches
                        else if (item.InventoryLocationId != currInventoryLocationId && item.LabelId == currLabelId)
                        {
                            itemDisplayString.Add(currCount + " x " + currItemType.ItemName);
                            if (currLabelId != null)
                                itemDisplayLabel.Add(db.Labels.Find(currLabelId).LabelName);
                            else
                                itemDisplayLabel.Add("(No Label)");
                            
                            itemDisplayInventoryLocation.Add(db.InventoryLocations.Find(currInventoryLocationId).InventoryLocationName);
                            returnItemCheckBoxes.Add(false);
                            returnItemQuantities.Add(currCount);
                            currInventoryLocationId = (int)item.InventoryLocationId;
                            currCount = 0;
                        }
                        //Inventory Location and Label Don't match
                        else if (item.InventoryLocationId != currInventoryLocationId && item.LabelId != currLabelId)
                        {
                            itemDisplayString.Add(currCount + " x " + currItemType.ItemName);
                            if (currLabelId != null)
                                itemDisplayLabel.Add(db.Labels.Find(currLabelId).LabelName);
                            else
                                itemDisplayLabel.Add("(No Label)");
                            
                            itemDisplayInventoryLocation.Add(db.InventoryLocations.Find(currInventoryLocationId).InventoryLocationName);
                            returnItemCheckBoxes.Add(false);
                            returnItemQuantities.Add(currCount);
                            currInventoryLocationId = (int)item.InventoryLocationId;
                            currLabelId = item.LabelId;
                            currCount = 0;
                        }
                        currIndex++;
                    }
                    if (currCount > 0)
                    {
                        itemDisplayString.Add(currCount + " x " + currItemType.ItemName);
                        if (currLabelId != null)
                            itemDisplayLabel.Add(db.Labels.Find(currLabelId).LabelName);
                        else
                            itemDisplayLabel.Add("(No Label)");
                        itemDisplayInventoryLocation.Add(db.InventoryLocations.Find(currInventoryLocationId).InventoryLocationName);
                        returnItemCheckBoxes.Add(false);
                        returnItemQuantities.Add(currCount);
                    }
                }
            }

            InventoryReturnIndexViewModel vm = new InventoryReturnIndexViewModel
            {
                ItemsToReturn = itemsToReturn,
                ItemDisplayString = itemDisplayString,
                ItemDisplayInventoryLocation = itemDisplayInventoryLocation,
                ItemDisplayLabelString = itemDisplayLabel,
                ItemReturnCheckBoxes = returnItemCheckBoxes,
                ItemReturnQuantities = returnItemQuantities
            };
            return View(vm);
        }
        private IList<Items> GetItemsToReturn(string userName)
        {
            var itemsToReturn = db.Items.Where(item => item.CheckedInById == userName);
            itemsToReturn = itemsToReturn.Where(item => item.IsReturned == false);
            return (itemsToReturn.OrderBy(item => item.ItemType.ItemName).ToList());
        }
        public ActionResult ReturnItems(InventoryReturnIndexViewModel vm)
        {
            if (vm == null)
                return RedirectToAction("Index");
            int currQuantityIndex = 0;
            int currItemIndex = 0;
            foreach(bool isSelected in vm.ItemReturnCheckBoxes)
            {
                if (isSelected)
                {
                    int numReturning = vm.ItemReturnQuantities[currQuantityIndex];
                    for (int i = 0; i < numReturning; i++)
                    {
                        var currItem = db.Items.Find(vm.ItemsToReturn[currItemIndex].ItemId);
                        if (currItem == null)
                            return RedirectToAction("Index");
                        currItemIndex++;
                        currItem.IsReturned = true;
                        db.Entry(currItem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                    currItemIndex += vm.ItemReturnQuantities[currQuantityIndex];
                currQuantityIndex++;
            }
            return RedirectToAction("Index");
        }

    }
}