namespace PrototypPlanerare.Models
{
    public enum EngineeringStatus
    {
        NA = 0,           // Not applicable
        NotStarted = 1,
        InProgress = 2,
        Blocked = 3,
        Done = 4
    }

    public class EngineeringTask
    {
        public int Id { get; set; }

        // FK to Item
        public int ItemId { get; set; }
        public Item? Item { get; set; }

        // A stable key you can use for defaults ("assembly_instructions", etc.)
        public string Key { get; set; } = string.Empty;

        // What you display in the UI
        public string Title { get; set; } = string.Empty;

        public EngineeringStatus Status { get; set; } = EngineeringStatus.NotStarted;

        public bool IsRequired { get; set; } = true;

        public string? Owner { get; set; }
        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
