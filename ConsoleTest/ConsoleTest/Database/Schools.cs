using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ConsoleTest.Database
{
    public class Schools
    {
        [Key]
        public int SchoolId { get; set; } //PK
        public string SchoolName { get; set; }
        public string TeacherName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public virtual List<Items> Item { get; set; }

    }
}