﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EFTest.Database
{
    public class Items
    {
        [Key]
        public int ItemId { get; set; }
        public int ItemTypeId { get; set; }
        public int? InventoryLocationId { get; set; }
        public int? SchoolId { get; set; }

        //public virtual ItemTypes ItemType { get; set; }
        public virtual InventoryLocations InventoryLocation { get; set; }
        public virtual Schools School{ get; set; }
    }
}