using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Database
{
    public class Schools
    {
        [Key]
        public int SchoolId { get; set; } //PK

        [Required(ErrorMessage = "A School Name is required")]
        [Display(Name ="School Name")]
        public string SchoolName { get; set; }

        [Display(Name = "Teacher Name"), Required(ErrorMessage = "A Teacher Name is required")]
        public string TeacherName { get; set; }

        [Display(Name = "Email Address"), DataType(DataType.EmailAddress, ErrorMessage = "Must enter a valid email address")]
        public string Email { get; set; }

        [Display(Name = "Contact Phone"), DataType(DataType.PhoneNumber, ErrorMessage = "Must Enter a valid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "A Label for the school is required")]
        public int LabelId { get; set; }

        public virtual Labels Label { get; set; }
        public virtual List<Items> Item { get; set; }
        public virtual List<Bundles> Bundles { get; set; }
    }
}