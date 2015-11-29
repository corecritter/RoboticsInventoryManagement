using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class Items
    {
        [Key]
        public int ItemId { get; set; }
        public int ItemTypeId { get; set; }
        public int? InventoryLocationId { get; set; }
        public int? CheckedOutSchoolId { get; set; }
        public int? LabelId { get; set; }
        public int? BundleId { get; set; }
        public string CheckedOutById { get; set; }
        public string CheckedInById { get; set; }
        
        public virtual ItemTypes ItemType { get; set; }
        public virtual InventoryLocations InventoryLocation { get; set; }
        public virtual Schools School{ get; set; }
        public virtual Labels Label { get; set; }
        public virtual Bundles Bundle { get; set; }
    }
}