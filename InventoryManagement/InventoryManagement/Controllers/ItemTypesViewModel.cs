using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InventoryManagement.Database;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Controllers
{
    public class ItemTypesViewModel
    {
        public ItemTypes ItemTypeModel { get; set; }
        //public Items ItemModel { get; set; }
        public List<Items> ItemsModel { get; set; }
        [Display(Name = "Inventory Locations")]
        public IEnumerable<SelectListItem> InventoryLocations { get; set; }
        public string selectedValue { get; set; }

        public IEnumerable<SelectListItem> Labels { get; set; }
        public string selectedLabel { get; set; }
    }
}