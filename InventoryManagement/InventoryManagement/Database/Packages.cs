using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class Packages
    {
        [Key]
        public int PackageId { get; set; } //PK
        [Required(ErrorMessage = "A Package Name is Required")]
        public string PackageName { get; set; }
        public virtual List<Items> Item { get; set; }
    }
}