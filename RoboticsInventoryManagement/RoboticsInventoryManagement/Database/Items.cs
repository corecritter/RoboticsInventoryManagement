using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboticsInventoryManagement.Database
{
    public class Items
    {
        public int ItemId { get; set; }

        public virtual ItemTypes ItemType { get; set; }
    }
}