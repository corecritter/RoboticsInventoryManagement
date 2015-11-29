using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InventoryManagement.Database;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace InventoryManagement.Models
{
    public class ItemTypesViewModel
    {
        public ItemTypes ItemTypeModel { get; set; }

        [Display(Name = "Inventory Location")]
        public  IList<IEnumerable<SelectListItem>> InventoryLocations{ get; set; }

        [Display(Name = "Label")]
        public IList<IEnumerable<SelectListItem>> Labels { get; set; }
    }
}