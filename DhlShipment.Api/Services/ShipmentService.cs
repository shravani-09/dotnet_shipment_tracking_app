using DhlShipment.Api.DTOs;
using DhlShipment.Api.Models;

namespace DhlShipment.Api.Services;

public class ShipmentService : IShipmentService
{
    private static readonly List<Shipment> Shipments = new();
    private readonly IShipmentLifecycleValidator _lifecycleValidator;

    public ShipmentService()
    {
        _lifecycleValidator = new ShipmentLifecycleValidator();
    }

    public IEnumerable<ShipmentResponseDto> GetAllShipments()
    {
        return Shipments.Select(MapToDto).ToList();
    }

    public ShipmentResponseDto CreateShipment(CreateShipmentDto dto)
    {
        var shipment = new Shipment
        {
            TrackingId = $"DHL{Random.Shared.Next(100000, 999999)}",
            Origin = dto.Origin,
            Destination = dto.Destination,
            EstimatedDeliveryDate = dto.EstimatedDeliveryDate,
            CurrentStatus = ShipmentStatus.Created,
            Milestones = new List<ShipmentMilestone>
            {
                new()
                {
                    Status = ShipmentStatus.Created,
                    Location = dto.Origin,
                    Timestamp = DateTime.UtcNow
                }
            }
        };

        Shipments.Add(shipment);

        return MapToDto(shipment);
    }

    public ShipmentResponseDto GetShipmentByTrackingId(string trackingId)
    {
        var shipment = Shipments.FirstOrDefault(s => s.TrackingId == trackingId);

        if (shipment == null)
            throw new KeyNotFoundException("Shipment not found");

        return MapToDto(shipment);
    }

    private static ShipmentResponseDto MapToDto(Shipment shipment)
    {
        return new ShipmentResponseDto(
            shipment.TrackingId,
            shipment.Origin,
            shipment.Destination,
            shipment.EstimatedDeliveryDate,
            shipment.CurrentStatus,
            shipment.Milestones.Select(m =>
                new ShipmentMilestoneDto(
                    m.Status,
                    m.Location,
                    m.Timestamp
                ))
        );
    }

    public ShipmentResponseDto UpdateShipmentStatus(
        string trackingId,
        UpdateShipmentStatusDto dto)
    {
        var shipment = Shipments.FirstOrDefault(s => s.TrackingId == trackingId);

        if (shipment == null)
            throw new KeyNotFoundException("Shipment not found");

        _lifecycleValidator.ValidateTransition(shipment.CurrentStatus, dto.Status);

        shipment.CurrentStatus = dto.Status;

        shipment.Milestones.Add(new ShipmentMilestone
        {
            Status = dto.Status,
            Location = dto.Location,
            Timestamp = DateTime.UtcNow
        });

        return MapToDto(shipment);
    }

    public void ClearShipments()
    {
        Shipments.Clear();
    }

}
