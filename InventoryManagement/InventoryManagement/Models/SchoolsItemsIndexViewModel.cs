using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class SchoolsItemsIndexViewModel
    {
        public IList<Schools> Schools { get; set; }
    }
    public class SchoolsItemsViewModel
    {
        public IList<Items> CheckedOutItems { get; set; }
    }
}