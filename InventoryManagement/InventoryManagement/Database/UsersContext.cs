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
                //var user = new Users(user.UserName = "admin", user.Password = "admin", user.isAdmin = true);
                //Users user = new Users { UserName = "admin", Password = "admin", isAdmin = true, Email = "a", FirstName = "a", LastName = "a", PhoneNumber = 2222222 };
                //InventoryManagement.Database.Users.Add(user);
            }
            //Database.Initialize( new CreateDatabaseIfNotExists<UsersContext>());
            //Database.SetInitializer(new CreateDatabaseIfNotExists<UsersContext>);
        }
        public DbSet<Users> Users { get; set; } //Entity for users
    }
}