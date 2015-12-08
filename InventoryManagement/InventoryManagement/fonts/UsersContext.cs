using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class UsersContext : DbContext
    {
        //public UsersContext() : base("InventoryDatabase")
        //{

        //}
        public UsersContext() : base("UsersContext")
        {
            if (!Database.Exists())
            {
                System.Data.Entity.Database.SetInitializer<UsersContext>(new DBInitialize<UsersContext>());
                Database.Initialize(false);
            }
        }
        public DbSet<Users> Users { get; set; } //Entity for users
        public DbSet<Items> Items { get; set; }
    }
}