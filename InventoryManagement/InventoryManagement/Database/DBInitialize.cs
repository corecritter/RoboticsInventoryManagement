﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class DBInitialize<DbContext> : DropCreateDatabaseAlways<UsersContext>
    {
        protected override void Seed(UsersContext context)
        {
            Users defaultUser = new Users { UserName = "admin", Password = "admin", isAdmin = true, FirstName = "Default", LastName = "User", };
            context.Users.Add(defaultUser);
            context.SaveChanges();
        }
    }
}