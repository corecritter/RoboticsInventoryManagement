using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboticsInventoryManagement.Database
{
    public class Items
    {
        public int ItemId { get; set; }
        public int ItemTypeId { get; set; }
        public int InventoryLocationId { get; set; }

        public virtual ItemTypes ItemType { get; set; }
        public virtual InventoryLocations InventoryLocation { get; set; }
    }
}