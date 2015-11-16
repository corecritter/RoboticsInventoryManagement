using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Controllers
{
    public class PackagesViewModel
    {
        //public Packages PackageModel { get; set; }

        //[Display(Name = "Schools")]
        public IEnumerable<SelectListItem> Schools { get; set; }
        public string selectedValue { get; set; }


    }
}