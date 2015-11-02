using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class SchoolsContext : DbContext
    {
        public DbSet<InventoryLocations> InventoryLocations { get; set; }

        public DbSet<InventoryManagement.Database.Schools> Schools { get; set; }
    }
}