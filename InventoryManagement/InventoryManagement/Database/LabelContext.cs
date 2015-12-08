using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Database
{
    public class LabelContext : DbContext
    {
        public DbSet<Labels> Labels { get; set; }
        public DbSet<Schools> Schools { get; set; }
        public DbSet<Bundles> Bundles { get; set; }
        public DbSet<Items> Items { get; set; }
    }
}