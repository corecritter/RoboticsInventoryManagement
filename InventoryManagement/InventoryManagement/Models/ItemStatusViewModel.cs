using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class ItemStatusViewModel
    {
        public IList<Items> CheckedOutItems { get; set; }
        public IList<Items> PendingApprovalItems { get; set; }
    }
}