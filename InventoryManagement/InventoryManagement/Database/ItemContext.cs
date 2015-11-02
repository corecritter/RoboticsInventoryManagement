using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Database
{
    public class ItemContext : DbContext
    {
        public DbSet<ItemTypes> ItemTypes { get; set; }  //Entity Of Item Types
        public DbSet<Items> Items{ get; set; }           //Entity Of Items
        public DbSet<InventoryLocations> InventoryLocations {get; set;}

        public System.Data.Entity.DbSet<InventoryManagement.Database.Labels> Labels { get; set; }

        public System.Data.Entity.DbSet<InventoryManagement.Database.Schools> Schools { get; set; }
    }
}