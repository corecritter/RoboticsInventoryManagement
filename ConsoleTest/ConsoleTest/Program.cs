using ConsoleTest.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1 = Add Inventory Location");
                Console.WriteLine("2 = Add Item Type");
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
                var newItemType = new ItemTypes { ItemName = itemName };
                db.ItemTypes.Add(newItemType);
                db.SaveChanges();

                // Display all Blogs from the database 
                var query = from b in db.ItemTypes
                            orderby b.ItemName
                            select b;

                Console.WriteLine("All blogs in the database:");
                foreach (var item in query)
                {
                    Console.WriteLine(item.ItemName);
                }

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
