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
            string userName = (string)Session["LoggedUserId"];
            var itemsToReturn = GetItemsToReturn(userName);
            IList<ItemTypes> itemTypesToReturn = new List<ItemTypes>();
            IList<InventoryLocations> inventoryLocations = new List<InventoryLocations>();
            IList<string> itemDisplayString = new List<string>();
            IList<string> itemDisplayInventoryLocation = new List<string>();
            IList<string> itemDisplayLabel = new List<string>();
            IList<bool> returnItemCheckBoxes = new List<bool>();
            IList<int> returnItemQuantities = new List<int>();

            for (int i=0; i<itemsToReturn.Count; i++)
            {
                var currItemType = db.ItemTypes.Find(itemsToReturn[i].ItemTypeId);
                if (currItemType == null)
                    return RedirectToAction("Index", new { Controller = "CheckOut", Action = "Index" });
                int index = itemTypesToReturn.IndexOf(currItemType);
                if (index == -1)
                {
                    itemTypesToReturn.Add(currItemType);
                    var byLocation = itemsToReturn.Where(item => item.ItemTypeId == currItemType.ItemTypeId).ToList();
                    //var byLocation = itemsOfType.OrderBy(item => item.InventoryLocation.InventoryLocationName).ToList();

                    //var byLabel = itemsOfType.OrderBy(item => item.Label.LabelName);
                    var currLocation = byLocation[0].InventoryLocation;
                    int prevIndex = 0;
                    int currIndex = 0;
                    
                    while(currIndex < byLocation.Count)
                    {
                        //currCount++;
                        if(currLocation.InventoryLocationId != byLocation[currIndex].InventoryLocationId || currIndex == byLocation.Count-1)
                        {
                            int currCount = 0;
                            IList<Items> currSet = new List<Items>();
                            for(int j= prevIndex; j<=currIndex; j++)
                            {
                                currSet.Add(byLocation[j]);
                            }
                            
                            if (currItemType.HasLabel)
                            {
                                //currSet.OrderBy(item => item.LabelId);
                                var currLabel = currSet[0].Label;
                                for (int j = 0; j < currSet.Count; j++)
                                {
                                    currCount++;
                                    if (currLabel.LabelId != currSet[j].LabelId || j == currSet.Count - 1)
                                    {
                                        itemDisplayString.Add(currCount + " x " + currItemType.ItemName);
                                        itemDisplayLabel.Add(db.Labels.Find(currLabel.LabelId).LabelName);
                                        returnItemQuantities.Add(currCount);
                                        returnItemCheckBoxes.Add(false);
                                        currLabel = currSet[j].Label;
                                        currCount = 0;
                                    }
                                }
                            }
                            else
                            {
                                itemDisplayString.Add(currSet.Count + " x " + currItemType.ItemName);
                                itemDisplayLabel.Add("(No Label)");
                                returnItemQuantities.Add(currSet.Count);
                                returnItemCheckBoxes.Add(false);
                            }
                            
                            itemDisplayInventoryLocation.Add(currLocation.InventoryLocationName);
                            currLocation = byLocation[currIndex].InventoryLocation;
                            prevIndex = currIndex+1;
                        }
                        currIndex++;
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
            return (itemsToReturn.OrderBy(item => item.ItemType.ItemName).ThenBy(item => item.InventoryLocation.InventoryLocationName).ThenBy(item => item.Label.LabelName).ToList());
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