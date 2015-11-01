using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class SchoolsContext
    {
        public DbSet<InventoryLocations> InventoryLocations { get; set; }
    }
}