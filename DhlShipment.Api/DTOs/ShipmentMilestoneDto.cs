using DhlShipment.Api.Models;

namespace DhlShipment.Api.DTOs;

public record ShipmentMilestoneDto(
    ShipmentStatus Status,
    string Location,
    DateTime Timestamp
);
