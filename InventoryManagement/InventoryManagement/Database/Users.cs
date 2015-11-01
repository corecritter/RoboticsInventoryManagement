using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Database
{
    public class Users
    {
        //[Key]
        //public int UserId { get; set; } //PK
        [Key]
        [Required(ErrorMessage = "User Name is Required and must be unique")]
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage ="A Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int PhoneNumber { get; set; }
        public bool isAdmin { get; set; }
    }
}