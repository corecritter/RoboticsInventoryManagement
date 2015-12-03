using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Models
{
    public class SchoolsViewModel
    {
        public Schools SchoolModel { get; set; }
        public IList<SelectListItem> Labels { get; set; }
    }
}