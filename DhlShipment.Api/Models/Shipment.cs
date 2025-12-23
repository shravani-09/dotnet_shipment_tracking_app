namespace DhlShipment.Api.Models;

public class Shipment
{
    public Guid Id { get; set; }
    public string TrackingId { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime EstimatedDeliveryDate { get; set; }
    public ShipmentStatus CurrentStatus { get; set; } = ShipmentStatus.Created;

    public List<ShipmentMilestone> Milestones { get; set; } = new();
}
