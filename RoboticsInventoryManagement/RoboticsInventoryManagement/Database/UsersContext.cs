﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RoboticsInventoryManagement.Database
{
    public class UsersContext : DbContext
    {
        public DbSet<Users> Users { get; set; } //Entity for users
    }
}