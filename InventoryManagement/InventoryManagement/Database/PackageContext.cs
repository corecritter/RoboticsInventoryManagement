﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Database
{
    public class PackageContext
    {
        public class ItemContext : DbContext
        {
            public DbSet<Packs> Packs { get; set; }  //Entity Of Packs
            public DbSet<Items> Items { get; set; }           //Entity Of Items
            public DbSet<InventoryLocations> InventoryLocations { get; set; }
        }
    }
}