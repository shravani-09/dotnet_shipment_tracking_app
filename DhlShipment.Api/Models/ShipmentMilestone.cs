namespace DhlShipment.Api.Models;

public class ShipmentMilestone
{
    public Guid Id { get; set; }
    public ShipmentStatus Status { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
