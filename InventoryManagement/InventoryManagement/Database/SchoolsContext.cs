using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class SchoolsContext : DbContext
    {
        public DbSet<Schools> Schools { get; set; }
        public DbSet<Labels> Labels { get; set; }
        public DbSet<InventoryLocations> InventoryLocations { get; set; }
    }
}