using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboticsInventoryManagement.Database
{
    public class ItemTypes
    {
        public int ItemTypeId { get; set; } //PK
        public string ItemName { get; set; }

        public virtual List<Items> Item { get; set; } 
    }
}