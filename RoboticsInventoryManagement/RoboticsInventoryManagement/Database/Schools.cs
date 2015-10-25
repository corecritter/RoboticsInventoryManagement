using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboticsInventoryManagement.Database
{
    public class Schools
    {
        public int SchoolId { get; set; } //PK
        public string SchoolName { get; set; }
        public string TeacherName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public virtual List<Items> Item { get; set; }

    }
}