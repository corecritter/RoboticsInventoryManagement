using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InventoryManagement.Database;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace InventoryManagement.Models
{
    public class CheckOutViewModel
    {
        //For the selected school id
        public int SelectedSchoolId { get; set; }
        //For the selected bundle id
        public int SelectedBundleId { get; set; }

        //for the index view
        [Display(Name = "Schools")]
        public IList<Schools> Schools { get; set; }

        //For BundleSelection View
        public IList<Bundles> Bundles { get; set; }

        //For ItemTypesSelectView
        public IList<ItemTypes> ItemTypesModel { get; set; }
        public IList<bool> ItemTypesCheckboxes { get; set; }
        public IList<int> SelectedItemTypesIds { get; set; }

        //For ItemsSelect View
        public IList<Items> ItemsToCheckOut { get; set; }
    }
}