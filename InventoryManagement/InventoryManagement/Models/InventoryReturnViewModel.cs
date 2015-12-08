using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class InventoryReturnIndexViewModel
    {
        public IList<Items> ItemsToReturn { get; set; }
        public IList<string> ItemDisplayString { get; set; }
        public IList<string> ItemDisplayInventoryLocation { get; set; }
        public IList<string> ItemDisplayLabelString { get; set; }
        public IList<bool> ItemReturnCheckBoxes{ get; set; }
        public IList<int> ItemReturnQuantities { get; set; }
    }
}