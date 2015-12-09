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
        [Required(ErrorMessage = "A Bundle Name is required")]
        public string BundleName { get; set; }

        public List<Schools> Schools { get; set; }
        public List<ItemTypes> ItemTypes { get; set; }
        
        public IList<int> SelectedSchoolIds { get; set; }
        public IList<bool> SchoolsCheckboxes { get; set; }
        
        public List<ItemTypes> SelectedItemTypes { get; set; }
        public IList<bool> ItemTypesCheckboxes { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be no less than 1.")]
        public IList<int> ItemQuantityFields { get; set; }
    }
    public class BundlesDetailsViewModel
    {
        public Bundles BundleModel { get; set; }
        public IList<string> ItemDisplayString { get; set; }
        public IList<string> ItemDisplayLabels { get; set; }
    }
}