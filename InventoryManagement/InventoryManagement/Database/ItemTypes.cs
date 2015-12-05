using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class ItemTypes
    {
        [Key]
        public int ItemTypeId { get; set; } //PK
        [Required]
        public string ItemName { get; set; }
        [Display(Name = "Use Item Labels")]
        public bool HasLabel { get; set; }
        public virtual List<Items> Item { get; set; } 
    }
}