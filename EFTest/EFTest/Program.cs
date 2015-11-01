using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFTest.Database;
namespace EFTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1 = Add Inventory Location");
                Console.WriteLine("2 = Add Item Type");
                Console.WriteLine("3 = Display Item Types");
                var selection = Console.ReadLine();
                int sel = int.Parse(selection);
                if (sel == 1)
                {
                    addInvLoc();
                }
                else if (sel == 2)
                {
                    addItemType();
                }
                else if (sel == 3)
                    printItemTypes();
                else
                    break;
            }
        }
        public static void addInvLoc()
        {
            using (var db = new ItemContext())
            {
                Console.Write("Enter a name for a new Inventory Location: ");
                var locationName = Console.ReadLine();
                var newLocation = new InventoryLocations { InventoryLocationName = locationName };
                db.InventoryLocations.Add(newLocation);
                db.SaveChanges();

                var query = from b in db.InventoryLocations
                            orderby b.InventoryLocationName
                            select b;
                foreach (var location in query)
                {
                    Console.WriteLine(location.InventoryLocationName);
                }

            }
        }
        public static void addItemType()
        {
            using (var db = new ItemContext())
            {
                // Create and save a new Item Type 
                Console.Write("Enter a name for a new Item Type: ");
                var itemName = Console.ReadLine();
                Console.Write("Enter Item Quantity: ");
                var qty = int.Parse(Console.ReadLine());
                var newItemType = new ItemTypes { ItemName = itemName, Quantity = qty };
                db.ItemTypes.Add(newItemType);
                db.SaveChanges();

                for(int i=0; i < qty; i++)
                {
                    var newItem = new Items { ItemTypeId = newItemType.ItemTypeId };
                    db.Items.Add(newItem);
                    db.SaveChanges();
                }
                
                
                printItemTypes();
                
            }
        }
        public static void printItemTypes()
        {
            using (var db = new ItemContext())
            {
                // Display all Item types from the database 
                var query = from b in db.ItemTypes
                            orderby b.ItemName
                            select b;//b.ItemName + b.Quantity;
                Console.WriteLine("All Item Types in the database:");
                foreach (var item in query)
                {
                    Console.WriteLine("Name: " + item.ItemName + " Quantity: "  + item.Quantity);
                }
            }
        }
    }
}
