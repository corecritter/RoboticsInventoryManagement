using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Database
{
    public class InventoryContext : DbContext
    {
        public DbSet<InventoryLocations> InventoryLocations { get; set; }
        public DbSet<Items> Items { get; set; }
    }
}