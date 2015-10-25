using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboticsInventoryManagement.Database
{
    public class Users
    {
        public int user_id { get; set; } //PK
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int phoneNumber { get; set; }
        public bool isAdmin { get; set; }
    }
}