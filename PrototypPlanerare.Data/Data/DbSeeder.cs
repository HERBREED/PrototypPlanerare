using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PrototypPlanerare.Models;

namespace PrototypPlanerare.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            // Ensure DB exists (Migrate() already runs, but this is harmless)
            db.Database.EnsureCreated();

            // If any items already exist, do nothing
            if (db.Items.Any())
                return;

            var item1 = new Item
            {
                Date = new DateTime(2023, 12, 7),
                EcoNumber = "MM231109",
                Customer = "Waybler",
                Product = "Vivo65 CPU",
                CurrentRevision = "R10",
                NewRevision = "N/A",
                Quantity = 20,
                Type = "ECO",
                Status = "InProgress",
                OwnerMarket = "Anna",
                OwnerEngineering = "Kari",
                OwnerPurchasing = "Lisa",
                OwnerPlanning = "Johan",
                Notes = "Alla modeller utom MID-mätare."
            };
            item1.Steps.Add(new ItemStep { Area = "Engineering", Name = "BOM Prepared", State = "Done" });
            item1.Steps.Add(new ItemStep { Area = "Engineering", Name = "Assembly Instruction", State = "InProgress" });
            item1.Steps.Add(new ItemStep { Area = "Purchasing", Name = "PCB Confirmed", State = "NotStarted" });

            var item2 = new Item
            {
                Date = new DateTime(2023, 12, 19),
                EcoNumber = "CH-23-009",
                Customer = "Waybler",
                Product = "Dynamic laddare",
                CurrentRevision = "R8",
                NewRevision = "R11B",
                Quantity = 4,
                Type = "PROTOTYPE",
                Status = "Blocked",
                OwnerEngineering = "Karl",
                Notes = "Ny laddare med flera kort. Väntar info."
            };
            item2.Steps.Add(new ItemStep { Area = "Engineering", Name = "Stencil", State = "Blocked", Notes = "Ritning saknas" });

            db.Items.AddRange(item1, item2);
            db.SaveChanges();
        }
    }
}
