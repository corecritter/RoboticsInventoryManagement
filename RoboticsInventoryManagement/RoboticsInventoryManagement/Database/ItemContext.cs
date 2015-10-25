using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RoboticsInventoryManagement.Database
{
    public class ItemContext : DbContext
    {
        public DbSet<ItemTypes> ItemTypes { get; set; }  //Entity Of Item Types
        public DbSet<Items> Items{ get; set; }           //Entity Of Items
    }
}