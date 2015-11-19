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

        [Display(Name = "Schools")]
        public IList<Schools> Schools { get; set; };
    }
}