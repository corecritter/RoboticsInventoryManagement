using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Database
{
    public class InventoryLocations
    {
        [Key]
        public int InventoryLocationId { get; set; }
        [Display(Name = "Inventory Location Name")]
        public string InventoryLocationName { get; set; }

        public virtual List<Items> Items { get; set; }
    }
}