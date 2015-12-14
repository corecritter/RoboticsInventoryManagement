using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class BundleDetails
    {
        [Key]
        public int BundleId { get; set; }
        public int ItemTypeId { get; set; }
        public int ItemQuantity { get; set; }

        public Bundles Bundle { get; set; }
        public ItemTypes ItemType { get; set; }
    }
}