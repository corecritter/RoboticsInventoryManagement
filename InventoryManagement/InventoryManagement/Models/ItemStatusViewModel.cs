using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Models
{
    public class ItemStatusViewModel
    {
        public IList<Items> CheckedOutItems { get; set; }
        public IList<Items> PendingApprovalItems { get; set; }
    }
    public class ItemsOutViewModel
    {
        public IList<Items> CheckedOutItems { get; set; }
        public IList<bool> ItemsLost { get; set; }
        public IList<bool> ItemReturn { get; set; }
    }
    public class ItemsApproveViewModel
    {
        public IList<Items> PendingApprovalItems { get; set; }
        public IList<bool> ApproveCheckBoxes { get; set; }
    }
    public class ItemsMissingLabelViewModel
    {
        public IList<Items> ItemsMissingLabel { get; set; }
        public IList<IEnumerable<SelectListItem>> Labels { get; set; }

    }
    public class ItemsMissingInventoryLocationViewModel
    {
        public IList<Items> ItemsMissingLocation { get; set; }
        public IList<IEnumerable<SelectListItem>> InventoryLocations { get; set; }
    }
}