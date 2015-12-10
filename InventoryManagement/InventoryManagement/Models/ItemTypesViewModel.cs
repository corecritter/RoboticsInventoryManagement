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
        public IList<Items> ItemsModel { get; set; }

        [Display(Name = "Inventory Location")]
        public  IList<IEnumerable<SelectListItem>> InventoryLocations{ get; set; }

        [Display(Name = "Label")]
        public IList<IEnumerable<SelectListItem>> Labels { get; set; }
    }

    public class ItemTypesIndexViewModel
    {
        public List<ItemTypes> ItemTypesModel { get; set; }
        public IList<int> ItemQuantities { get; set; }
    }

    public class ItemTypesQuantityModel
    {
        public ItemTypes ItemType { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Inventory Location")]
        public IEnumerable<SelectListItem> InventoryLocations { get; set; }
        public int? SelectedInventoryLocation { get; set; }
    }
}