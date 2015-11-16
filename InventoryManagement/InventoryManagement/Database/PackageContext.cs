using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Database
{
    public class PackageContext : DbContext
    {
        public DbSet<Packages> Packages { get; set; }  //Entity Of Packs
        public DbSet<ItemTypes> ItemTypes { get; set;}
        public DbSet<Items> Items { get; set; }        //Entity Of Items
        public DbSet<Schools> Schools { get; set; }
    }
}