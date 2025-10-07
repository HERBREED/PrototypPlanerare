using System;

namespace PrototypPlanerare.Models
{
    public class ItemComment
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Author { get; set; } = "";
        public string Text { get; set; } = "";
        public string Section { get; set; } = "Overview"; // Overview, Market, Engineering, ...

    }
}
