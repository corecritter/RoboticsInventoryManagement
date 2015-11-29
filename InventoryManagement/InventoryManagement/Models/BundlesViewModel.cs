using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Models
{
    public class BundlesViewModel
    {
        public List<Schools> Schools { get; set; }
        public List<ItemTypes> ItemTypes { get; set; }
        [Required(ErrorMessage = "A Bundle Name is required")]
        public string BundleName { get; set;}
        //public Packages PackageModel { get; set; }

        //[Display(Name = "Schools")]
        //public IEnumerable<SelectListItem> Schools { get; set; }
        //public string selectedValue { get; set; }
        public IList<int> SelectedSchoolIds { get; set; }
        public IList<bool> SchoolsCheckboxes { get; set; }
        public IList<bool> ItemTypesCheckboxes { get; set; }
    }
}