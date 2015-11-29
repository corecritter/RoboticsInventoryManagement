using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class Bundles
    {
        [Key]
        public int BundleId { get; set; } //PK
        [Required(ErrorMessage = "A Package Name is Required")]
        public string BundleName { get; set; }
        public int SchoolId { get; set; }
        public virtual Schools School { get; set; }
        public virtual List<Items> Items { get; set; }
    }
}