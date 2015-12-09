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

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress), Display(Name = "Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage ="A Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.PhoneNumber), Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Administrator")]
        public bool isAdmin { get; set; }
    }
}