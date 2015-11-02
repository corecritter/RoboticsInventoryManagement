using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Database
{
    public class Packs
    {
        [Key]
        public int PackId { get; set; } //PK
        public string PackName { get; set; }
        public virtual List<Items> Item { get; set; }
    }
}