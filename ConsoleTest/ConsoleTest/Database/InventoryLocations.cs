using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsoleTest.Database
{
    public class InventoryLocations
    {
        public int InventoryLocationId { get; set; }
        public string InventoryLocationName { get; set; }

        public virtual List<Items> Items { get; set; }
    }
}