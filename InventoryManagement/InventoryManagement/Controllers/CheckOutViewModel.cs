using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InventoryManagement.Database;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace InventoryManagement.Controllers
{
    public class CheckOutViewModel
    {
        public Schools SchoolModel { get; set; }
        //for the index view
        [Display(Name = "Schools")]
        public IList<Schools> Schools { get; set; }

        public Items ItemModel { get; set; }

        //for check out view
        [Display(Name = "Item")]
        public IList<Items> Items { get; set; }
        public IList<bool> ItemsCheckBoxes { get; set; }

        //for check in view
        [Display(Name = "Item")]
        public IList<Items> RenameThis { get; set; }

    }
}