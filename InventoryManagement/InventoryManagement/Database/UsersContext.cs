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
            //Database.SetInitializer(new CreateDatabaseIfNotExists<UsersContext>);
        }
        public DbSet<Users> Users { get; set; } //Entity for users
    }
}