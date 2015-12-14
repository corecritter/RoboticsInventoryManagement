using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class SchoolBundles
    {
        [Key]
        public int BundleId { get; set; }
        public int SchoolId { get; set; }

        public Bundles Bundle { get; set; }
        public Schools School { get; set; }
    }
}