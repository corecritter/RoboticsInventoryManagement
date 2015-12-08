using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Database
{
    public class Labels
    {
        [Key]
        public int LabelId { get; set; }
        [Display(Name = "Label Name")]
        public string LabelName { get; set; }

        public virtual List<Items> Items { get; set; }
    }
}