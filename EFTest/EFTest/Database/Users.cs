using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EFTest.Database
{
    public class Users
    {
        [Key]
        public int user_id { get; set; } //PK
        public string fName { get; set; }
        public string lName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int phoneNumber { get; set; }
        public bool isAdmin { get; set; }
    }
}