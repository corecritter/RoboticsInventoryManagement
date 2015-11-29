using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class CheckInViewModel
    {
        //For Index View (Selecting Items to check in)
        public IList<Items> RentedItems { get; set; }
        public IList<bool> RentedItemsCheckboxes { get; set; }
    }
}