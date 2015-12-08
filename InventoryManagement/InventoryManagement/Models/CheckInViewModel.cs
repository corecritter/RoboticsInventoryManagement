using InventoryManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class CheckInViewModel
    {
        //For Index View (Selecting Items to check in)
        public IList<Items> RentedItems { get; set; }
        public IList<bool> RentedItemsCheckboxes { get; set; }
    }
    public class CheckInItemTypeSelectViewModel
    {
        //For ItemTypesSelectView
        public IList<ItemTypes> ItemTypesModel { get; set; }
        public IList<bool> ItemTypesCheckboxes { get; set; }
        public int SelectedSchoolId { get; set; }
    }
    public class CheckInQuantitySelectViewModel
    {
        public IList<int> ItemQuantityFields { get; set; }
        public IList<ItemTypes> SelectedItemTypesModel { get; set; }
        public int SelectedSchoolId { get; set; }
    }
    public class CheckInItemConfirmModel
    {
        public IList<Items> ItemsToReturn { get; set; }
        public IList<string> ItemDisplayString { get; set; }
        public IList<string> ItemDisplayLabels { get; set; }
        public int SelectedSchoolId { get; set; }
    }


}