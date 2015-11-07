using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class ItemTypes
    {
        public ItemTypes()
        {
            this.Item = new List<Items>();
        }
        [Key]
        public int ItemTypeId { get; set; } //PK
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public virtual List<Items> Item { get; set; } 
    }
}