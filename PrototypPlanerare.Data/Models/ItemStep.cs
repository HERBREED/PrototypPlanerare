using System;

namespace PrototypPlanerare.Models
{
    public class ItemStep
    {
        public int Id { get; set; }

        // FK to Item
        public int ItemId { get; set; }
        public Item? Item { get; set; }

        // What/where
        public string Area { get; set; } = "General";      // Market/Engineering/Purchasing/Production/Planning
        public string Name { get; set; } = "";             // e.g., "BOM Prepared", "Stencil", "AOI", ...

        // State
        public string State { get; set; } = "NotStarted";  // NotStarted / InProgress / Blocked / Done
        public string? Assignee { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string? Notes { get; set; }
    }
}
